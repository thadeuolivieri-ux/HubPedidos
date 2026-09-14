namespace HubPedidos.Api.Hubs;

using System.Threading.Tasks;
using HubPedidos.Application.Events;
using Microsoft.AspNetCore.SignalR;

public interface IPedidosClient
{
    Task ReceberAtualizacaoPedido(PedidoAtualizadoEvent evento);
}

public class PedidosHub : Hub<IPedidosClient>
{
    public async Task InscreverNoPedido(string pedidoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Pedido_{pedidoId}");
    }

    public async Task SairDoPedido(string pedidoId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Pedido_{pedidoId}");
    }
}
