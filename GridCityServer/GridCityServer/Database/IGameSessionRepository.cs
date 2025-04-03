using GridCityServer.Models;

namespace GridCityServer.Database;

public interface IGameSessionRepository
{
    Task<bool> DeleteGameSessionAsync(Guid gameSessionId);
    Task<GameSession?> GetGameSessionAsync(Guid gameSessionId);
    Task<List<GameSession>> GetGameSessionsAsync();
    Task SaveGameSessionAsync(GameSession gameSession);
}