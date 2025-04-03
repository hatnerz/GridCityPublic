using System;
using System.Collections.Generic;

namespace Assets.Scripts.Dtos
{
    public class GameSessionFinalResultDto
    {
        public List<PlayerScoreDto> PlayerScores { get; set; }
        public PlayerScoreDto Winner { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
