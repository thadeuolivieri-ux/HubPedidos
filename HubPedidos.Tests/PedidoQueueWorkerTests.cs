namespace HubPedidos.Tests;

using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using HubPedidos.Application.Jobs;
using HubPedidos.Application.Services;
using Xunit;

public class PedidoQueueWorkerTests
{
    [Fact]
    public void Channel_TryWrite_QuandoFilaCheia_DeveRetornarFalsoERejeitar()
    {
        var channel = Channel.CreateBounded<ProcessarPedidoJob>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        var job1 = new ProcessarPedidoJob(Guid.NewGuid(), "SP", 1, 100m, DateTime.UtcNow);
        var job2 = new ProcessarPedidoJob(Guid.NewGuid(), "RJ", 2, 200m, DateTime.UtcNow);

        var escreveu1 = channel.Writer.TryWrite(job1);
        var escreveu2 = channel.Writer.TryWrite(job2);

        Assert.True(escreveu1);
        Assert.False(escreveu2);
    }

    [Fact]
    public void QueueMetricsService_OperacoesAtomicas_DevemIncrementarCorretamente()
    {
        var metrics = new QueueMetricsService();

        Parallel.For(0, 100, _ =>
        {
            metrics.IncrementEnfileirados();
            metrics.IncrementProcessados();
        });

        Assert.Equal(100, metrics.ItensEnfileirados);
        Assert.Equal(100, metrics.ItensProcessados);
    }
}
