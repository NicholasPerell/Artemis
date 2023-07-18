using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis.Example.Rituals
{
    [RequireComponent(typeof(CenterAndSquarePanel))]
    public class CircularSorterUI : MonoBehaviour
    {
        [SerializeField]
        public RectTransform[] buttons;
        [SerializeField]
        [Min(0)]
        public int numberOfItemsToUse;
        [SerializeField]
        [Range(0, Mathf.PI * 2)]
        public float startingRadian;

        RectTransform rect;

        void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            RepostionItems();
        }

        void RepostionItems()
        {
            //Only works if these items all have the same sizing
            float length = rect.sizeDelta.x / 2 - buttons[0].sizeDelta.x / 2;

            numberOfItemsToUse = Mathf.Min(buttons.Length, numberOfItemsToUse);

            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            for(int i = 0; i < numberOfItemsToUse; i++)
            {
                buttons[i].anchorMin = 0.5f * Vector2.one;
                buttons[i].anchorMax = 0.5f * Vector2.one;
                buttons[i].anchoredPosition = new Vector2(Mathf.Cos(i * Mathf.PI * 2 / numberOfItemsToUse + startingRadian), Mathf.Sin(i * Mathf.PI * 2 / numberOfItemsToUse + startingRadian)) * length;
                min = Vector2.Min(buttons[i].anchoredPosition, min);
                max = Vector2.Max(buttons[i].anchoredPosition, max);
            }

            Vector2 offset = (max + min) / 2;

            for (int i = 0; i < numberOfItemsToUse; i++)
            {
                buttons[i].anchoredPosition -= offset;
            }
        }
    }
}
