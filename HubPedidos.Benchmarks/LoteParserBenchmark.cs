namespace HubPedidos.Benchmarks;

using BenchmarkDotNet.Attributes;
using HubPedidos.Application.Services;

[MemoryDiagnoser]
public class LoteParserBenchmark
{
    private string _csvData = null!;

    [Params(10, 100)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("ProdutoId,Quantidade,PrecoUnitario,Regiao,Prioridade");
        for (int i = 0; i < N; i++)
        {
            sb.AppendLine($"PROD-{i},{i + 1},{10.5 + i},SP,3");
        }
        _csvData = sb.ToString();
    }

    [Benchmark(Baseline = true)]
    public object Baseline() => LotePedidoParserBaseline.ParseCsv(_csvData);

    [Benchmark]
    public object OtimizadoSpan() => LotePedidoParserOtimizado.ParseCsv(_csvData.AsSpan());
}
