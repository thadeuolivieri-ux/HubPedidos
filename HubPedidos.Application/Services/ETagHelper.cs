namespace HubPedidos.Application.Services;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public static class ETagHelper
{
    public static string GerarETag<T>(T objeto)
    {
        var json = JsonSerializer.Serialize(objeto);
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = SHA256.HashData(bytes);
        return $"\"{Convert.ToHexString(hash)[..16]}\"";
    }

    public static bool ETagValida(string? ifNoneMatchHeader, string etagAtual)
    {
        if (string.IsNullOrWhiteSpace(ifNoneMatchHeader))
            return false;

        return string.Equals(ifNoneMatchHeader.Trim(), etagAtual, StringComparison.Ordinal);
    }
}
