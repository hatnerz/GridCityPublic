using GridCity.GameLogic.Cards;
using GridCityServer.Dtos;
using GridCityServer.Infrastructure;
using GridCityServer.Models;
using GridCityServer.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GridCityServer.Hubs;

public class GameplayHub : Hub
{
    private static ConcurrentDictionary<string, GameSessionPlayerDto> _userConnections = new ConcurrentDictionary<string, GameSessionPlayerDto>(); // string - connection id
    private readonly IGameplayService _gameplayService;
    private readonly ILobbiesService _lobbiesService;
    private readonly IMatchHistoryService _matchHistoryService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GameplayHub(IGameplayService gameplayService, ILobbiesService lobbiesService, IMatchHistoryService matchHistoryService, ICurrentUserProvider currentUserProvider)
    {
        _gameplayService = gameplayService;
        _lobbiesService = lobbiesService;
        _matchHistoryService = matchHistoryService;
        _currentUserProvider = currentUserProvider;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        Console.WriteLine("Connection handling");
        var playerId = _currentUserProvider.GetCurrentUser()?.PlayerId;
        if (playerId == null)
            return;

        if (_userConnections.Values.Any(x => x.PlayerId == playerId))
        {
            Console.WriteLine("Aborted because of connected again");
            Context.Abort();
            return;
        }

        var players = await _lobbiesService.GetAllLobbyJoinedPlayers();
        Console.WriteLine($"Player: {playerId}; Lobby players: {string.Join(" ", players.Select(e => e.PlayerId))}");
        var joinedPlayerLobby = players.FirstOrDefault(x => x.PlayerId == playerId);
        if (joinedPlayerLobby == null)
        {
            Console.WriteLine($"Total lobbies players: {players.Count}");
            Console.WriteLine("Aborted because of not joined lobby");
            Context.Abort();
            return;
        }

        var gameSessionId = await _gameplayService.GetGameSessionIdByLobbyIdAsync(joinedPlayerLobby.LobbyId);
        if (gameSessionId == null)
        {
            Console.WriteLine("Aborted because of invalid game lobby/session");
            Context.Abort();
            return;
        }

        await AddToGroupAsync(new GameSessionPlayerDto(gameSessionId.Value, joinedPlayerLobby.PlayerId), Context.ConnectionId);
        await StartGameIfPlayersConnected(gameSessionId.Value, joinedPlayerLobby.LobbyId);
        Console.WriteLine("Completed connection");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        _userConnections.Remove(Context.ConnectionId, out _);
    }

    public async Task MakeMove(GameSessionMoveRequestDto moveDto)
    {
        var playerId = _currentUserProvider.GetCurrentUser()?.PlayerId;
        if (playerId == null)
            return;

        var playerConnection = _userConnections.GetValueOrDefault(Context.ConnectionId);
        if (playerConnection == null)
            return;

        var gameSessionId = playerConnection.GameSessionId;
        var moveResult = await _gameplayService.MakeMoveAsync(new GameSessionMoveDto(
            playerId.Value,
            playerConnection.GameSessionId,
            moveDto.GridPositionX,
            moveDto.GridPositionY,
            moveDto.PlayedCard));

        if (moveResult == null)
            return;

        var playerCardsInHand = await _gameplayService.GetPlayerActualCardsAsync(playerConnection.GameSessionId, playerId.Value);
        await Clients.Caller.SendAsync(EventsNames.ActualCardsUpdated, playerCardsInHand);
        await Clients.Group(gameSessionId.ToString()).SendAsync(EventsNames.MoveMade, moveResult);

        var score = await _gameplayService.GetPlayersScoreAsync(gameSessionId);
        Console.WriteLine($"Sending score to player: {JsonSerializer.Serialize(score)}");
        await Clients.Group(gameSessionId.ToString()).SendAsync(EventsNames.ScoreUpdated, score);

        var isNoCardsLeft = await _gameplayService.IsNoCardsLeftAsync(gameSessionId) ?? false;
        if (isNoCardsLeft)
        {
            var endResult = await _gameplayService.EndGameSessionAsync(gameSessionId);
            if (endResult != null)
            {
                await _matchHistoryService.SaveMatchResultAsync(endResult);
            }

            await Clients.Group(gameSessionId.ToString()).SendAsync(EventsNames.GameEnded, endResult);
            await RemoveGameSessionGroupAsync(gameSessionId);
        }

    }

    private async Task StartGameIfPlayersConnected(Guid gameSessionId, Guid lobbyId)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();
        if (currentUser == null)
            return;

        Console.WriteLine("[GameStartTry]: Current user confirmed");
        var lobbyPlayers = await _lobbiesService.GetLobbyPlayers(lobbyId);
        if (lobbyPlayers == null)
            return;

        Console.WriteLine("[GameStartTry]: Lobby players confirmed");
        var lobbyConnectedPlayerIds = _userConnections.Values.Where(e => e.GameSessionId == gameSessionId).Select(e => e.PlayerId).ToList();

        Console.WriteLine($"Lobby connected players: {string.Join(" ", lobbyConnectedPlayerIds)}; All lobby players: {string.Join(" ", lobbyPlayers)}");
        if (!lobbyConnectedPlayerIds.OrderBy(id => id).SequenceEqual(lobbyPlayers.OrderBy(id => id)))
            return;

        Console.WriteLine("[GameStartTry]: All players connected confirmed");
        var gameSessionInitialData = await _gameplayService.InitializeGameSessionAsync(gameSessionId);
        if (gameSessionInitialData == null)
            return;

        Console.WriteLine("Game start confirmed");
        await Clients.Group(gameSessionId.ToString()).SendAsync(EventsNames.GameStartConfirmed, gameSessionInitialData);

        var lobbyConnections = _userConnections.Keys.Where(e => _userConnections[e].GameSessionId == gameSessionId).ToList();
        foreach(var connection in lobbyConnections)
        {
            var playerCards = await _gameplayService.GetPlayerActualCardsAsync(_userConnections[connection].GameSessionId, _userConnections[connection].PlayerId);
            await Clients.Client(connection).SendAsync(EventsNames.ActualCardsUpdated, playerCards);
        }
    }

    private async Task AddToGroupAsync(GameSessionPlayerDto gameSesionPlayer, string connectionId)
    {
        _userConnections[connectionId] = gameSesionPlayer;
        await Groups.AddToGroupAsync(connectionId, gameSesionPlayer.GameSessionId.ToString());
    }

    private async Task RemoveGameSessionGroupAsync(Guid gameSessionId)
    {
        foreach(var connection in _userConnections.Keys)
        {
            if (_userConnections[connection].GameSessionId == gameSessionId)
            {
                await Groups.RemoveFromGroupAsync(connection, gameSessionId.ToString());
                _userConnections.Remove(connection, out _);
            }
        }
    }
}
