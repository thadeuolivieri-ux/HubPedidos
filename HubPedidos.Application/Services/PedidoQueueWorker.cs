namespace HubPedidos.Application.Services;

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HubPedidos.Application.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PedidoQueueWorker : BackgroundService
{
    private readonly ChannelReader<ProcessarPedidoJob> _reader;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly QueueMetricsService _metrics;
    private readonly ILogger<PedidoQueueWorker> _logger;
    private readonly SemaphoreSlim _dbSemaphore;

    public PedidoQueueWorker(
        ChannelReader<ProcessarPedidoJob> reader,
        IServiceScopeFactory scopeFactory,
        QueueMetricsService metrics,
        ILogger<PedidoQueueWorker> logger,
        int maxConcorrenciaBanco = 5)
    {
        _reader = reader;
        _scopeFactory = scopeFactory;
        _metrics = metrics;
        _logger = logger;
        _dbSemaphore = new SemaphoreSlim(maxConcorrenciaBanco, maxConcorrenciaBanco);
    }

    public int ConcorrenciaAtiva => _dbSemaphore.CurrentCount;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _reader.ReadAllAsync(stoppingToken))
        {
            _ = ProcessarJobAsync(job, stoppingToken);
        }
    }

    private async Task ProcessarJobAsync(ProcessarPedidoJob job, CancellationToken cancellationToken)
    {
        await _dbSemaphore.WaitAsync(cancellationToken);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            await Task.Delay(20, cancellationToken);
            _metrics.IncrementProcessados();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _metrics.IncrementFalhados();
            _logger.LogError(ex, "Falha ao processar job {PedidoId}", job.PedidoId);
        }
        finally
        {
            _dbSemaphore.Release();
        }
    }
}
