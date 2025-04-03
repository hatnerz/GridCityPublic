using System;
using System.Collections.Generic;

namespace Assets.Scripts.Dtos
{
    public class GameSessionInitialDto
    {
        public Guid Id { get; set; }
        public int GridSizeX { get; set; }
        public int GridSizeY { get; set; }
        public List<PlayerDto> Players { get; set; }
        public Guid FirstTurnPlayer { get; set; }
        public int InitialDeckSize { get; set; }
    }
}
