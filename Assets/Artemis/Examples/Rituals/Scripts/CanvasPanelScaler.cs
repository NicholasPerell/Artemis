using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CanvasPanelScaler : MonoBehaviour
{
    [SerializeField]
    PixelPerfectCamera pixelPerfectCamera;

    [SerializeField]
    [Min(1)]
    int resolutionIncrease = 2;

    RectTransform rect;

    private void OnValidate()
    {
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }
        Resize();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Resize();
        Rescale();
    }

    private void Resize()
    {
        if(pixelPerfectCamera == null)
        {
            return;
        }

        //Centers Panel
        rect.anchorMin = 0.5f * Vector2.one;
        rect.anchorMax = 0.5f * Vector2.one;
        rect.anchoredPosition = Vector2.zero;

        //Resizes Panel
        rect.sizeDelta = new Vector2(pixelPerfectCamera.refResolutionX, pixelPerfectCamera.refResolutionY) * resolutionIncrease;
    }

    private void Rescale()
    {
        rect.localScale = Vector3.one * (1.0f * pixelPerfectCamera.pixelRatio / resolutionIncrease);
    }
}
