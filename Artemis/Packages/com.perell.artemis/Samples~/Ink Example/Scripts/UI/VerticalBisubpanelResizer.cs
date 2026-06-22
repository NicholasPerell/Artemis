using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class VerticalBisubpanelResizer : MonoBehaviour
    {
        [SerializeField]
        RectTransform affectingPanel, affectedPanel;

        VerticalLayoutGroup verticalLayoutGroup;
        RectTransform rect;

        private void Awake()
        {
            verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            rect = GetComponent<RectTransform>();
            Debug.Assert(!verticalLayoutGroup.childControlHeight);
        }

        private void Update()
        {
            Debug.Assert(!verticalLayoutGroup.childControlHeight);

            float height = rect.rect.height;
            height -= verticalLayoutGroup.padding.vertical;

            if (affectingPanel.gameObject.activeSelf)
            {
                height -= affectingPanel.sizeDelta.y;
                height -= verticalLayoutGroup.spacing;
            }

            affectedPanel.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }
    }
}