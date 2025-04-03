using GridCity.GameLogic.Shared;

namespace GridCity.GameLogic.CellElements.BaseElements
{
    public abstract class CellElement
    {
        protected CellElement(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public IntCoordinates? GridPosition { get; set; }

        public bool IsPlaced() => GridPosition != null;
    }
}