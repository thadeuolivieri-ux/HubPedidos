namespace HubPedidos.Domain.Entities;

using HubPedidos.Domain.ValueObjects;

public class Pedido
{
    public PedidoId Id { get; private set; }
    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();
    public string Regiao { get; private set; }
    public int Prioridade { get; private set; }
    public Endereco? EnderecoEntrega { get; private set; }

    // Construtor sem parâmetros para EF Core
    private Pedido() 
    { 
        Regiao = string.Empty;
    }

    public Pedido(PedidoId id, IEnumerable<ItemPedido> itens, string regiao, int prioridade, Endereco? enderecoEntrega = null)
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
        EnderecoEntrega = enderecoEntrega;
    }

    public decimal ValorTotal => _itens.Sum(i => i.ValorTotal);
    public int QuantidadeTotalItens => _itens.Sum(i => i.Quantidade);
}
