using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class ApartmentBuilding : Building
    {
        public ApartmentBuilding()
            : base("Apartment Building")
        {

        }

        public override int BaseScore => 4;

        public override BuildingCategory BuildingCategory => BuildingCategory.Residential;

        public override BuildingType BuildingType => BuildingType.ApartmentBuilding;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);

            var bonusPoints = adjacentBuildings
                .Count(building => building.BuildingCategory == BuildingCategory.Residential);

            var penaltyPoints = adjacentBuildings
                .Count(building => building.BuildingCategory == BuildingCategory.Industrial) * -2;

            return BaseScore + bonusPoints + penaltyPoints;
        }
    }
}