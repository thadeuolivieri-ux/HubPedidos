namespace HubPedidos.Application.Mappers;

using HubPedidos.Application.DTOs;
using HubPedidos.Domain.Entities;
using HubPedidos.Domain.ValueObjects;

public static class PedidoMapper
{
    public static Pedido ToDomain(CriarPedidoRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "A requisição de criação de pedido não pode ser nula.");

        var itensDominio = request.Itens?.Select(i => new ItemPedido(i.ProdutoId, i.Quantidade, i.PrecoUnitario))
                           ?? Enumerable.Empty<ItemPedido>();

        return new Pedido(PedidoId.New(), itensDominio, request.Regiao, request.Prioridade);
    }

    public static PedidoResponse ToResponse(Pedido pedido)
    {
        if (pedido == null)
            throw new ArgumentNullException(nameof(pedido), "O objeto de domínio Pedido não pode ser nulo.");

        var itensDto = pedido.Itens.Select(i => new ItemPedidoDto(i.ProdutoId, i.Quantidade, i.PrecoUnitario));

        return new PedidoResponse(
            pedido.Id.Value,
            itensDto,
            pedido.Regiao,
            pedido.Prioridade,
            pedido.ValorTotal,
            pedido.QuantidadeTotalItens
        );
    }
}
