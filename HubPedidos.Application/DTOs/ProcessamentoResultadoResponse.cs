namespace HubPedidos.Application.DTOs;

public record ProcessamentoResultadoResponse(
    PedidoResponse Pedido,
    string Categoria,
    string MensagemProcessamento
);
