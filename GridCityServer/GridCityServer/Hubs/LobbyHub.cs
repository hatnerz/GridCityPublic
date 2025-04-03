using GridCityServer.Dtos;
using GridCityServer.Infrastructure;
using GridCityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace GridCityServer.Hubs;


[Authorize]
public class LobbyHub : Hub
{
    private readonly ConcurrentDictionary<string, LobbyPlayerDto> _userLobbyConnections = new ConcurrentDictionary<string, LobbyPlayerDto>(); // string - connection id
    private readonly ILobbiesService _lobbiesService;
    private readonly IGameplayService _gameplayService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public LobbyHub(ILobbiesService lobbiesService, IGameplayService gameplayService, ICurrentUserProvider currentUserProvider)
    {
        _lobbiesService = lobbiesService;
        _gameplayService = gameplayService;
        _currentUserProvider = currentUserProvider;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var playerId = _currentUserProvider.GetCurrentUser()?.PlayerId;
        if (playerId == null)
            return;

        if (_userLobbyConnections.Values.Any(x => x.PlayerId == playerId))
        {
            Context.Abort();
            return;
        }

        var players = await _lobbiesService.GetAllLobbyJoinedPlayers();
        var joinedPlayer = players.FirstOrDefault(x => x.PlayerId == playerId);
        if (joinedPlayer != null)
        {
            await AddToLobbyGroupAsync(new LobbyPlayerDto(joinedPlayer.PlayerId, joinedPlayer.LobbyId), Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        var playerId = _currentUserProvider.GetCurrentUser()?.PlayerId;
        if (playerId == null)
            return;

        _userLobbyConnections.Remove(Context.ConnectionId, out _);
        // TODO: Add disconnection after some time period logic
        /*if (_userLobbyConnections.Values.Any(x => x.PlayerId == playerId))
        {

        }*/
    }

    public async Task CreateLobby(CreateLobbyDto createDto)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();
        if (currentUser == null)
            return;

        var lobby = await _lobbiesService.CreateLobbyAsync(
            createDto.LobbyName,
            currentUser.PlayerId,
            (createDto.MapSizeX, createDto.MapSizeY),
            createDto.DeckSize);

        if (lobby == null)
            return;

        await AddToLobbyGroupAsync(new LobbyPlayerDto(currentUser.PlayerId, lobby.Id), Context.ConnectionId);
        await SendLobbyUpdate(lobby.Id, EventsNames.MyLobbyCreated);
        await SendLobbiesUpdates();
    }

    public async Task RemoveLobby(Guid lobbyId)
    {
        var result = await _lobbiesService.RemoveLobbyAsync(lobbyId);
        if (!result)
            return;

        await SendLobbyUpdate(lobbyId, EventsNames.LobbyRemoved);
        await RemoveFromLobbyGroupAsync(lobbyId, Context.ConnectionId);
        await SendLobbiesUpdates();
    }

    public async Task GetLobbies()
    {
        var user = Context.User;
        var userName = user?.Identity?.Name;
        Console.WriteLine($"CurrentUser {userName}");
        var lobbies = await _lobbiesService.GetAllLobbiesAsync();
        await Clients.Caller.SendAsync(EventsNames.LobbiesUpdated, lobbies);
    }

    public async Task JoinLobby(Guid lobbyId)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();
        if (currentUser == null)
            return;

        var result = await _lobbiesService.JoinLobbyAsync(lobbyId, currentUser.PlayerId);
        if (!result)
            return;

        await AddToLobbyGroupAsync(new LobbyPlayerDto(currentUser.PlayerId, lobbyId), Context.ConnectionId);
        await SendLobbyUpdate(lobbyId, EventsNames.PlayerJoined);
        await SendLobbiesUpdates();
    }

    public async Task LeaveLobby(Guid lobbyId)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();
        if (currentUser == null)
            return;

        var result = await _lobbiesService.LeaveLobbyAsync(lobbyId, currentUser.PlayerId);
        if (!result)
            return;

        await SendLobbyUpdate(lobbyId, EventsNames.PlayerLeft);
        await RemoveFromLobbyGroupAsync(lobbyId, Context.ConnectionId);
        await SendLobbiesUpdates();
    }

    public async Task StartGame(Guid lobbyId)
    {
        var result = await _lobbiesService.StartGameAsync(lobbyId);
        if (!result)
            return;

        await _gameplayService.CreateEmptyGameSessionAsync(lobbyId);

        await SendLobbyUpdate(lobbyId, EventsNames.GameStarted);
        await SendLobbiesUpdates();
    }

    private async Task AddToLobbyGroupAsync(LobbyPlayerDto lobbyPlayerDto, string connectionId)
    {
        _userLobbyConnections[connectionId] = lobbyPlayerDto;
        await Groups.AddToGroupAsync(connectionId, lobbyPlayerDto.LobbyId.ToString());
    }

    private async Task RemoveFromLobbyGroupAsync(Guid lobbyId, string? connectionId)
    {
        if (connectionId != null)
        {
            _userLobbyConnections.Remove(connectionId, out LobbyPlayerDto? _);
            await Groups.RemoveFromGroupAsync(connectionId, lobbyId.ToString());
        }
        else
        {
            var connections = _userLobbyConnections.Where(x => x.Value.LobbyId == lobbyId).Select(x => x.Key).ToList();
            foreach (var connection in connections)
            {
                _userLobbyConnections.Remove(connection, out LobbyPlayerDto? _);
                await Groups.RemoveFromGroupAsync(connection, lobbyId.ToString());
            }
        }
    }

    private async Task SendLobbiesUpdates(List<LobbyItemDto>? lobbies = null)
    {
        if (lobbies == null)
            lobbies = await _lobbiesService.GetAllLobbiesAsync();

        var usersInLobby = _userLobbyConnections.Keys.ToList();

        await Clients.AllExcept(usersInLobby).SendAsync(EventsNames.LobbiesUpdated, lobbies);
    }

    private async Task SendLobbyUpdate(Guid lobbyId, string eventName = EventsNames.PlayerJoined)
    {
        var lobby = await _lobbiesService.GetLobbyDetailsAsync(lobbyId);
        await Clients.Group(lobbyId.ToString()).SendAsync(eventName, lobby);
    }
}