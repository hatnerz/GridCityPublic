using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class Park : Building
    {
        public Park()
            : base("Park")
        {
        }

        public override int BaseScore => 3;

        public override BuildingCategory BuildingCategory => BuildingCategory.Facilities;

        public override BuildingType BuildingType => BuildingType.Park;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);

            int additionalPoints = adjacentBuildings
                .Count(e => e.BuildingCategory == BuildingCategory.Commercial || e.BuildingCategory == BuildingCategory.Residential);

            additionalPoints = Math.Min(additionalPoints, 2);

            int penaltyPoints = adjacentBuildings
                .Count(e => e.BuildingCategory == BuildingCategory.Industrial);

            return BaseScore + additionalPoints - penaltyPoints;
        }

    }
}