namespace HubPedidos.Application.DTOs;

public record PedidoResponse(
    Guid Id,
    IEnumerable<ItemPedidoDto> Itens,
    string Regiao,
    int Prioridade,
    decimal ValorTotal,
    int QuantidadeTotalItens
);
