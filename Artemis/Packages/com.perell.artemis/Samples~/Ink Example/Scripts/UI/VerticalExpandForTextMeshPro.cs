using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VerticalExpandForTextMeshPro : MonoBehaviour
    {
        TextMeshProUGUI textBox;
        RectTransform rect;

        private void Awake()
        {
            textBox = GetComponent<TextMeshProUGUI>();
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (textBox.text.Trim() != "")
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, textBox.renderedHeight + textBox.margin.y + textBox.margin.w);
            }
            else
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, textBox.preferredHeight);
            }
        }
    }
}