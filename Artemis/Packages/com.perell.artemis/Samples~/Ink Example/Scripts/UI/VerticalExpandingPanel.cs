using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class VerticalExpandingPanel : MonoBehaviour
    {
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

            float height = 0;
            int children = 0;
            Transform child;
            RectTransform childRect;
            height += verticalLayoutGroup.padding.vertical;

            for(int i = 0; i < transform.childCount; i++)
            {
                child = transform.GetChild(i);
                if (child.gameObject.activeSelf && child.TryGetComponent<RectTransform>(out childRect))
                {
                    height += childRect.sizeDelta.y;
                    children++;
                }
            }

            height += Mathf.Max(children - 1, 0) * verticalLayoutGroup.spacing;

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }
    }
}