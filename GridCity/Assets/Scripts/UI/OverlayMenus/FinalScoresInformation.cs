using Assets.Scripts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.OverlayMenus
{
    public class FinalScoresInformation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _winnerText;
        [SerializeField] private TMP_Text _scoreResultsText;

        public void SetPlayersScores(string winner, List<PlayerScoreDisplayDto> playerScores)
        {
            _winnerText.text = "Winner: " + winner;
            _scoreResultsText.text = "Scores:" + Environment.NewLine + string.Join(
                Environment.NewLine,
                playerScores
                    .OrderBy(e => e.Position)
                    .Select(e => $"{e.Position}. {e.Name}: {e.Score}")
                    .ToList());
        }
    }
}
