namespace HubPedidos.Application.Interfaces;

using HubPedidos.Domain.Entities;

public interface IProcessadorPedido
{
    string Processar(Pedido pedido);
}
