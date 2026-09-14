namespace HubPedidos.Application.Processadores;

using HubPedidos.Application.Attributes;
using HubPedidos.Application.Interfaces;
using HubPedidos.Domain.Entities;
using HubPedidos.Domain.Enums;

[ProcessadorPedido(CategoriaProcessamento.Padrao, "1.0")]
public class ProcessadorPadrao : IProcessadorPedido
{
    public string Processar(Pedido pedido) => $"Pedido {pedido.Id} processado via regra Padrão.";
}
