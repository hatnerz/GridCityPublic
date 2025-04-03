using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class SuburbanHouse : Building
    {
        public SuburbanHouse()
            : base("Suburban House")
        {

        }

        public override int BaseScore => 2;

        public override BuildingCategory BuildingCategory => BuildingCategory.Residential;

        public override BuildingType BuildingType => BuildingType.SuburbanHouse;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentBuildings = GridElementsHelper.GetAdjacentBuildings(GridPosition.Value, gridState);

            var finalScore = BaseScore
                + adjacentBuildings.Count(e => e.BuildingCategory == BuildingCategory.Facilities)
                - adjacentBuildings.Count(e => e.BuildingCategory == BuildingCategory.Industrial);

            return finalScore;
        }
    }
}