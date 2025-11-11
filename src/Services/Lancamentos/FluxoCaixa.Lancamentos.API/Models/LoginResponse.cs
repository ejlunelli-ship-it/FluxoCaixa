namespace FluxoCaixa.Lancamentos.API.Models;

/// <summary>
/// Resposta do login com token JWT
/// </summary>
public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string Username { get; init; } = string.Empty;
    public string[] Roles { get; init; } = Array.Empty<string>();
}
