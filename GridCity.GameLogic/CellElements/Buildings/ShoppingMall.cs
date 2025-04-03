using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Helpers;

namespace GridCity.GameLogic.CellElements.Buildings
{
    public class ShoppingMall : Building
    {
        public ShoppingMall()
            : base("Shopping Mall")
        {
        }

        public override int BaseScore => 4;

        public override BuildingCategory BuildingCategory => BuildingCategory.Commercial;

        public override BuildingType BuildingType => BuildingType.ShoppingMall;

        public override int CalculateTotalBuildingScore(IGridState gridState)
        {
            if (GridPosition == null)
                return BaseScore;

            var adjacentCoordinates = GridElementsHelper.GetAdjacentCoordinates(GridPosition.Value, gridState);

            int totalScore = BaseScore;

            foreach (var coordinate in adjacentCoordinates)
            {
                var building = GridElementsHelper.GetBuildingIfExistsOnGrid(coordinate, gridState);

                if (building == null)
                {
                    totalScore -= 1;
                }
                else if (building.BuildingCategory == BuildingCategory.Commercial)
                {
                    totalScore += 2;
                }
                else if (building.BuildingCategory == BuildingCategory.Industrial)
                {
                    totalScore -= 1;
                }
            }

            return totalScore;
        }
    }
}