using Assets.Scripts.Dtos;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    public class TopPlayerItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _colorVisualizer;
        [SerializeField] private TMP_Text _topPlayerText;

        public void DisplayData(PlayerDetailsDto player, int position)
        {
            _colorVisualizer.color = player.Color;
            _topPlayerText.text = $"{position}. {player.Name}: {player.Score}";
        }
    }
}
