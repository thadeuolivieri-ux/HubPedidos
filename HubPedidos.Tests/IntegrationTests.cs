namespace HubPedidos.Tests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HubPedidos.Application.DTOs;
using HubPedidos.Application.Jobs;
using Xunit;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public IntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_ProcessarPedido_RetornaStatusOk()
    {
        var client = _factory.CreateClient();
        var itens = new List<ItemPedidoDto>
        {
            new ItemPedidoDto("PROD-001", 2, 50.0m)
        };
        var request = new CriarPedidoRequest(itens, "SP", 5);

        var response = await client.PostAsJsonAsync("/api/pedidos/processar", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ETagDemonstracao_Retorna200E304EmChamadaCondicional()
    {
        var client = _factory.CreateClient();

        var response1 = await client.GetAsync("/api/v1/pedidos/etag-demonstracao");
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        string etag = response1.Headers.ETag!.Tag;

        var request2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/pedidos/etag-demonstracao");
        request2.Headers.TryAddWithoutValidation("If-None-Match", etag);

        var response2 = await client.SendAsync(request2);

        Assert.Equal(HttpStatusCode.NotModified, response2.StatusCode);
    }

    [Fact]
    public async Task Post_FilaPedidos_Retorna202Accepted()
    {
        var client = _factory.CreateClient();
        var job = new ProcessarPedidoJob(Guid.NewGuid(), "RJ", 3, 100.0m, DateTime.UtcNow);

        var response = await client.PostAsJsonAsync("/api/fila/pedidos", job);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }
}
