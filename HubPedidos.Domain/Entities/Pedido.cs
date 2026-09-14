namespace HubPedidos.Domain.Entities;

using HubPedidos.Domain.ValueObjects;

public class Pedido
{
    public PedidoId Id { get; }
    private readonly List<ItemPedido> _itens;
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();
    public string Regiao { get; }
    public int Prioridade { get; }

    public Pedido(PedidoId id, IEnumerable<ItemPedido> itens, string regiao, int prioridade)
    {
        var listaItens = itens?.ToList() ?? new List<ItemPedido>();

        if (listaItens.Count == 0)
            throw new ArgumentException("O pedido deve conter pelo menos um item válido.", nameof(itens));
        if (string.IsNullOrWhiteSpace(regiao))
            throw new ArgumentException("A região de entrega é obrigatória.", nameof(regiao));
        if (prioridade < 0)
            throw new ArgumentException("A prioridade não pode ser negativa.", nameof(prioridade));

        Id = id;
        _itens = listaItens;
        Regiao = regiao;
        Prioridade = prioridade;
    }

    public decimal ValorTotal => _itens.Sum(i => i.ValorTotal);
    public int QuantidadeTotalItens => _itens.Sum(i => i.Quantidade);
}
