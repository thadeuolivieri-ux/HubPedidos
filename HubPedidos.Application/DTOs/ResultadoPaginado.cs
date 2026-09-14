namespace HubPedidos.Application.DTOs;

public record ResultadoPaginado<T>(
    IReadOnlyCollection<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalRegistros
);
