using GridCity.GameLogic.CellElements.BaseElements;
using System;

namespace Assets.Scripts.Dtos
{
    public class MoveResultDto
    {
        public Guid PlacedPlayerId { get; set; }
        public Guid NextTurnPlayerId { get; set; }
        public BuildingType PlacedBuilding { get; set; }
        public int GridPositionX { get; set; }
        public int GridPositionY { get; set; }
    }
}
