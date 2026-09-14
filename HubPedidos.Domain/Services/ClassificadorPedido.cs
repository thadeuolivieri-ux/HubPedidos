namespace HubPedidos.Domain.Services;

using HubPedidos.Domain.Entities;
using HubPedidos.Domain.Enums;

public static class ClassificadorPedido
{
    public static CategoriaProcessamento Classificar(Pedido pedido) => pedido switch
    {
        null => CategoriaProcessamento.Invalido,
        { QuantidadeTotalItens: <= 0 } or { ValorTotal: <= 0 } => CategoriaProcessamento.Invalido,
        { QuantidadeTotalItens: >= 50 } => CategoriaProcessamento.Atacado,
        { ValorTotal: >= 1000m } or { Prioridade: >= 5 } => CategoriaProcessamento.VIP,
        { Regiao: "SP" or "Sudeste" } or { Prioridade: >= 3 } => CategoriaProcessamento.Express,
        _ => CategoriaProcessamento.Padrao
    };
}
