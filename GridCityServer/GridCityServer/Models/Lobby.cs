using System.Numerics;

namespace GridCityServer.Models;

public class Lobby : Entity<Guid>
{
    public string Name { get; set; }
    public int MaxPlayers { get; set; }
    public Guid CreatedPlayerId { get; set; }
    public List<Guid> Players { get; set; } = [];
    public bool GameStarted { get; set; }
    public DateTime CreatedAt { get; set; }
    public MapSize MapSize { get; set; }
    public int DeckPerPlayerSize { get; set; }

    public Lobby()
    {
    }

    public Lobby(string name, int maxPlayers, Guid createdPlayerId, MapSize mapSize, int deckSize)
    {
        Id = Guid.NewGuid();
        Name = name;
        MaxPlayers = maxPlayers;
        CreatedPlayerId = createdPlayerId;
        Players.Add(createdPlayerId);
        CreatedAt = DateTime.UtcNow;
        GameStarted = false;
        MapSize = mapSize;
        DeckPerPlayerSize = deckSize;
    }

    public bool StartGame()
    {
        if (/*Players.Count == 1||*/ GameStarted)
            return false;

        GameStarted = true;
        return true;
    }
}

public class MapSize
{
    public int X { get; set; }
    public int Y { get; set; }

    public MapSize(int x, int y)
    {
        X = x;
        Y = y;
    }
}