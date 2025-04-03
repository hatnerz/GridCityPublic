using GridCityServer.Dtos;

namespace GridCityServer.Services;

public interface IGameplayService
{
    Task<Guid?> GetGameSessionIdByLobbyIdAsync(Guid lobbyId);
    Task<GameSessionFinalResultDto?> EndGameSessionAsync(Guid gameSessionId);
    Task<PlayerCardsDto?> GetPlayerActualCardsAsync(Guid gameSessionId, Guid playerId);
    Task<List<PlayerScoreDto>> GetPlayersScoreAsync(Guid gameSessionId);
    Task<GameSessionInitialDto?> InitializeGameSessionAsync(Guid gameSessionId);
    Task<bool?> IsNoCardsLeftAsync(Guid gameSessionId);
    Task<MoveResultDto?> MakeMoveAsync(GameSessionMoveDto moveDto);
    Task<Guid> CreateEmptyGameSessionAsync(Guid lobbyId);
}