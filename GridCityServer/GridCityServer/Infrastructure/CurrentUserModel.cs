namespace GridCityServer.Infrastructure;

public record CurrentUserModel(
    Guid PlayerId,
    string Username);
