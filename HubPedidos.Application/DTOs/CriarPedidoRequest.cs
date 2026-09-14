namespace HubPedidos.Application.DTOs;

public record CriarPedidoRequest(IEnumerable<ItemPedidoDto> Itens, string Regiao, int Prioridade);
