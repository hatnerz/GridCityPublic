using System.Security.Claims;

namespace GridCityServer.Infrastructure;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserModel? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return null;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = user.FindFirst(ClaimTypes.Name)?.Value;

        if (userId == null || username == null || !Guid.TryParse(userId, out Guid userIdGuid))
            return null;

        return new CurrentUserModel(
            userIdGuid, username);
    }
}