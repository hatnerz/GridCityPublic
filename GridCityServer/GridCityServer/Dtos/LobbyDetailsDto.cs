namespace GridCityServer.Dtos;

public record LobbyDetailsDto(
    Guid Id,
    string Name,
    int MaxPlayers,
    int JoinedPlayers,
    Guid CreatedPlayerId,
    string CreatedPlayerName,
    int MapSizeX,
    int MapSizeY,
    int DeckPerPlayerSize,
    List<string> Players);