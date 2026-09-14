using HubPedidos.Application.Data;
using HubPedidos.Application.DTOs;
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

app.MapGet("/api/pedidos/painel", async (PedidoConsultaService service, int pagina = 1, int tamanho = 10, string? regiao = null) =>
{
    var resultado = await service.ListarPaginadoAsync(pagina, tamanho, regiao);
    return Results.Ok(resultado);
});

app.MapGet("/api/pedidos/{id:guid}/resumo-compilado", async (Guid id, PedidoConsultaService service) =>
{
    var resumo = await service.ObterResumoCompiladoAsync(id);
    return resumo is not null ? Results.Ok(resumo) : Results.NotFound();
});

app.Run();
