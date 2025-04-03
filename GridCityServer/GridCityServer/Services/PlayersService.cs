using GridCityServer.Database;
using GridCityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GridCityServer.Services;

public class PlayersService : IPlayersService
{
    private readonly ApplicationDbContext _context;

    public PlayersService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> CreatePlayerAccount(string username, string passwordHash)
    {
        var player = new Player(username, passwordHash);
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }

    public async Task<Player?> GetPlayerAccount(Guid playerId)
    {
        return await _context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
    }

    public async Task<Player?> GetPlayerAccountByUsername(string username)
    {
        return await _context.Players.FirstOrDefaultAsync(p => p.Username == username);
    }
}
