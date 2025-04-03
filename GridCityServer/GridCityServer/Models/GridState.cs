using GridCity.GameLogic.CellElements.BaseElements;
using GridCity.GameLogic.Gameplay;

namespace GridCityServer.Models;

public class GridState : IGridState
{
    public CellElement?[,] GridElements { get; private set; }

    public int SizeX { get { return GridElements.GetLength(0); } }
    public int SizeY { get { return GridElements.GetLength(1); } }

    public GridState(CellElement[,] grid)
    {
        GridElements = grid;
    }
}
