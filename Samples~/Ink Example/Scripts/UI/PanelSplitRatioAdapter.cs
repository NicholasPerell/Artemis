using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    public class PanelSplitRatioAdapter : RespondsToOrientation
    {
        [SerializeField]
        RectTransform[] panels;

        protected override void TurnHorizontal()
        {
            ChangeLayout(Vector2.zero, Vector2.right, Vector2.up);
        }

        protected override void TurnVertical()
        {
            ChangeLayout(Vector2.up, Vector2.down, Vector2.right);
        }

        private void ChangeLayout(Vector2 anchor, Vector2 unit, Vector2 jumper)
        {
            Vector2 a, b;
            for (int i = 0; i < panels.Length; i++)
            {
                a = anchor + unit * i / panels.Length;
                b = anchor + unit * (i + 1) / panels.Length + jumper;

                panels[i].anchorMin = Vector2.Min(a, b);
                panels[i].anchorMax = Vector2.Max(a, b);
            }
        }
    }
}