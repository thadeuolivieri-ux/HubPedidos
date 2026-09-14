namespace HubPedidos.Tests;

using System;
using System.Linq;
using HubPedidos.Application.DTOs;
using HubPedidos.Domain.Entities;
using Xunit;

public class ArquiteturaEVersioningTests
{
    [Fact]
    public void Domínio_NaoDeveDependerDeEFCoreOuApi()
    {
        var referencias = typeof(Pedido).Assembly.GetReferencedAssemblies();

        bool referenciaEFCore = referencias.Any(r => r.Name!.Contains("EntityFrameworkCore"));
        bool referenciaApi = referencias.Any(r => r.Name!.Contains("HubPedidos.Api"));

        Assert.False(referenciaEFCore, "O projeto Domain não deve depender do Entity Framework Core.");
        Assert.False(referenciaApi, "O projeto Domain não deve depender da camada Api.");
    }

    [Fact]
    public void ClienteV1_DevePermanecerCompativelComContratoAntigo()
    {
        var id = Guid.NewGuid();
        var resumoV1 = new PedidoResumoDto(id, "SP", 5, 2, 100.0m);

        Assert.Equal(id, resumoV1.Id);
        Assert.Equal("SP", resumoV1.Regiao);
        Assert.Equal(5, resumoV1.Prioridade);
        Assert.Equal(2, resumoV1.QuantidadeTotalItens);
        Assert.Equal(100.0m, resumoV1.ValorTotal);
    }

    [Fact]
    public void ContratoV2_DeveIncluirTaxaESubtotal()
    {
        var id = Guid.NewGuid();
        var resumoV2 = new PedidoResumoV2Dto(id, "SP", 5, 2, 100.0m, 5.0m, 105.0m, "PROCESSADO");

        Assert.Equal(100.0m, resumoV2.ValorSubtotal);
        Assert.Equal(5.0m, resumoV2.TaxaServico);
        Assert.Equal(105.0m, resumoV2.ValorTotalFinal);
        Assert.Equal("PROCESSADO", resumoV2.StatusCicloVida);
    }
}
