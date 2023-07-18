using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.Rituals
{
    public class CenterAndSquarePanel : MonoBehaviour
    {
        RectTransform rect;
        Canvas parent;

        [SerializeField]
        private int minPixelMargin;
        [SerializeField]
        private int pixelSizeToIgnoreMargins;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            parent = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            Reposition();
            Resize();
        }

        private void Reposition()
        {
            rect.anchorMin = 0.5f * Vector2.one;
            rect.anchorMax = 0.5f * Vector2.one;
            rect.anchoredPosition = Vector2.zero;
        }

        private void Resize()
        {
            Vector2 screen = parent.renderingDisplaySize;
            if(screen.x < screen.y)
            {
                screen = Vector2.one * screen.x;
            }
            else
            {
                screen = Vector2.one * screen.y;
            }

            if(screen.x > pixelSizeToIgnoreMargins + minPixelMargin * 2)
            {
                screen -= Vector2.one * 2 * minPixelMargin;
            }
            else if(screen.x > pixelSizeToIgnoreMargins)
            {
                screen = Vector2.one * pixelSizeToIgnoreMargins;
            }

            rect.sizeDelta = screen;
        }
    }
}
