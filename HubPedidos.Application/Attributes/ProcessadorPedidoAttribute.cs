namespace HubPedidos.Application.Attributes;

using HubPedidos.Domain.Enums;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ProcessadorPedidoAttribute : Attribute
{
    public CategoriaProcessamento Categoria { get; }
    public string Versao { get; }

    public ProcessadorPedidoAttribute(CategoriaProcessamento categoria, string versao = "1.0")
    {
        Categoria = categoria;
        Versao = versao;
    }
}
