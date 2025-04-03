namespace GridCityServer.Models;

public class MatchPlayer : Entity<int>
{
    private MatchPlayer()
    {
    }

    public Guid PlayerId { get; private set; }
    public Guid MatchId { get; private set; }
    public int FinalScore { get; private set; }
    public MatchPlayer(Guid playerId, int finalScore)
    {
        PlayerId = playerId;
        FinalScore = finalScore;
    }
}