namespace FluxoCaixa.Lancamentos.API.Models;

/// <summary>
/// Requisição de login
/// </summary>
public record LoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
