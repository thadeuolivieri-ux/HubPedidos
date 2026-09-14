namespace HubPedidos.Tests;

using HubPedidos.Domain.Entities;
using HubPedidos.Domain.Enums;
using HubPedidos.Domain.Services;
using HubPedidos.Domain.ValueObjects;

public class ClassificadorPedidoTests
{
    [Theory]
    [InlineData(60, 10, "RJ", 1, CategoriaProcessamento.Atacado)]
    [InlineData(5, 1200, "RJ", 1, CategoriaProcessamento.VIP)]
    [InlineData(5, 100, "SP", 1, CategoriaProcessamento.Express)]
    [InlineData(2, 50, "PR", 1, CategoriaProcessamento.Padrao)]
    public void Classificar_DeveRetornarCategoriaCorreta(
        int quantidade, decimal preco, string regiao, int prioridade, CategoriaProcessamento esperada)
    {
        var itens = new[] { new ItemPedido("PROD-01", quantidade, preco) };
        var pedido = new Pedido(PedidoId.New(), itens, regiao, prioridade);

        var resultado = ClassificadorPedido.Classificar(pedido);

        Assert.Equal(esperada, resultado);
    }

    [Fact]
    public void Classificar_PedidoNulo_DeveRetornarInvalido()
    {
        var resultado = ClassificadorPedido.Classificar(null!);
        Assert.Equal(CategoriaProcessamento.Invalido, resultado);
    }
}
