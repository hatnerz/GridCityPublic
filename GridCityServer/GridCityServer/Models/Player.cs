namespace GridCityServer.Models;

public class Player : Entity<Guid>
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    public Player(string username, string passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;
    }
}
