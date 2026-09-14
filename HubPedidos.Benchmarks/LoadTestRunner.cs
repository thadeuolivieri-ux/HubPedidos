namespace HubPedidos.Benchmarks;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class LoadTestRunner
{
    private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

    public static async Task ExecutarSimulacaoAsync(string endpoint, int totalRequisicoes = 500, int concorrencia = 10)
    {
        Console.WriteLine($"=== Iniciando Teste de Carga: {endpoint} ===");
        Console.WriteLine($"Total Recomendações: {totalRequisicoes} | Concorrência: {concorrencia}");

        // Warmup (Aquecimento)
        for (int i = 0; i < 20; i++)
        {
            try { await Client.GetAsync(endpoint); } catch { }
        }

        var latencias = new ConcurrentBag<double>();
        var erros = new ConcurrentBag<bool>();
        var stopwatch = Stopwatch.StartNew();

        var options = new ParallelOptions { MaxDegreeOfParallelism = concorrencia };

        await Parallel.ForEachAsync(Enumerable.Range(0, totalRequisicoes), options, async (_, ct) =>
        {
            var reqStopwatch = Stopwatch.StartNew();
            try
            {
                using var response = await Client.GetAsync(endpoint, ct);
                reqStopwatch.Stop();
                latencias.Add(reqStopwatch.Elapsed.TotalMilliseconds);

                if (!response.IsSuccessStatusCode)
                    erros.Add(true);
            }
            catch
            {
                reqStopwatch.Stop();
                erros.Add(true);
            }
        });

        stopwatch.Stop();

        var listaLatencias = latencias.OrderBy(x => x).ToList();
        if (listaLatencias.Count == 0)
        {
            Console.WriteLine("Nenhuma requisição concluída. Verifique se a API está no ar.");
            return;
        }

        double tempoTotalSeg = stopwatch.Elapsed.TotalSeconds;
        double throughput = listaLatencias.Count / tempoTotalSeg;
        double p50 = CalcularPercentil(listaLatencias, 0.50);
        double p95 = CalcularPercentil(listaLatencias, 0.95);
        double p99 = CalcularPercentil(listaLatencias, 0.99);
        double taxaErro = ((double)erros.Count / totalRequisicoes) * 100;

        Console.WriteLine($"Throughput: {throughput:F2} req/seg");
        Console.WriteLine($"Latência p50: {p50:F2} ms");
        Console.WriteLine($"Latência p95: {p95:F2} ms");
        Console.WriteLine($"Latência p99: {p99:F2} ms");
        Console.WriteLine($"Taxa de Erros: {taxaErro:F2}%\n");
    }

    private static double CalcularPercentil(List<double> sequenciaOrdenada, double percentil)
    {
        int idx = (int)Math.Ceiling(percentil * sequenciaOrdenada.Count) - 1;
        return sequenciaOrdenada[Math.Max(0, idx)];
    }
}
