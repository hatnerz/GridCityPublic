namespace GridCityServer.Infrastructure.AuthSecurity;

public interface IJwtGenerator
{
    string GenerateJwtToken(Guid userId, string username);
}