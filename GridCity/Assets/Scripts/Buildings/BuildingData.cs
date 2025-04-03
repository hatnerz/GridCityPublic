using GridCity.GameLogic.CellElements.BaseElements;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Grid City/Building Data")]
public class BuildingData : ScriptableObject
{
    public Sprite BuildingSprite;
    public Vector2 BuildingSpriteScale = new Vector2(0.08f, 0.08f);
    public Vector2 BuildingSpriteOffset = new Vector2(0, 0.2f);
    public BuildingType BuildingType;
}