namespace HubPedidos.Tests;

using HubPedidos.Application.Services;

public class LotePedidoParserBaselineTests
{
    [Fact]
    public void ParseCsv_ConteudoValido_DeveRetornarListaDePedidos()
    {
        var csv = "ProdutoId,Quantidade,PrecoUnitario,Regiao,Prioridade\nPROD-1,10,50.0,SP,3\nPROD-2,5,100.0,RJ,5";

        var pedidos = LotePedidoParserBaseline.ParseCsv(csv);

        Assert.Equal(2, pedidos.Count);
        Assert.Equal("SP", pedidos[0].Regiao);
        Assert.Equal(3, pedidos[0].Prioridade);
    }

    [Fact]
    public void ParseCsv_EntradaVazia_DeveRetornarListaVazia()
    {
        var pedidos = LotePedidoParserBaseline.ParseCsv("");
        Assert.Empty(pedidos);
    }
}
