using Assets.Scripts.UI.Buttons.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Buttons
{
    public class ActionButton : MonoBehaviour
    {
        private Button button;
        private IButtonAction action;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void SetAction(IButtonAction action)
        {
            this.action = action;
            button.onClick.AddListener(ExecuteAction);
        }

        private void ExecuteAction()
        {
            action?.Execute();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(ExecuteAction);
        }
    }
}