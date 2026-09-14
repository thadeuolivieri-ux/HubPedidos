namespace HubPedidos.Tests;

using HubPedidos.Application.Services;

public class LotePedidoParserEquivalenciaTests
{
    [Theory]
    [InlineData("ProdutoId,Quantidade,PrecoUnitario,Regiao,Prioridade\nPROD-1,10,50.0,SP,3\nPROD-2,5,100.0,RJ,5")]
    [InlineData("PROD-99,1,10.0,MG,1")]
    [InlineData("")]
    public void Parsers_DevemRetornarResultadosIdenticos(string csv)
    {
        var baseline = LotePedidoParserBaseline.ParseCsv(csv);
        var otimizado = LotePedidoParserOtimizado.ParseCsv(csv.AsSpan());

        Assert.Equal(baseline.Count, otimizado.Count);

        for (int i = 0; i < baseline.Count; i++)
        {
            Assert.Equal(baseline[i].Regiao, otimizado[i].Regiao);
            Assert.Equal(baseline[i].Prioridade, otimizado[i].Prioridade);

            var itemBase = baseline[i].Itens.First();
            var itemOtimizado = otimizado[i].Itens.First();

            Assert.Equal(itemBase.ProdutoId, itemOtimizado.ProdutoId);
            Assert.Equal(itemBase.Quantidade, itemOtimizado.Quantidade);
            Assert.Equal(itemBase.PrecoUnitario, itemOtimizado.PrecoUnitario);
        }
    }
}
