namespace HubPedidos.Application.Jobs;

public record ProcessarPedidoJob(
    Guid PedidoId,
    string Regiao,
    int Prioridade,
    decimal ValorTotal,
    DateTime CriadoEm
);
