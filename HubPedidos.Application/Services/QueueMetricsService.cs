namespace HubPedidos.Application.Services;

using System.Threading;

public class QueueMetricsService
{
    private long _itensEnfileirados;
    private long _itensProcessados;
    private long _itensFalhados;
    private long _itensRejeitados;

    public long ItensEnfileirados => Interlocked.Read(ref _itensEnfileirados);
    public long ItensProcessados => Interlocked.Read(ref _itensProcessados);
    public long ItensFalhados => Interlocked.Read(ref _itensFalhados);
    public long ItensRejeitados => Interlocked.Read(ref _itensRejeitados);

    public void IncrementEnfileirados() => Interlocked.Increment(ref _itensEnfileirados);
    public void IncrementProcessados() => Interlocked.Increment(ref _itensProcessados);
    public void IncrementFalhados() => Interlocked.Increment(ref _itensFalhados);
    public void IncrementRejeitados() => Interlocked.Increment(ref _itensRejeitados);
}
