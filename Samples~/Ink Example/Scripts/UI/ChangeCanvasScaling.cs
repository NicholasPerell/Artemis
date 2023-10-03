using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class ChangeCanvasScaling : RespondsToOrientation
    {
        CanvasScaler canvasScaler;

        protected override void Awake()
        {
            base.Awake();
            canvasScaler = GetComponent<CanvasScaler>();
        }

        protected override void TurnHorizontal()
        {
            canvasScaler.matchWidthOrHeight = 1;
        }

        protected override void TurnVertical()
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
    }
}