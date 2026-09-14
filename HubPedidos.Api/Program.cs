using System.Threading.Channels;
using HubPedidos.Application.Data;
using HubPedidos.Application.DTOs;
using HubPedidos.Application.Jobs;
using HubPedidos.Application.Mappers;
using HubPedidos.Application.Services;
using HubPedidos.Domain.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<CatalogoProcessadores>();

builder.Services.AddDbContext<HubPedidosDbContext>(options =>
    options.UseSqlite("Data Source=hubpedidos.db"));

builder.Services.AddScoped<PedidoConsultaService>();
builder.Services.AddSingleton<QueueMetricsService>();

// Configuração do Canal Limitado (Bounded Channel)
var channel = Channel.CreateBounded<ProcessarPedidoJob>(new BoundedChannelOptions(capacity: 10)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleReader = false,
    SingleWriter = false
});

builder.Services.AddSingleton(channel);
builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);
builder.Services.AddHostedService<PedidoQueueWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
    return Results.StatusCode(429); // 429 Too Many Requests (Backpressure / Fila cheia)
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
