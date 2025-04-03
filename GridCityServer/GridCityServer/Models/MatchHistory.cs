using System.Reflection;

namespace GridCityServer.Models;

public class MatchHistory : Entity<Guid>
{
    private MatchHistory()
    {
    }

    public Guid WinnerPlayerId { get; private set; }
    public List<MatchPlayer> Players { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public MatchHistory(List<MatchPlayer> players, Guid winnerPlayerId)
    {
        Id = Guid.NewGuid();
        Players = players;
        WinnerPlayerId = winnerPlayerId;
        CompletedAt = DateTime.UtcNow;
    }
}