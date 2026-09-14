namespace HubPedidos.Application.Services;

using System.Buffers;
using System.Text;
using HubPedidos.Application.DTOs;

public class LotePedidoBufferedService
{
    private const int BufferSize = 4096;

    public async Task<List<CriarPedidoRequest>> ProcessarStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            int bytesLidos = await stream.ReadAsync(buffer.AsMemory(0, BufferSize), cancellationToken);
            if (bytesLidos == 0)
                return new List<CriarPedidoRequest>();

            string conteudoCsv = Encoding.UTF8.GetString(buffer, 0, bytesLidos);
            return LotePedidoParserOtimizado.ParseCsv(conteudoCsv.AsSpan());
        }
        finally
        {
            // Limpeza do buffer por segurança (dados sensíveis) e devolução ao Pool
            Array.Clear(buffer, 0, buffer.Length);
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
