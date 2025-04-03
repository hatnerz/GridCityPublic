using GridCity.GameLogic.Gameplay;

namespace GridCity.GameLogic.CellElements.BaseElements
{
    public abstract class Building : CellElement
    {
        protected Building(string name)
            : base(name)
        {
        }

        public abstract BuildingCategory BuildingCategory { get; }

        public abstract BuildingType BuildingType { get; }

        public abstract int BaseScore { get; }

        public virtual int CalculateTotalBuildingScore(IGridState gridState) => BaseScore;
    }
}