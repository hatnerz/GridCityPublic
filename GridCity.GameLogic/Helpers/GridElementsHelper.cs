using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;
using GridCity.GameLogic.Shared;
using System.Collections.Generic;
using System.Linq;

namespace GridCity.GameLogic.Helpers
{
    public static class GridElementsHelper 
    {
        public static List<Building> GetAdjacentBuildings(IntCoordinates gridCoordinates, IGridState gridState)
        {
            var adjacentBuildings = new List<Building>();

            var adjacentCoordinates = GetAdjacentCoordinates(gridCoordinates, gridState);
            foreach(var coordinate in adjacentCoordinates)
            {
                var potentialBuilding = GetBuildingIfExistsOnGrid(new IntCoordinates(coordinate.X, coordinate.Y), gridState);
                if(potentialBuilding != null)
                {
                    adjacentBuildings.Add(potentialBuilding);
                }
            }

            return adjacentBuildings;
        }

        public static List<IntCoordinates> GetAdjacentCoordinates(IntCoordinates gridCoordinates, IGridState gridState)
        {
            var x = gridCoordinates.X;
            var y = gridCoordinates.Y;
            var adjacentCoordinates = new List<IntCoordinates>() { new IntCoordinates(x - 1, y), new IntCoordinates(x, y - 1), new IntCoordinates(x + 1, y), new IntCoordinates(x, y + 1) };

            adjacentCoordinates.RemoveAll(coord =>
                coord.X < 0 || coord.X >= gridState.SizeX || // За межами по X
                coord.Y < 0 || coord.Y >= gridState.SizeY    // За межами по Y
            );
            return adjacentCoordinates;
        }

        public static Building? GetBuildingIfExistsOnGrid(IntCoordinates gridCoordinates, IGridState gridState)
        {
            var x = gridCoordinates.X;
            var y = gridCoordinates.Y;
            if (x < 0 || x >= gridState.SizeX || y < 0 || y >= gridState.SizeY)
                return null;

            var potentialBuilding = gridState.GridElements[x, y];

            if (!(potentialBuilding is Building))
                return null;

            return potentialBuilding as Building;
        }

        public static List<Building> GetBuildingsInRowAndColumn(IntCoordinates gridCoordinates, IGridState gridState)
        {
            var rowBuildings = new List<Building>();
            var columnBuildings = new List<Building>();

            for (int x = 0; x < gridState.SizeX; x++)
            {
                var element = gridState.GridElements[x, gridCoordinates.Y];
                if (element is Building building)
                {
                    rowBuildings.Add(building);
                }
            }

            for (int y = 0; y < gridState.SizeY; y++)
            {
                var element = gridState.GridElements[gridCoordinates.X, y];
                if (element is Building building)
                {
                    columnBuildings.Add(building);
                }
            }

            return rowBuildings.Concat(columnBuildings).Distinct().ToList();
        }

        public static List<Building> GetBuildingsInRadius(IntCoordinates center, int radius, IGridState gridState)
        {
            var buildingsInRadius = new List<Building>();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    var currentPosition = new IntCoordinates(center.X + x, center.Y + y);

                    if (currentPosition == center)
                        continue;

                    if (currentPosition.X < 0 || currentPosition.X >= gridState.SizeX ||
                        currentPosition.Y < 0 || currentPosition.Y >= gridState.SizeY)
                    {
                        continue;
                    }

                    var element = gridState.GridElements[currentPosition.X, currentPosition.Y];
                    if (element is Building building)
                    {
                        buildingsInRadius.Add(building);
                    }
                }
            }

            return buildingsInRadius;
        }
    }

}
