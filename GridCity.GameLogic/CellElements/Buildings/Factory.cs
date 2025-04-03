using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class Factory : Building
    {
        public Factory()
            : base("Factory")
        {

        }

        public override int BaseScore => 3;

        public override BuildingCategory BuildingCategory => BuildingCategory.Industrial;

        public override BuildingType BuildingType => BuildingType.Factory;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);

            var bonusPoints = adjacentBuildings
                .Count(building => building.BuildingCategory == BuildingCategory.Industrial);

            return BaseScore + bonusPoints;
        }
    }
}