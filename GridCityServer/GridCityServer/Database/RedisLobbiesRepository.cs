using GridCityServer.Models;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text.Json;

namespace GridCityServer.Database;

public class RedisLobbiesRepository : ILobbiesRepository
{
    private readonly IDatabase _database;
    private readonly TimeSpan _lobbyExpiration = TimeSpan.FromHours(6);
    private const string LobbyPrefix = "lobby:";

    public RedisLobbiesRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task SaveLobbyAsync(Lobby lobby)
    {
        string key = LobbyPrefix + lobby.Id;
        string json = JsonSerializer.Serialize(lobby);
        await _database.StringSetAsync(key, json, _lobbyExpiration);
    }

    public async Task<bool> DeleteLobbyAsync(Guid lobbyId)
    {
        string key = LobbyPrefix + lobbyId;
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<Lobby?> GetLobbyAsync(Guid lobbyId)
    {
        string key = LobbyPrefix + lobbyId;
        string? json = await _database.StringGetAsync(key);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Lobby>(json);
    }

    public async Task<List<Lobby>> GetAllLobbiesAsync()
    {
        var endpoints = _database.Multiplexer.GetEndPoints();
        var server = _database.Multiplexer.GetServer(endpoints[0]);

        var keys = server.Keys(pattern: LobbyPrefix + "*");
        var lobbies = new List<Lobby>();

        foreach (var key in keys)
        {
            string? json = await _database.StringGetAsync(key);
            if (!string.IsNullOrEmpty(json))
            {
                var lobby = JsonSerializer.Deserialize<Lobby>(json);
                if (lobby != null) lobbies.Add(lobby);
            }
        }

        return lobbies;
    }
}
