using Assets.Scripts.UI.Buttons;
using Assets.Scripts.UI.Buttons.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private ActionButton backToMenuButton;
    [SerializeField] private GameObject levelSelectButtonPrefab;
    [SerializeField] private GameObject levelSelectButtonsParent;

    private void Start()
    {
        InitializeLevelSelectButton();
        InitializeBackToMenuButton();
    }

    private void InitializeLevelSelectButton()
    {
        var levels = ResourceManager.Instance.LevelDataDictionary.ToList();
        levels.Sort((x, y) => x.Key - y.Key);
        var startLevelButtonPosition = new Vector2(0, 0);
        foreach (var level in levels)
        {
            var button = Instantiate(levelSelectButtonPrefab, levelSelectButtonsParent.transform);
            var startLevelAction = new StartLevelAction(level.Key, GameManager.Instance);
            button.GetComponent<ActionButton>().SetAction(startLevelAction);

            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.localPosition = startLevelButtonPosition;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70);
            button.GetComponent<LevelSelectButton>().LevelSelectButtonText.text = level.Key.ToString();
            startLevelButtonPosition.x += 100;
        }
    }

    private void InitializeBackToMenuButton()
    {
        backToMenuButton.SetAction(new BackToMenuAction(GameManager.Instance));
    }
}
