using FluxoCaixa.Lancamentos.API.Models;
using FluxoCaixa.Lancamentos.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.Lancamentos.API.Controllers;

/// <summary>
/// Controller para autenticação de usuários
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _tokenService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
      JwtTokenService tokenService,
      ILogger<AuthController> logger,
      IConfiguration configuration)
    {
        _tokenService = tokenService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Autentica usuário e retorna token JWT
    /// </summary>
    /// <remarks>
    /// Usuários de teste disponíveis:
    /// 
    /// - **admin** / Admin@123 (Roles: Admin, Operador)
    /// - **operador** / Oper@123 (Roles: Operador)
    /// - **viewer** / View@123 (Roles: Viewer)
    /// 
    /// </remarks>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT válido por 1 hora</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login: {Username}", request.Username);

        var (isValid, roles) = ValidateCredentials(request.Username, request.Password);

        if (!isValid)
        {
            _logger.LogWarning("Login falhou para usuário: {Username}", request.Username);
            return Unauthorized(new
            {
                error = "Unauthorized",
                message = "Usuário ou senha inválidos"
            });
        }

        var token = _tokenService.GenerateToken(request.Username, roles);
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        _logger.LogInformation("Login bem-sucedido: {Username} - Roles: {Roles}",
          request.Username, string.Join(", ", roles));

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = request.Username,
            Roles = roles
        });
    }

    /// <summary>
    /// Obtém informações do usuário autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;
        var roles = User.Claims
          .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
          .Select(c => c.Value)
          .ToArray();

        return Ok(new
        {
            username,
            roles,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    private (bool IsValid, string[] Roles) ValidateCredentials(string username, string password)
    {
        // Usuários hardcoded para demonstração
        return (username, password) switch
        {
            ("admin", "Admin@123") => (true, new[] {
        "Admin",
        "Operador"
      }),
            ("operador", "Oper@123") => (true, new[] {
        "Operador"
      }),
            ("viewer", "View@123") => (true, new[] {
        "Viewer"
      }),
            _ => (false, Array.Empty<string>())
        };
    }
}