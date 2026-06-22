using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    public class ChoiceButton : MonoBehaviour
    {
        private Button button;
        private TextMeshProUGUI textBox;
        private Choice choice;
        private UnityAction<Choice> callback;

        private void Awake()
        {
            button = GetComponent<Button>();
            textBox = GetComponentInChildren<TextMeshProUGUI>();
            button.onClick.AddListener(RespondToClick);
        }

        public void Init(Choice _choice, UnityAction<Choice> _callback)
        {
            choice = _choice;
            callback = _callback;

            gameObject.name = choice.text + " Button";
            textBox.text = choice.text;
        }

        private void RespondToClick()
        {
            callback(choice);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(RespondToClick);
        }
    }
}