namespace GridCityServer.Infrastructure;

public interface ICurrentUserProvider
{
    CurrentUserModel? GetCurrentUser();
}