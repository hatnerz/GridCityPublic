namespace GridCity.GameLogic.CellElements.BaseElements
{
    public class Ground : CellElement
    {
        public Ground(string name, GroundType groundType)
            : base(name)
        {
            GroundType = groundType;
        }

        public GroundType GroundType { get; private set; }
    }
}