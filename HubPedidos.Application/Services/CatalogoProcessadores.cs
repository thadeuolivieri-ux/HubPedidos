namespace HubPedidos.Application.Services;

using System.Reflection;
using HubPedidos.Application.Attributes;
using HubPedidos.Application.Interfaces;
using HubPedidos.Domain.Enums;

public class CatalogoProcessadores
{
    private readonly Dictionary<CategoriaProcessamento, IProcessadorPedido> _processadores = new();

    public CatalogoProcessadores()
    {
        CarregarProcessadores();
    }

    private void CarregarProcessadores()
    {
        var assembly = typeof(IProcessadorPedido).Assembly;

        var tipos = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IProcessadorPedido).IsAssignableFrom(t));

        foreach (var tipo in tipos)
        {
            var attr = tipo.GetCustomAttribute<ProcessadorPedidoAttribute>();
            if (attr == null) continue;

            if (_processadores.ContainsKey(attr.Categoria))
            {
                throw new InvalidOperationException($"Duplicidade de processador detectada para a categoria {attr.Categoria}.");
            }

            var instancia = Activator.CreateInstance(tipo) as IProcessadorPedido
                ?? throw new InvalidOperationException($"Não foi possível instanciar o tipo {tipo.FullName}.");

            _processadores.Add(attr.Categoria, instancia);
        }
    }

    public IProcessadorPedido ObterProcessador(CategoriaProcessamento categoria)
    {
        if (!_processadores.TryGetValue(categoria, out var processador))
        {
            throw new KeyNotFoundException($"Nenhum processador configurado para a categoria {categoria}.");
        }
        return processador;
    }
}
