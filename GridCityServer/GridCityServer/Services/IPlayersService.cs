using GridCityServer.Models;

namespace GridCityServer.Services;

public interface IPlayersService
{
    Task<Player?> CreatePlayerAccount(string username, string passwordHash);
    Task<Player?> GetPlayerAccount(Guid playerId);
    Task<Player?> GetPlayerAccountByUsername(string username);
}