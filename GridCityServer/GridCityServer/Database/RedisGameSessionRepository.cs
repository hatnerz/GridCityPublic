using GridCityServer.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace GridCityServer.Database;

public class RedisGameSessionRepository : IGameSessionRepository
{
    private readonly IDatabase _database;
    private readonly TimeSpan _gameSessionExpiration = TimeSpan.FromHours(6);
    private const string GameSessionsPrefix = "game:";

    public RedisGameSessionRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task SaveGameSessionAsync(GameSession gameSession)
    {
        string key = GameSessionsPrefix + gameSession.Id;
        string json = JsonSerializer.Serialize(gameSession);
        await _database.StringSetAsync(key, json, _gameSessionExpiration);
    }

    public async Task<bool> DeleteGameSessionAsync(Guid gameSessionId)
    {
        string key = GameSessionsPrefix + gameSessionId;
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<GameSession?> GetGameSessionAsync(Guid gameSessionId)
    {
        string key = GameSessionsPrefix + gameSessionId;
        string? json = await _database.StringGetAsync(key);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<GameSession>(json);
    }

    public async Task<List<GameSession>> GetGameSessionsAsync()
    {
        var endpoints = _database.Multiplexer.GetEndPoints();
        var server = _database.Multiplexer.GetServer(endpoints[0]);
        
        var keys = server.Keys(pattern: GameSessionsPrefix + "*");
        var gameSessions = new List<GameSession>();

        foreach (var key in keys)
        {
            string? json = await _database.StringGetAsync(key);
            if (!string.IsNullOrEmpty(json))
            {
                var gameSession = JsonSerializer.Deserialize<GameSession>(json);
                if (gameSession != null) gameSessions.Add(gameSession);
            }
        }

        return gameSessions;
    }
}
