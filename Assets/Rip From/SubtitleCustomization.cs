using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
[System.Serializable]
public struct EnumFontPair
{
    public SubtitleSettings.FontStyle id;
    public TMP_FontAsset fontNorm;
    public TMP_FontAsset fontOutlined;
}

public class SubtitleCustomization : MonoBehaviour
{
    [SerializeField]
    EnumFontPair[] fontKey;

    [Space]
    [SerializeField]
    SubtitleSettings settings;

    [Space]
    public Image letterBox;
    public TextMeshProUGUI textDisplayed;

    int prevOutline = -1;
    SubtitleSettings.FontStyle prevFont = (SubtitleSettings.FontStyle)(-1);

    private void Update()
    {
        UpdateUIStyle();
    }


    private void UpdateUIStyle()
    {
        SetFont();
        SetContrast();
    }


    void SetFont()
    {
        textDisplayed.fontSize = settings.fontSize;
    }

    void SetContrast()
    {
        bool outlined = settings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.OUTLINES);
        if (prevFont != settings.fontChoice || prevOutline != (outlined? 1:0))
        {
            if (outlined)
            {
                foreach (EnumFontPair e in fontKey)
                {
                    if (e.id == settings.fontChoice)
                    {
                        textDisplayed.font = e.fontOutlined;
                        break;
                    }
                }
            }
            else
            {

                foreach (EnumFontPair e in fontKey)
                {
                    if (e.id == settings.fontChoice)
                    {
                        textDisplayed.font = e.fontNorm;
                        break;
                    }
                }
            }

            prevFont = settings.fontChoice;
            prevOutline = (outlined ? 1 : 0);
        }

        if (settings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.LETTERBOX))
        {
            letterBox.color = new Color(0.0f, 0.0f, 0.0f, settings.letterBoxOpacity);
        }
        else
        {
            letterBox.color = Color.clear;
        }
    }
}
