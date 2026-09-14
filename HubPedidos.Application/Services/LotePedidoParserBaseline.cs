namespace HubPedidos.Application.Services;

using HubPedidos.Application.DTOs;

public static class LotePedidoParserBaseline
{
    public static List<CriarPedidoRequest> ParseCsv(string conteudoCsv)
    {
        if (string.IsNullOrWhiteSpace(conteudoCsv))
            return new List<CriarPedidoRequest>();

        var linhas = conteudoCsv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var resultado = new List<CriarPedidoRequest>();

        foreach (var linha in linhas)
        {
            if (linha.StartsWith("ProdutoId", StringComparison.OrdinalIgnoreCase))
                continue; // Pula cabeçalho

            var colunas = linha.Split(',');
            if (colunas.Length < 5)
                continue;

            var produtoId = colunas[0].Trim();
            var quantidade = int.Parse(colunas[1].Trim());
            var preco = decimal.Parse(colunas[2].Trim());
            var regiao = colunas[3].Trim();
            var prioridade = int.Parse(colunas[4].Trim());

            var item = new ItemPedidoDto(produtoId, quantidade, preco);
            resultado.Add(new CriarPedidoRequest(new[] { item }, regiao, prioridade));
        }

        return resultado;
    }
}
