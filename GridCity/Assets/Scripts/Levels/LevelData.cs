using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Grid City/Level Data")]
public class LevelData : ScriptableObject
{
    public int LevelNumber;
    public List<DeckComposition> DeckComposition = new List<DeckComposition>();
    public int TargetScore;
    public Vector2Int GridSize;
}