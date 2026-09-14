using System.Threading.Channels;
using HubPedidos.Api.Hubs;
using HubPedidos.Application;
using HubPedidos.Application.DTOs;
using HubPedidos.Application.Events;
using HubPedidos.Application.Jobs;
using HubPedidos.Application.Mappers;
using HubPedidos.Application.Services;
using HubPedidos.Domain.Services;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("CachePublicoPedidos", b => b.Expire(TimeSpan.FromSeconds(60)).Tag("pedidos"));
});

builder.Services.AddApplicationModule();
builder.Services.AddInfrastructureModule("Data Source=hubpedidos.db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseOutputCache();
app.MapHub<PedidosHub>("/hubs/pedidos");

app.MapGet("/api/v1/pedidos/etag-demonstracao", (HttpContext context) =>
{
    var resumo = new PedidoResumoDto(Guid.Parse("11111111-1111-1111-1111-111111111111"), "SP", 5, 2, 250.00m);
    string etag = ETagHelper.GerarETag(resumo);
    string? ifNoneMatch = context.Request.Headers.IfNoneMatch;

    if (ETagHelper.ETagValida(ifNoneMatch, etag))
    {
        return Results.StatusCode(StatusCodes.Status304NotModified);
    }

    context.Response.Headers.ETag = etag;
    return Results.Ok(resumo);
});

app.MapGet("/api/v1/pedidos/{id:guid}/resumo", async (Guid id, PedidoConsultaService service, HttpContext context) =>
{
    context.Response.Headers["Deprecation"] = "true";
    context.Response.Headers["Sunset"] = "Wed, 31 Dec 2026 23:59:59 GMT";

    var resumo = await service.ObterResumoCompiladoAsync(id);
    return resumo is not null ? Results.Ok(resumo) : Results.NotFound();
});

app.MapGet("/api/v2/pedidos/{id:guid}/resumo", async (Guid id, PedidoConsultaService service) =>
{
    var resumoV1 = await service.ObterResumoCompiladoAsync(id);
    if (resumoV1 is null) return Results.NotFound();

    var taxa = resumoV1.ValorTotal * 0.05m;
    var resumoV2 = new PedidoResumoV2Dto(
        resumoV1.Id,
        resumoV1.Regiao,
        resumoV1.Prioridade,
        resumoV1.QuantidadeTotalItens,
        resumoV1.ValorTotal,
        taxa,
        resumoV1.ValorTotal + taxa,
        "PROCESSADO"
    );

    return Results.Ok(resumoV2);
});

app.MapPost("/api/pedidos/processar", (CriarPedidoRequest request, CatalogoProcessadores catalogo) =>
{
    try
    {
        var pedido = PedidoMapper.ToDomain(request);
        var categoria = ClassificadorPedido.Classificar(pedido);
        var processador = catalogo.ObterProcessador(categoria);
        var resultadoMensagem = processador.Processar(pedido);

        var response = new ProcessamentoResultadoResponse(
            PedidoMapper.ToResponse(pedido),
            categoria.ToString(),
            resultadoMensagem
        );

        return Results.Ok(response);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.UnprocessableEntity(new { erro = ex.Message });
    }
});

app.MapPost("/api/fila/pedidos", async (ProcessarPedidoJob job, ChannelWriter<ProcessarPedidoJob> writer, QueueMetricsService metrics) =>
{
    if (writer.TryWrite(job))
    {
        metrics.IncrementEnfileirados();
        return Results.Accepted($"/api/pedidos/{job.PedidoId}", new { status = "Enfileirado", job.PedidoId });
    }

    metrics.IncrementRejeitados();
    return Results.StatusCode(429);
});

app.MapGet("/api/fila/metricas", (QueueMetricsService metrics) =>
{
    return Results.Ok(new
    {
        enfileirados = metrics.ItensEnfileirados,
        processados = metrics.ItensProcessados,
        falhados = metrics.ItensFalhados,
        rejeitados = metrics.ItensRejeitados
    });
});

app.Run();

public partial class Program { }
