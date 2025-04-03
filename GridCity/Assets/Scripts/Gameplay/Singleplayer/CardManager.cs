using GridCity.GameLogic.Cards;
using GridCity.GameLogic.CellElements.BaseElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private int maxCardsInHand = 3;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RectTransform cardParent;
    [SerializeField] private CardSelectionManager selectionManager;
    [SerializeField] private HandVisualizer handVisualizer;
    [SerializeField] private LevelData levelData;

    private List<Card> deckCards = new List<Card>();
    private List<GameObject> cardObjectsInHand = new List<GameObject>();

    public int InitialDeckSize { get; private set; }
    public int RemainsCardsInDeck { get => deckCards.Count; }
    public int RemainsCardsInHand { get => cardObjectsInHand.Count; }

    public event CardEventHandler CardTakenFromDeck;

    public delegate void CardEventHandler(GameObject playedCard);
    public delegate void LastCardEventHandler();

    void Start()
    {
        if(levelData != null)
        {
            InitializeLevelDeck(levelData);
        }
    }

    public void InitializeLevelDeck(LevelData levelData)
    {
        InitializeDeck(levelData.DeckComposition);

        for (int i = 0; i < maxCardsInHand; i++)
        {
            TakeCardFromDeck();
        }
    }

    public void InitializeDeck(List<DeckComposition> deck)
    {
        var random = new System.Random();

        InitialDeckSize = deck.Sum(e => e.Count);
        deckCards = deck
            .SelectMany(e => Enumerable.Repeat(e.CardType, e.Count)
                .Select(cardType => CardFactory.CreateCard(cardType)))
            .OrderBy(e => random.Next())
            .ToList();
    }

    public void TakeCardFromDeck()
    {
        if(deckCards.Count == 0)
            return;

        var card = deckCards[0];
        deckCards.RemoveAt(0);
        var takenCard = AddCardGameObject(ResourceManager.Instance.CardDataDictionary[card.Type], card as BuildingCard);
        handVisualizer.VisualizeCardTaking(takenCard, cardObjectsInHand);

        CardTakenFromDeck?.Invoke(takenCard);
    }

    public BuildingCard TryPlayActiveCard()
    {
        var selectedCard = selectionManager.SelectedCard;
        if (selectedCard == null)
            return null;

        var selectedCardObject = selectedCard.gameObject;

        cardObjectsInHand.Remove(selectedCardObject);

        handVisualizer.VisualizeCardRemoval(selectedCardObject, cardObjectsInHand);

        if (RemainsCardsInDeck > 0)
        {
            TakeCardFromDeck();
        }

        return selectedCard.BuildingCard;
    }

    private GameObject AddCardGameObject(CardData cardData, BuildingCard card)
    {
        var createdCardGameObject = CreateCardGameObject(cardData, card);
        cardObjectsInHand.Add(createdCardGameObject);
        return createdCardGameObject;
    }

    private GameObject CreateCardGameObject(CardData cardData, BuildingCard card)
    {
        GameObject cardObject = Instantiate(cardPrefab, cardParent);

        cardObject.name = card.Building.Name + " card";

        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();

        cardDisplay.SetCardData(cardData, card);

        cardDisplay.SelectionManager = selectionManager;

        return cardObject;
    }
}