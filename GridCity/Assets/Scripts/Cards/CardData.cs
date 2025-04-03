using GridCity.GameLogic.Cards;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "Grid City/Card Data")]
public class CardData : ScriptableObject
{
    public CardType CardType;
    public Sprite CardSprite;
    public List<string> EffectsDescriptions;
}
