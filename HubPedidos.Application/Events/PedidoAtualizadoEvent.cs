namespace HubPedidos.Application.Events;

public record PedidoAtualizadoEvent(
    Guid PedidoId,
    string NovoStatus,
    decimal ValorTotal,
    DateTime AtualizadoEm
);
