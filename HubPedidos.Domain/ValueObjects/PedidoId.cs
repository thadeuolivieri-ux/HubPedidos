namespace HubPedidos.Domain.ValueObjects;

public readonly record struct PedidoId
{
    public Guid Value { get; }

    public PedidoId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("O identificador do pedido não pode ser um Guid vazio.", nameof(value));
        Value = value;
    }

    public static PedidoId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
