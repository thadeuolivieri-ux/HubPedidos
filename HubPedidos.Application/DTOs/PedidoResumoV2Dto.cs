namespace HubPedidos.Application.DTOs;

public record PedidoResumoV2Dto(
    Guid PedidoId,
    string RegiaoAtendimento,
    int NivelPrioridade,
    int TotalItens,
    decimal ValorSubtotal,
    decimal TaxaServico,
    decimal ValorTotalFinal,
    string StatusCicloVida
);
