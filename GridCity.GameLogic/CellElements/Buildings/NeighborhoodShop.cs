using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class NeighborhoodShop : Building
    {
        public NeighborhoodShop()
            : base("Neighborhood Shop")
        {

        }

        public override int BaseScore => 3;

        public override BuildingCategory BuildingCategory => BuildingCategory.Commercial;

        public override BuildingType BuildingType => BuildingType.NeighborhoodShop;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);

            var finalScore = BaseScore
                + adjacentBuildings.Count(e => e.BuildingCategory == BuildingCategory.Residential)
                - adjacentBuildings.Count(e => e.BuildingCategory == BuildingCategory.Industrial);

            return finalScore;
        }
    }
}