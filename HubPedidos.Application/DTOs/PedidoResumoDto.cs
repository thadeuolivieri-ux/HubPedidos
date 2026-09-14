namespace HubPedidos.Application.DTOs;

public record PedidoResumoDto(
    Guid Id,
    string Regiao,
    int Prioridade,
    int QuantidadeTotalItens,
    decimal ValorTotal
);
