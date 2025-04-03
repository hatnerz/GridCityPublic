using GridCityServer.Database;
using GridCityServer.Dtos;
using GridCityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GridCityServer.Services;

public class LobbiesService : ILobbiesService
{
    private readonly int MaxPlayers = 2;
    private readonly ILobbiesRepository _lobbiesRepository;
    private readonly ApplicationDbContext _dbContext;

    public LobbiesService(ApplicationDbContext dbContext, ILobbiesRepository lobbiesRepository)
    {
        _dbContext = dbContext;
        _lobbiesRepository = lobbiesRepository;
    }

    public async Task<Lobby?> CreateLobbyAsync(string lobbyName, Guid createdPlayerId, (int x, int y) mapSize, int deckSize)
    {
        if (!IsValidSessionParameters(MaxPlayers, deckSize, mapSize))
            return null;

        var lobby = new Lobby(lobbyName, MaxPlayers, createdPlayerId, new MapSize(mapSize.x, mapSize.y), deckSize);
        await _lobbiesRepository.SaveLobbyAsync(lobby);
        return lobby;
    }

    public async Task<List<LobbyItemDto>> GetAllLobbiesAsync()
    {
        var players = await _dbContext.Players.ToListAsync();
        var lobbies = (await _lobbiesRepository.GetAllLobbiesAsync())
            .Select(e => new LobbyItemDto(
                e.Id,
                e.Name,
                e.MaxPlayers,
                e.Players.Count,
                e.Players.Count < e.MaxPlayers && !e.GameStarted,
                e.CreatedPlayerId,
                players.FirstOrDefault(players => players.Id == e.CreatedPlayerId)?.Username ?? "",
                e.MapSize.X,
                e.MapSize.Y,
                e.DeckPerPlayerSize))
            .ToList();
        return lobbies;
    }

    public async Task<LobbyDetailsDto?> GetLobbyDetailsAsync(Guid lobbyId)
    {
        var lobby = await _lobbiesRepository.GetLobbyAsync(lobbyId);
        if (lobby == null)
            return null;

        var players = await _dbContext.Players.Where(e => lobby.Players.Contains(e.Id)).ToListAsync();
        return new LobbyDetailsDto(
            lobby.Id,
            lobby.Name,
            lobby.MaxPlayers,
            lobby.Players.Count,
            lobby.CreatedPlayerId,
            players.FirstOrDefault(players => players.Id == lobby.CreatedPlayerId)?.Username ?? "",
            lobby.MapSize.X,
            lobby.MapSize.Y,
            lobby.DeckPerPlayerSize,
            players.Select(e => e.Username).ToList());
    }

    public async Task<bool> RemoveLobbyAsync(Guid lobbyId)
    {
        return await _lobbiesRepository.DeleteLobbyAsync(lobbyId);
    }

    public async Task<bool> JoinLobbyAsync(Guid lobbyId, Guid playerId)
    {
        var playerExists = await _dbContext.Players.AnyAsync(e => e.Id == playerId);
        var lobby = await _lobbiesRepository.GetLobbyAsync(lobbyId);
        if (!playerExists || lobby == null)
            return false;

        lobby.Players.Add(playerId);
        await _lobbiesRepository.SaveLobbyAsync(lobby);
        return true;
    }

    public async Task<bool> LeaveLobbyAsync(Guid lobbyId, Guid playerId)
    {
        var lobby = await _lobbiesRepository.GetLobbyAsync(lobbyId);
        if (lobby == null || !lobby.Players.Contains(playerId))
            return false;

        var removeResult = lobby.Players.Remove(playerId);
        if (!removeResult)
            return false;

        await _lobbiesRepository.SaveLobbyAsync(lobby);
        return true;
    }

    public async Task<bool> StartGameAsync(Guid lobbyId)
    {
        var lobby = await _lobbiesRepository.GetLobbyAsync(lobbyId);
        if (lobby == null)
            return false;

        if (!lobby.StartGame())
            return false;

        await _lobbiesRepository.SaveLobbyAsync(lobby);
        return true;
    }

    private bool IsValidSessionParameters(int playersCount, int deckSize, (int x, int y) mapSize)
    {
        if (playersCount > MaxPlayers)
            return false;

        return playersCount * deckSize <= mapSize.x * mapSize.y;
    }

    public async Task<List<LobbyPlayerDto>> GetAllLobbyJoinedPlayers()
    {
        var lobbies = await _lobbiesRepository.GetAllLobbiesAsync();
        var players = lobbies
            .SelectMany(lobby => lobby.Players.Select(playerId => new LobbyPlayerDto(playerId, lobby.Id)))
            .ToList();

        return players;
    }

    public async Task<List<Guid>?> GetLobbyPlayers(Guid lobbyId)
    {
        var lobby = await _lobbiesRepository.GetLobbyAsync(lobbyId);
        if (lobby == null)
            return null;

        return lobby.Players;
    }
}
