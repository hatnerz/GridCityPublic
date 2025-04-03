using GridCity.GameLogic.CellElements.BaseElements;

namespace GridCity.GameLogic.Gameplay
{
    public interface IGridState
    {
        CellElement?[,] GridElements { get; }

        int SizeX { get; }
        int SizeY { get; }
    }
}