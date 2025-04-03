using System;

namespace Assets.Scripts.Dtos
{
    public class PlayerScoreDto
    {
        public Guid PlayerId { get; set; }
        public int Score { get; set; }
    }
}
