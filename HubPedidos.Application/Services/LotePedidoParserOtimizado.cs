namespace HubPedidos.Application.Services;

using System;
using System.Collections.Generic;
using HubPedidos.Application.DTOs;

public static class LotePedidoParserOtimizado
{
    public static List<CriarPedidoRequest> ParseCsv(ReadOnlySpan<char> conteudoCsv)
    {
        var resultado = new List<CriarPedidoRequest>();
        if (conteudoCsv.IsEmpty || conteudoCsv.IsWhiteSpace())
            return resultado;

        var reader = conteudoCsv;
        while (!reader.IsEmpty)
        {
            int lineLength = reader.IndexOf('\n');
            ReadOnlySpan<char> line;

            if (lineLength < 0)
            {
                line = reader;
                reader = ReadOnlySpan<char>.Empty;
            }
            else
            {
                line = reader.Slice(0, lineLength);
                reader = reader.Slice(lineLength + 1);
            }

            line = line.Trim('\r').Trim();
            if (line.IsEmpty || line.StartsWith("ProdutoId", StringComparison.OrdinalIgnoreCase))
                continue;

            ParseLine(line, resultado);
        }

        return resultado;
    }

    private static void ParseLine(ReadOnlySpan<char> line, List<CriarPedidoRequest> resultado)
    {
        int idx1 = line.IndexOf(',');
        if (idx1 < 0) return;
        var produtoIdSpan = line.Slice(0, idx1).Trim();

        var rest1 = line.Slice(idx1 + 1);
        int idx2 = rest1.IndexOf(',');
        if (idx2 < 0) return;
        var qtdSpan = rest1.Slice(0, idx2).Trim();

        var rest2 = rest1.Slice(idx2 + 1);
        int idx3 = rest2.IndexOf(',');
        if (idx3 < 0) return;
        var precoSpan = rest2.Slice(0, idx3).Trim();

        var rest3 = rest2.Slice(idx3 + 1);
        int idx4 = rest3.IndexOf(',');
        if (idx4 < 0) return;
        var regiaoSpan = rest3.Slice(0, idx4).Trim();
        var prioridadeSpan = rest3.Slice(idx4 + 1).Trim();

        if (int.TryParse(qtdSpan, out int quantidade) &&
            decimal.TryParse(precoSpan, out decimal preco) &&
            int.TryParse(prioridadeSpan, out int prioridade))
        {
            var item = new ItemPedidoDto(produtoIdSpan.ToString(), quantidade, preco);
            resultado.Add(new CriarPedidoRequest(new[] { item }, regiaoSpan.ToString(), prioridade));
        }
    }
}
