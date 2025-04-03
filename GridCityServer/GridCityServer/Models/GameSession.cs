using GridCity.GameLogic.Cards;
using GridCity.GameLogic.CellElements.BaseElements;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridCityServer.Models;

public class GameSession : Entity<Guid>
{
    public Guid LobbyId { get; set; }
    public bool Initialized { get; set; }

    [JsonIgnore]
    private PlacedBuilding?[,]? _gameGrid;
    [JsonInclude]
    public string _serializedGrid { get; set; } = string.Empty;
    public PlacedBuilding? GetPlacedBuilding(int x, int y)
    {
        _gameGrid = DeserializeGrid(_serializedGrid);
        return _gameGrid == null ? null : _gameGrid[x, y];
    }
    public void SetPlacedBuilding(int x, int y, PlacedBuilding placedBuilding)
    {
        if (_gameGrid == null)
            return;

        _gameGrid[x, y] = placedBuilding;
        _serializedGrid = SerializeGrid(_gameGrid);
    }
    [JsonIgnore]
    public PlacedBuilding?[,]? GameGridReadonly => DeserializeGrid(_serializedGrid);

    public int GridSizeX => _gameGrid?.GetLength(0) ?? 0;
    public int GridSizeY => _gameGrid?.GetLength(1) ?? 0;


    public List<PlayerStats> Players { get; set; }
    public List<CardType> InitialDeck { get; set; }
    public DateTime StartDate { get; set; }
    public Guid NextTurnPlayerId { get; set; }

    public GameSession(Guid lobbyId)
    {
        Id = Guid.NewGuid();
        LobbyId = lobbyId;
        Initialized = false;
    }

    public void Initialize(int gridSizeX, int gridSizeY, List<(Guid Id, string Color)> players, List<CardType> InitialDecks, DateTime startDate)
    {
        Initialized = true;
        InitialDeck = InitialDecks;

        _gameGrid = new PlacedBuilding?[gridSizeX, gridSizeY];
        _serializedGrid = SerializeGrid(_gameGrid);
        Players = players.Select(p => new PlayerStats(
            p.Id, p.Color, InitialDecks.ToList(), new())).ToList();
        StartDate = startDate;
        NextTurnPlayerId = players.FirstOrDefault().Id;
    }

    public bool PassMoveTurn()
    {
        int currentMovePlayerIndex = Players.FindIndex(p => p.PlayerId == NextTurnPlayerId);
        if (currentMovePlayerIndex == -1)
            return false;

        int nextIndex = (currentMovePlayerIndex + 1) % Players.Count;

        NextTurnPlayerId = Players[nextIndex].PlayerId;
        return true;
    }

    private static string SerializeGrid(PlacedBuilding?[,] grid)
    {
        if (grid == null) return string.Empty;

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        var jaggedArray = new PlacedBuilding?[rows][];

        for (int i = 0; i < rows; i++)
        {
            jaggedArray[i] = new PlacedBuilding?[cols];
            for (int j = 0; j < cols; j++)
            {
                jaggedArray[i][j] = grid[i, j];
            }
        }

        return JsonSerializer.Serialize(jaggedArray);
    }

    private static PlacedBuilding?[,] DeserializeGrid(string json)
    {
        if (string.IsNullOrEmpty(json)) return new PlacedBuilding?[0, 0];

        var jaggedArray = JsonSerializer.Deserialize<PlacedBuilding?[][]>(json);
        if (jaggedArray == null || jaggedArray.Length == 0) return new PlacedBuilding?[0, 0];

        int rows = jaggedArray.Length;
        int cols = jaggedArray[0].Length;
        var grid = new PlacedBuilding?[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid[i, j] = jaggedArray[i][j];
            }
        }

        return grid;
    }
};

public record PlacedBuilding(
    Guid PlayerId,
    BuildingType Building);

public record PlayerStats
{
    public Guid PlayerId { get; private set; }
    public string Color { get; private set; }
    public int Score { get; set; }
    public List<CardType> CardsInDeck { get; private set; }
    public List<CardType> CardsInHand { get; private set; }
    public PlayerStats(Guid playerId, string color, List<CardType> cardsInDeck, List<CardType> cardsInHand)
    {
        PlayerId = playerId;
        Color = color;
        Score = 0;
        CardsInDeck = cardsInDeck;
        CardsInHand = cardsInHand;
    }
}