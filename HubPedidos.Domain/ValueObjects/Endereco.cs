namespace HubPedidos.Domain.ValueObjects;

public record Endereco
{
    public string Logradouro { get; }
    public string Cidade { get; }
    public string Estado { get; }
    public string Cep { get; }

    public Endereco(string logradouro, string cidade, string estado, string cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro)) throw new ArgumentException("Logradouro obrigatório.", nameof(logradouro));
        if (string.IsNullOrWhiteSpace(cidade)) throw new ArgumentException("Cidade obrigatória.", nameof(cidade));
        if (string.IsNullOrWhiteSpace(estado)) throw new ArgumentException("Estado obrigatório.", nameof(estado));
        if (string.IsNullOrWhiteSpace(cep)) throw new ArgumentException("CEP obrigatório.", nameof(cep));

        Logradouro = logradouro;
        Cidade = cidade;
        Estado = estado;
        Cep = cep;
    }
}
