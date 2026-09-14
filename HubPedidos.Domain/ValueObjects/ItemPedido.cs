namespace HubPedidos.Domain.ValueObjects;

public record ItemPedido
{
    public string ProdutoId { get; }
    public int Quantidade { get; }
    public decimal PrecoUnitario { get; }

    public ItemPedido(string produtoId, int quantidade, decimal precoUnitario)
    {
        if (string.IsNullOrWhiteSpace(produtoId))
            throw new ArgumentException("O ID do produto é obrigatório.", nameof(produtoId));
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
        if (precoUnitario <= 0)
            throw new ArgumentException("O preço unitário deve ser maior que zero.", nameof(precoUnitario));

        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public decimal ValorTotal => Quantidade * PrecoUnitario;
}
