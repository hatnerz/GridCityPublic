using GridCityServer.Models;

namespace GridCityServer.Database;

public interface ILobbiesRepository
{
    Task<bool> DeleteLobbyAsync(Guid lobbyId);
    Task<List<Lobby>> GetAllLobbiesAsync();
    Task<Lobby?> GetLobbyAsync(Guid lobbyId);
    Task SaveLobbyAsync(Lobby lobby);
}