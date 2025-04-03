using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class University : Building
    {
        public University()
            : base("University")
        {

        }

        public override int BaseScore => 5;

        public override BuildingCategory BuildingCategory => BuildingCategory.Facilities;

        public override BuildingType BuildingType => BuildingType.University;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);
            var rowAndColumnBuildings = GridElementsHelper.GetBuildingsInRowAndColumn(GridPosition.Value, gridState);

            int universityPenalty = rowAndColumnBuildings
                .Count(building => building.BuildingType == BuildingType.University && building != this) * -2;

            int facilitiesBonus = adjacentBuildings
                .Count(building => building.BuildingCategory == BuildingCategory.Facilities);

            return BaseScore + universityPenalty + facilitiesBonus;
        }
    }
}