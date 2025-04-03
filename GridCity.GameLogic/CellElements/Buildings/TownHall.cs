using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;
using System.Linq;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class TownHall : Building
    {
        public TownHall()
            : base("Town Hall")
        {

        }

        public override int BaseScore => 1;

        public override BuildingCategory BuildingCategory => BuildingCategory.Special;

        public override BuildingType BuildingType => BuildingType.TownHall;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var buildingsInRadius = GridElementsHelper.GetBuildingsInRadius(GridPosition.Value, 1, gridState);

            return BaseScore + buildingsInRadius.Count;
        }
    }
}