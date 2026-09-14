namespace HubPedidos.Tests;

using System;
using System.Threading.Tasks;
using HubPedidos.Application.Data;
using HubPedidos.Application.Services;
using HubPedidos.Domain.Entities;
using HubPedidos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class PedidoConsultaServiceTests
{
    private HubPedidosDbContext ObterDbContextInMemory()
    {
        var options = new DbContextOptionsBuilder<HubPedidosDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new HubPedidosDbContext(options);
    }

    [Fact]
    public async Task ListarPaginadoAsync_DeveRetornarSomenteQuantidadePaginada()
    {
        using var context = ObterDbContextInMemory();

        for (int i = 1; i <= 15; i++)
        {
            var itens = new[] { new ItemPedido("PROD-1", i, 10.0m) };
            context.Pedidos.Add(new Pedido(PedidoId.New(), itens, "SP", i));
        }
        await context.SaveChangesAsync();

        var service = new PedidoConsultaService(context);
        var resultado = await service.ListarPaginadoAsync(pagina: 1, tamanhoPagina: 10);

        Assert.Equal(15, resultado.TotalRegistros);
        Assert.Equal(10, resultado.Itens.Count);
    }

    [Fact]
    public async Task ObterResumoCompiladoAsync_DeveRetornarResumoCorreto()
    {
        using var context = ObterDbContextInMemory();
        var pedidoId = PedidoId.New();
        var itens = new[] { new ItemPedido("PROD-A", 2, 50.0m) };
        var pedido = new Pedido(pedidoId, itens, "RJ", 5);

        context.Pedidos.Add(pedido);
        await context.SaveChangesAsync();

        var service = new PedidoConsultaService(context);
        var resumo = await service.ObterResumoCompiladoAsync(pedidoId.Value);

        Assert.NotNull(resumo);
        Assert.Equal(pedidoId.Value, resumo.Id);
        Assert.Equal("RJ", resumo.Regiao);
        Assert.Equal(100.0m, resumo.ValorTotal);
    }
}
