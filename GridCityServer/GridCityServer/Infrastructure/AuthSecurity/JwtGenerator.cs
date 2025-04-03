using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GridCityServer.Infrastructure.AuthSecurity;

public class JwtGenerator : IJwtGenerator
{
    public const string ConfigurationSection = "JWT";

    private string _secretKey;
    private string _issuer;
    private string _audience;

    public JwtGenerator(IConfiguration configuration)
    {
        _secretKey = configuration[$"{ConfigurationSection}:SecretKey"] ?? throw new ArgumentNullException(nameof(configuration));
        _issuer = configuration[$"{ConfigurationSection}:Issuer"] ?? throw new ArgumentNullException(nameof(configuration));
        _audience = configuration[$"{ConfigurationSection}:Audience"] ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GenerateJwtToken(Guid userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
