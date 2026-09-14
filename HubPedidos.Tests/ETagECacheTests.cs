namespace HubPedidos.Tests;

using System;
using HubPedidos.Application.DTOs;
using HubPedidos.Application.Services;
using Xunit;

public class ETagECacheTests
{
    [Fact]
    public void GerarETag_MesmoObjeto_DeveProduzirHashesIdenticos()
    {
        var id = Guid.NewGuid();
        var dto1 = new PedidoResumoDto(id, "SP", 3, 5, 250.0m);
        var dto2 = new PedidoResumoDto(id, "SP", 3, 5, 250.0m);

        string etag1 = ETagHelper.GerarETag(dto1);
        string etag2 = ETagHelper.GerarETag(dto2);

        Assert.Equal(etag1, etag2);
    }

    [Fact]
    public void GerarETag_ObjetoModificado_DeveProduzirHashesDiferentes()
    {
        var id = Guid.NewGuid();
        var dtoOriginal = new PedidoResumoDto(id, "SP", 3, 5, 250.0m);
        var dtoModificado = new PedidoResumoDto(id, "SP", 3, 10, 500.0m);

        string etagOriginal = ETagHelper.GerarETag(dtoOriginal);
        string etagModificado = ETagHelper.GerarETag(dtoModificado);

        Assert.NotEqual(etagOriginal, etagModificado);
    }

    [Fact]
    public void ETagValida_QuandoIfNoneMatchIgual_DeveRetornarVerdadeiro()
    {
        var id = Guid.NewGuid();
        var dto = new PedidoResumoDto(id, "RJ", 1, 2, 100.0m);
        string etagAtual = ETagHelper.GerarETag(dto);

        bool valida = ETagHelper.ETagValida(etagAtual, etagAtual);

        Assert.True(valida);
    }

    [Fact]
    public void ETagValida_QuandoIfNoneMatchDiferente_DeveRetornarFalso()
    {
        string etagAtual = "\"1234567890ABCDEF\"";
        string headerObsoleto = "\"0000000000AAAAAA\"";

        bool valida = ETagHelper.ETagValida(headerObsoleto, etagAtual);

        Assert.False(valida);
    }
}
