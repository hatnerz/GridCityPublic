using GridCityServer.Dtos;
using GridCityServer.Models;

namespace GridCityServer.Services;

public interface ILobbiesService
{
    Task<Lobby?> CreateLobbyAsync(string lobbyName, Guid createdPlayerId, (int x, int y) mapSize, int deckSize);
    Task<List<LobbyItemDto>> GetAllLobbiesAsync();
    Task<LobbyDetailsDto?> GetLobbyDetailsAsync(Guid lobbyId);
    Task<List<LobbyPlayerDto>> GetAllLobbyJoinedPlayers();
    Task<List<Guid>?> GetLobbyPlayers(Guid lobbyId);
    Task<bool> JoinLobbyAsync(Guid lobbyId, Guid playerId);
    Task<bool> LeaveLobbyAsync(Guid lobbyId, Guid playerId);
    Task<bool> RemoveLobbyAsync(Guid lobbyId);
    Task<bool> StartGameAsync(Guid lobbyId);
}