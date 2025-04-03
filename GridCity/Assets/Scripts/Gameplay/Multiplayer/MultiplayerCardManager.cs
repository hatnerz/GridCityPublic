using GridCity.GameLogic.Cards;
using GridCity.GameLogic.CellElements.BaseElements;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.Gameplay
{
    internal class MultiplayerCardManager : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private RectTransform cardParent;
        [SerializeField] private HandVisualizer handVisualizer;
        [SerializeField] private CardSelectionManager selectionManager;

        public bool IsInitialized { get; private set; } = false;

        private List<(CardType card, GameObject gameObject)> _cardsInHand = new List<(CardType card, GameObject gameObject)>();


        public BuildingCard TryPlayActiveCard()
        {
            var selectedCard = selectionManager.SelectedCard;
            if (selectedCard == null)
                return null;

            var selectedCardObject = selectedCard.gameObject;

            _cardsInHand.Remove(_cardsInHand.FirstOrDefault(e => e.gameObject == selectedCardObject));

            handVisualizer.VisualizeCardRemoval(selectedCardObject, _cardsInHand.Select(e => e.gameObject).ToList());

            return selectedCard.BuildingCard;
        }

        public void UpdatePlayerActualCards(List<CardType> cards)
        {
            if (!IsInitialized)
            {
                cards.ForEach(c => TakeCardToHand(c));
                IsInitialized = true;
                return;
            }

            var cardsInHandTypes = _cardsInHand.Select(e => e.card).ToList();

            var missingCard = cards.Cast<CardType?>().FirstOrDefault(c => !cardsInHandTypes.Contains(c.Value));
            if (missingCard != null)
                TakeCardToHand(missingCard.Value);
        }

        private void TakeCardToHand(CardType cardType)
        {
            var card = CardFactory.CreateCard(cardType);
            var takenCard = AddCardGameObject(ResourceManager.Instance.CardDataDictionary[card.Type], card as BuildingCard);
            handVisualizer.VisualizeCardTaking(takenCard, _cardsInHand.Select(e => e.gameObject).ToList());
        }

        private GameObject AddCardGameObject(CardData cardData, BuildingCard card)
        {
            var createdCardGameObject = CreateCardGameObject(cardData, card);
            _cardsInHand.Add((cardData.CardType, createdCardGameObject));
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
}
