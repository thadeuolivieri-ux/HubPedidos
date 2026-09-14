using HubPedidos.Application.DTOs;
using HubPedidos.Application.Mappers;
using HubPedidos.Application.Services;
using HubPedidos.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<CatalogoProcessadores>();

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

app.Run();
