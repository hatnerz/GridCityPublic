using GridCity.GameLogic.Cards;
using GridCity.GameLogic.CellElements.BaseElements;
using GridCityServer.Database;
using GridCityServer.Dtos;
using GridCityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GridCityServer.Services;

public class GameplayService : IGameplayService
{
    private readonly List<string> _playerColors = ["FF0000", "#0000FF"];
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly ILobbiesRepository _lobbiesRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly Random _random;
    private readonly int _maxCardsInHand = 3;

    public GameplayService(IGameSessionRepository gameSessionRepository, ILobbiesRepository lobbiesRepository, ApplicationDbContext applicationDbContext)
    {
        _gameSessionRepository = gameSessionRepository;
        _lobbiesRepository = lobbiesRepository;
        _dbContext = applicationDbContext;
        _random = new Random();
    }

    public async Task<Guid> CreateEmptyGameSessionAsync(Guid lobbyId)
    {
        var gameSession = new GameSession(lobbyId);
        await _gameSessionRepository.SaveGameSessionAsync(gameSession);
        return gameSession.Id;
    }

    public async Task<GameSessionInitialDto?> InitializeGameSessionAsync(Guid gameSessionId)
    {
        var gameSession = await _gameSessionRepository.GetGameSessionAsync(gameSessionId);
        if (gameSession == null)
            return null;

        var lobby = await _lobbiesRepository.GetLobbyAsync(gameSession.LobbyId);
        if (lobby == null)
            return null;

        var players = await _dbContext.Players.Where(u => lobby.Players.Contains(u.Id)).ToListAsync();
        if (players.Count != lobby.Players.Count)
            return null;

        var initialDeck = GenerateDeck(lobby.MapSize.X, lobby.MapSize.Y, lobby.Players.Count, lobby.DeckPerPlayerSize);

        var dateNow = DateTime.UtcNow;

        gameSession.Initialize(
            lobby.MapSize.X,
            lobby.MapSize.Y,
            lobby.Players.Select(e => (e, _playerColors[lobby.Players.IndexOf(e) % _playerColors.Count])).ToList(),
            initialDeck,
            dateNow);

        gameSession.Players.ForEach(p => { for (int i = 0; i < _maxCardsInHand; i++) TakeCard(p); });

        await _gameSessionRepository.SaveGameSessionAsync(gameSession);
        return new GameSessionInitialDto(
            gameSession.Id,
            lobby.MapSize.X,
            lobby.MapSize.Y,
            gameSession.Players.Select(e => new PlayerDto(
                e.PlayerId,
                players.FirstOrDefault(p => p.Id == e.PlayerId)?.Username ?? "",
                e.Color)).ToList(),
            gameSession.NextTurnPlayerId,
            gameSession.InitialDeck.Count);
    }

    public async Task<MoveResultDto?> MakeMoveAsync(GameSessionMoveDto moveDto)
    {
        var gameSession = await _gameSessionRepository.GetGameSessionAsync(moveDto.GameSessionId);
        if (gameSession == null)
            return null;

        var isPlayersTurn = gameSession.NextTurnPlayerId == moveDto.PlayerId;
        if (!isPlayersTurn)
            return null;

        var playerStats = gameSession.Players.FirstOrDefault(e => e.PlayerId == moveDto.PlayerId);
        if (playerStats == null)
            return null;

        var hasPlayerCardInHand = playerStats.CardsInHand.Any(e => e == moveDto.Card);
        if (!hasPlayerCardInHand)
            return null;

        var isGridCellEmpty = gameSession.GetPlacedBuilding(moveDto.GridPositionX, moveDto.GridPositionY) == null;
        if (!isGridCellEmpty)
            return null;

        var playerCard = CardFactory.CreateCard(moveDto.Card) as BuildingCard;
        if (playerCard == null)
            return null;

        gameSession.SetPlacedBuilding(moveDto.GridPositionX, moveDto.GridPositionY, new PlacedBuilding(
            moveDto.PlayerId,
            playerCard.Building.BuildingType));

        playerStats.CardsInHand.Remove(moveDto.Card);
        TakeCard(playerStats);
        RecalculatePlayersScore(gameSession);
        gameSession.PassMoveTurn();

        await _gameSessionRepository.SaveGameSessionAsync(gameSession);

        return new MoveResultDto(
            moveDto.PlayerId,
            gameSession.NextTurnPlayerId,
            playerCard.Building.BuildingType,
            moveDto.GridPositionX,
            moveDto.GridPositionY);
    }

    public async Task<List<PlayerScoreDto>> GetPlayersScoreAsync(Guid gameSessionId)
    {
        var gameSession = await _gameSessionRepository.GetGameSessionAsync(gameSessionId);
        if (gameSession == null)
            return [];

        return gameSession.Players.Select(e => new PlayerScoreDto(
            e.PlayerId,
            e.Score)).ToList();
    }

