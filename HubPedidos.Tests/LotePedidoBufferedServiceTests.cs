namespace HubPedidos.Tests;

using System.Text;
using HubPedidos.Application.Services;

public class LotePedidoBufferedServiceTests
{
    [Fact]
    public async Task ProcessarStreamAsync_StreamValido_DeveRetornarPedidos()
    {
        var csv = "ProdutoId,Quantidade,PrecoUnitario,Regiao,Prioridade\nPROD-1,10,50.0,SP,3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var service = new LotePedidoBufferedService();

        var pedidos = await service.ProcessarStreamAsync(stream);

        Assert.Single(pedidos);
        Assert.Equal("SP", pedidos[0].Regiao);
    }

    [Fact]
    public async Task ProcessarStreamAsync_CancelamentoSolicitado_DeveLancarExcecaoSemVazamento()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("PROD-1,10,50.0,SP,3"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var service = new LotePedidoBufferedService();

        await Assert.ThrowsAsync<TaskCanceledException>(() => service.ProcessarStreamAsync(stream, cts.Token));
    }
}
