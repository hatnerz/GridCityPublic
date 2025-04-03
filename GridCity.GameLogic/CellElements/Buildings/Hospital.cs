using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class Hospital : Building
    {
        public Hospital()
            : base("Hospital")
        {

        }

        public override int BaseScore => 4;

        public override BuildingCategory BuildingCategory => BuildingCategory.Facilities;

        public override BuildingType BuildingType => BuildingType.Hospital;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var buildingsInRadius = GridElementsHelper.GetBuildingsInRadius(GridPosition.Value, 1, gridState);

            var bonusPoints = Math.Min(
                buildingsInRadius.Count(building => building.BuildingCategory == BuildingCategory.Residential ||
                    building.BuildingCategory == BuildingCategory.Facilities),
                5);

            var penaltyPoints = buildingsInRadius
                .Count(building => building.BuildingCategory == BuildingCategory.Industrial) * 2;

            return BaseScore + bonusPoints - penaltyPoints;
        }


    }
}