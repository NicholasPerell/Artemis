using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawnBetween : MonoBehaviour
{
    [SerializeField]
    RectTransform from;
    [SerializeField]
    RectTransform to;

    [SerializeField]
    int lineThickness;

    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        if (rect != null)
        {
            rect.anchorMin = 0.5f * Vector2.one;
            rect.anchorMax = 0.5f * Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
    }

    private void Update()
    {
        if (from != null && to != null)
        {
            Reposition();
        }
    }

    public void SetDots(RectTransform _from, RectTransform _to)
    {
        from = _from;
        to = _to;
    }

    private void Reposition()
    {
        rect.anchorMin = 0.5f * Vector2.one;
        rect.anchorMax = 0.5f * Vector2.one;

        rect.anchoredPosition = (from.anchoredPosition + to.anchoredPosition) / 2;

        Vector2 diff = to.anchoredPosition - from.anchoredPosition;
        float length = diff.magnitude;
        rect.sizeDelta = new Vector2(length, lineThickness);
        rect.localRotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(diff, Vector2.right));
    }
}
