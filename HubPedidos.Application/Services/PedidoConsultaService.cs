namespace HubPedidos.Application.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HubPedidos.Application.Data;
using HubPedidos.Application.DTOs;
using HubPedidos.Domain.Entities;
using HubPedidos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public class PedidoConsultaService
{
    private readonly HubPedidosDbContext _dbContext;

    // Consulta Compilada para busca recorrente por ID sem overhead de parsing LINQ
    private static readonly Func<HubPedidosDbContext, Guid, Task<PedidoResumoDto?>> ConsultaCompiladaPorId =
        EF.CompileAsyncQuery((HubPedidosDbContext db, Guid id) =>
            db.Pedidos
                .AsNoTracking()
                .Where(p => p.Id == new PedidoId(id))
                .Select(p => new PedidoResumoDto(
                    p.Id.Value,
                    p.Regiao,
                    p.Prioridade,
                    p.Itens.Sum(i => i.Quantidade),
                    p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)
                ))
                .FirstOrDefault());

    public PedidoConsultaService(HubPedidosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultadoPaginado<PedidoResumoDto>> ListarPaginadoAsync(int pagina = 1, int tamanhoPagina = 10, string? regiao = null)
    {
        if (pagina < 1) pagina = 1;
        if (tamanhoPagina < 1) tamanhoPagina = 10;

        IQueryable<Pedido> query = _dbContext.Pedidos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(regiao))
        {
            query = query.Where(p => p.Regiao == regiao);
        }

        int totalRegistros = await query.CountAsync();

        var itens = await query
            .OrderByDescending(p => p.Prioridade)
            .ThenBy(p => p.Regiao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(p => new PedidoResumoDto(
                p.Id.Value,
                p.Regiao,
                p.Prioridade,
                p.Itens.Sum(i => i.Quantidade),
                p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)
            ))
            .ToListAsync();

        return new ResultadoPaginado<PedidoResumoDto>(itens, pagina, tamanhoPagina, totalRegistros);
    }

    public Task<PedidoResumoDto?> ObterResumoCompiladoAsync(Guid id)
    {
        return ConsultaCompiladaPorId(_dbContext, id);
    }
}
