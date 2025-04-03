using System;

namespace Assets.Scripts.Core
{
    public static class GameState
    {
        public static int CurrentLevelNumber { get; set; } = 1;
        public static Guid MultiplayerLobbyId { get; set; } = Guid.Empty;
        public static bool IsPlayersTurn { get; set; } = true;
    }
}
