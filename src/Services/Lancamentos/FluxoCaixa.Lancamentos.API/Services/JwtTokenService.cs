using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FluxoCaixa.Lancamentos.API.Services;

/// <summary>
/// Serviço para geração e validação de tokens JWT
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username, string[] roles)
    {
    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
   {
    new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Sub, username),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
    };

        // Adicionar roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!);

        var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"],
       audience: _configuration["Jwt:Audience"],
            claims: claims,
       expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
   );

   return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
      try
        {
      var tokenHandler = new JwtSecurityTokenHandler();
   var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
       {
  ValidateIssuer = true,
    ValidateAudience = true,
      ValidateLifetime = true,
     ValidateIssuerSigningKey = true,
         ValidIssuer = _configuration["Jwt:Issuer"],
       ValidAudience = _configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(key),
           ClockSkew = TimeSpan.Zero
          };

      return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
  catch
        {
          return null;
        }
    }
}
