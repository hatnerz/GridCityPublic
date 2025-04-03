namespace GridCityServer.Dtos;

public record LobbyItemDto(
    Guid Id,
    string Name,
    int MaxPlayers,
    int JoinedPlayers,
    bool CanJoin,
    Guid CreatedPlayerId,
    string CreatedPlayerName,
    int MapSizeX,
    int MapSizeY,
    int DeckPerPlayerSize);