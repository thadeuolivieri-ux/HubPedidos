namespace HubPedidos.Tests;

using HubPedidos.Application.Services;
using HubPedidos.Domain.Enums;

public class CatalogoProcessadoresTests
{
    [Theory]
    [InlineData(CategoriaProcessamento.Atacado)]
    [InlineData(CategoriaProcessamento.VIP)]
    [InlineData(CategoriaProcessamento.Padrao)]
    public void ObterProcessador_CategoriaExistente_DeveRetornarProcessadorValido(CategoriaProcessamento categoria)
    {
        var catalogo = new CatalogoProcessadores();

        var processador = catalogo.ObterProcessador(categoria);

        Assert.NotNull(processador);
    }
}
