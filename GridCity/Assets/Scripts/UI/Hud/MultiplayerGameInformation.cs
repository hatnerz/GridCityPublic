using Assets.Scripts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    internal class MultiplayerGameInformation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerTurnText;
        [SerializeField] private TMP_Text _totalCardsInDeckText;
        [SerializeField] private TMP_Text _remainsCardsInDeckText;
        [SerializeField] private TMP_Text _yourScoreText;
        [SerializeField] private TMP_Text _totalPlayersText;
        [SerializeField] private TMP_Text _waitingForPlayersText;
        [SerializeField] private Transform _playersTopScoresContainer;
        [SerializeField] private GameObject _playerTopScoreItemPrefab;

        public void Initialize(
            bool isYourTurn,
            string currentTurnPlayerName,
            int initialDeckSize,
            int remainsCardsInDeck,
            int yourScore,
            List<PlayerDetailsDto> players)
        {
            _totalCardsInDeckText.text = $"Total cards: {initialDeckSize}";
            _totalPlayersText.text = $"Total players: {players.Count}";

            UpdateTurnInformation(isYourTurn, currentTurnPlayerName);
            UpdateRemainsCardsInformation(remainsCardsInDeck);
            UpdateScoreInformation(yourScore, players);
        }

        public void UpdateRemainsCardsInformation(int remainsCards)
        {
            _remainsCardsInDeckText.text = $"Remains cards: {remainsCards}";
        }

        public void UpdateTurnInformation(bool isYourTurn,string currentTurnPlayerName)
        {
            _playerTurnText.text = isYourTurn ? "Now it's your turn" : $"Current player turn: {currentTurnPlayerName}";
        }

        public void UpdateScoreInformation(int yourScore, List<PlayerDetailsDto> players)
        {
            Debug.Log($"Score updated inside. MyScore: {yourScore}");
            Debug.Log($"Score updated inside. PlayersScore: {players[0].Id} {players[0].Score} {players[0].Name}");
            _yourScoreText.text = $"Your score: {yourScore}";
            
            foreach (Transform child in _playersTopScoresContainer)
            {
                Destroy(child.gameObject);
            }

            players = players.OrderByDescending(e => e.Score).ToList();
            for(int i = 0; i < players.Count; i++)
            {
                var playerItem = Instantiate(_playerTopScoreItemPrefab, _playersTopScoresContainer);
                playerItem.transform.localPosition = new Vector3(0, -i * 25);
                playerItem.GetComponent<TopPlayerItem>().DisplayData(players[i], i + 1);
            }
        }

        public void DisableWaitingForPlayersText()
        {
            _waitingForPlayersText.gameObject.SetActive(false);
        }
    }
}
