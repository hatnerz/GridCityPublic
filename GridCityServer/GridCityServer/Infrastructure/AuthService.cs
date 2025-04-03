using GridCityServer.Dtos;
using GridCityServer.Infrastructure.AuthSecurity;
using GridCityServer.Services;

namespace GridCityServer.Infrastructure;

public class AuthService : IAuthService
{
    private readonly IPlayersService _playersService;
    private readonly IJwtGenerator _jwtGenerator;

    public AuthService(IPlayersService playersService, IJwtGenerator jwtGenerator)
    {
        _playersService = playersService;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<AuthResultDto> LoginAsync(string username, string password)
    {
        var player = await _playersService.GetPlayerAccountByUsername(username);
        if (player == null)
            return new(false, null, "Player with given username does not exist");

        if (!PasswordHasher.VerifyPassword(password, player.PasswordHash))
            return new(false, null, "Invalid password");

        var jwt = _jwtGenerator.GenerateJwtToken(player.Id, player.Username);
        return new(true, jwt, null);
    }

    public async Task<AuthResultDto> RegisterAsync(string username, string password)
    {
        var existingPlayer = await _playersService.GetPlayerAccountByUsername(username);
        if (existingPlayer != null)
            return new(false, null, "Player with given username already exists");

        var passwordHash = PasswordHasher.HashPassword(password);
        var player = await _playersService.CreatePlayerAccount(username, passwordHash);
        if (player == null)
            return new(false, null, "Error during registration, please try again");

        var jwt = _jwtGenerator.GenerateJwtToken(player.Id, player.Username);
        return new(true, jwt, null);
    }
}
