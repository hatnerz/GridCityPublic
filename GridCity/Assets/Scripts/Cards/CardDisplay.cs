using GridCity.GameLogic.Cards;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardScore;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private GameObject backgroundOutlineObject;
    [SerializeField] private TMP_Text buildingCategory;

    private bool isHighlighted;
    private Outline outline;

    public CardData BuildingCardData { get; private set; }
    public BuildingCard BuildingCard { get; private set; }
    public CardSelectionManager SelectionManager { get; set; }

    public void Start()
    {
        UpdateDisplay();
        InitializeOutline();
    }

    public void SetCardData(CardData data, BuildingCard card)
    {
        BuildingCardData = data;
        BuildingCard = card;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (BuildingCardData != null)
        {
            cardImage.sprite = BuildingCardData.CardSprite;
            cardDescription.text = string.Join(Environment.NewLine, BuildingCardData.EffectsDescriptions);
        }

        if (BuildingCard != null)
        {
            cardName.text = BuildingCard.Building.Name;
            cardScore.text = BuildingCard.Building.BaseScore.ToString();
            buildingCategory.text = $"Category: {BuildingCard.Building.BuildingCategory.ToString()}";
        }
    }

    public void SetHighlight(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
        outline.enabled = isHighlighted;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.SelectCard(this);
    }

    private void InitializeOutline()
    {
        outline = backgroundOutlineObject.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.5f, 0f, 1f);
        outline.effectDistance = new Vector2(3f, -3f);
        outline.useGraphicAlpha = false;
        outline.enabled = false;
    }
}
