using GridCityServer.Dtos;

namespace GridCityServer.Infrastructure;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(string username, string password);
    Task<AuthResultDto> RegisterAsync(string username, string password);
}