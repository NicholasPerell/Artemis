using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    [RequireComponent(typeof(Button))]
    public class RandomChanceButton : MonoBehaviour
    {
        Button button;

        [SerializeField]
        UnityEvent[] onClickPossibilities;

        private void Awake()
        {
            button = GetComponent<Button>();    
        }

        private void OnEnable()
        {
            button.onClick.AddListener(RespondToClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(RespondToClick);
        }

        private void RespondToClick()
        {
            if (onClickPossibilities.Length > 0)
            {
                onClickPossibilities[Random.Range(0, onClickPossibilities.Length)]?.Invoke();
            }
        }
    }
}