using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class Bar : Building
    {
        public Bar()
            : base("Bar")
        {

        }

        public override int BaseScore => 3;

        public override BuildingCategory BuildingCategory => BuildingCategory.Commercial;

        public override BuildingType BuildingType => BuildingType.Bar;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);

            int residentialBonus = Math.Min(adjacentBuildings.Count(e => e.BuildingCategory == BuildingCategory.Residential), 2) * 2;

            int industrialPenalty = adjacentBuildings
                .Count(e => e.BuildingCategory == BuildingCategory.Industrial);

            var finalScore = BaseScore + residentialBonus - industrialPenalty;

            return finalScore;
        }
    }
}