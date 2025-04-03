using System;
using System.Collections.Generic;

namespace Assets.Scripts.Dtos
{
    public class LobbyDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int JoinedPlayers { get; set; }
        public Guid CreatedPlayerId { get; set; }
        public string CreatedPlayerName { get; set; }
        public int MapSizeX { get; set; }
        public int MapSizeY { get; set; }
        public int DeckPerPlayerSize { get; set; }
        public List<string> Players { get; set; }
    }
}
