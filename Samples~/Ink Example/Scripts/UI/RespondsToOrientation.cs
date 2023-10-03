using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class RespondsToOrientation : MonoBehaviour
    {
        RectTransform rectTransform;
        bool isVertical = false;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {
            isVertical = CalcShouldBeVertical();
            if (isVertical)
            {
                TurnVertical();
            }
            else
            {
                TurnHorizontal();
            }
        }

        protected virtual void Update()
        {
            bool previousIsVertical = isVertical;
            isVertical = CalcShouldBeVertical();
            if (previousIsVertical != isVertical)
            {
                if (isVertical)
                {
                    TurnVertical();
                }
                else
                {
                    TurnHorizontal();
                }
            }
        }

        private bool CalcShouldBeVertical()
        {
            return CalculateRatio() < 1;
        }

        protected abstract void TurnVertical();
        protected abstract void TurnHorizontal();

        protected float CalculateRatio()
        {
            float width = rectTransform.rect.width * rectTransform.localScale.x;
            float height = rectTransform.rect.height * rectTransform.localScale.x;

            if (transform.parent)
            {
                width *= transform.parent.lossyScale.x;
                height *= transform.parent.lossyScale.y;
            }

            if (height == 0)
            {
                return float.PositiveInfinity;
            }
            else
            {
                return width / height;
            }
        }
    }
}