    public async Task<bool?> IsNoCardsLeftAsync(Guid gameSessionId)
    {
        var gameSession = await _gameSessionRepository.GetGameSessionAsync(gameSessionId);
        if (gameSession == null)
            return null;

        return gameSession.Players.All(e => e.CardsInDeck.Count == 0 && e.CardsInHand.Count == 0);
    }

    public async Task<PlayerCardsDto?> GetPlayerActualCardsAsync(Guid gameSessionId, Guid playerId)
    {
        var gameSession = await _gameSessionRepository.GetGameSessionAsync(gameSessionId);
        if (gameSession == null)
            return null;

        var playerStats = gameSession.Players.FirstOrDefault(e => e.PlayerId == playerId);
        if (playerStats == null)
            return null;

        return new PlayerCardsDto(
            playerStats.CardsInDeck.Count,
            playerStats.CardsInHand.ToList());
    }

    public async Task<GameSessionFinalResultDto?> EndGameSessionAsync(Guid gameSessionId)
    {
        var gameSession = await _gameSessionRepository.GetGameSessionAsync(gameSessionId);
        if (gameSession == null)
            return null;

        if (gameSession.Players.Any(e => e.CardsInDeck.Count != 0))
            return null;

        var duration = DateTime.UtcNow - gameSession.StartDate;
        var winner = gameSession.Players.MaxBy(e => e.Score);
        if (winner == null)
            return null;

        var gameSessionResult = new GameSessionFinalResultDto(
            gameSession.Players.Select(e => new PlayerScoreDto(
                e.PlayerId,
                e.Score)).ToList(),
            new PlayerScoreDto(
                winner.PlayerId,
                winner.Score),
            duration);

        await _lobbiesRepository.DeleteLobbyAsync(gameSession.LobbyId);
        await _gameSessionRepository.DeleteGameSessionAsync(gameSessionId); // TODO: Add game session game stats saving
        return gameSessionResult;
    }

    public async Task<Guid?> GetGameSessionIdByLobbyIdAsync(Guid lobbyId)
    {
        var gameSessions = await _gameSessionRepository.GetGameSessionsAsync();
        return gameSessions.FirstOrDefault(e => e.LobbyId == lobbyId)?.Id;
    }

    private List<CardType> GenerateDeck(int gridSizeX, int gridSizeY, int playersCount, int proposedDeckPerPlayerSize) // TODO: Add custom deck size complicatfed logic
    {
        int totalCards = gridSizeX * gridSizeY;
        int deckSizePerPlayer = totalCards / playersCount;
        if (proposedDeckPerPlayerSize < deckSizePerPlayer && proposedDeckPerPlayerSize > 0)
            deckSizePerPlayer = proposedDeckPerPlayerSize;

        CardType[] allCards = Enum.GetValues<CardType>();

        return Enumerable.Range(0, deckSizePerPlayer)
            .Select(_ => allCards[_random.Next(allCards.Length)])
            .ToList();
    }

    private void RecalculatePlayersScore(GameSession gameSession)
    {
        var gridSizeX = gameSession.GridSizeX;
        int gridSizeY = gameSession.GridSizeY;
        var gameGrid = gameSession.GameGridReadonly;
        if (gameGrid == null)
            return;

        var gridElements = ToCellElemenets(gameGrid);
        var gridState = new GridState(gridElements);

        gameSession.Players.ForEach(e => e.Score = 0);
        var playersDictionary = gameSession.Players.ToDictionary(e => e.PlayerId);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                var placedBuilding = gameSession.GetPlacedBuilding(x, y);

                if (placedBuilding != null && gridState.GridElements[x, y] is Building building)
                {
                    var score = building.CalculateTotalBuildingScore(gridState);
                    playersDictionary[placedBuilding.PlayerId].Score += score;
                }
            }
        }
    }

    private bool TakeCard(PlayerStats player)
    {
        if (player.CardsInDeck.Count == 0)
            return false;

        var takeCardIndex = _random.Next() % player.CardsInDeck.Count;
        var nextCard = player.CardsInDeck[takeCardIndex];
        player.CardsInHand.Add(nextCard);
        player.CardsInDeck.Remove(nextCard);
        return true;
    }

    private CellElement[,] ToCellElemenets(PlacedBuilding?[,] gameGrid)
    {
        var gridSizeX = gameGrid.GetLength(0);
        var gridSizeY = gameGrid.GetLength(1);

        var cellElements = new CellElement[gridSizeX, gridSizeY];

       
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                var placedBuilding = gameGrid[x, y];
                if (placedBuilding != null)
                {
                    var building = BuildingFactory.CreateBuilding(placedBuilding.Building);
                    building.GridPosition = new(x, y);
                    cellElements[x, y] = building;
                }
            }
        }

        return cellElements;
    }
}
