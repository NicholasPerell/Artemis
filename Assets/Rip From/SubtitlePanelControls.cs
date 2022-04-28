using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
public class SubtitlePanelControls : MonoBehaviour
{
    [SerializeField]
    SubtitleSettings currentSettings;

    [Space]
    [SerializeField]
    SubtitleSettings artPreset;
    [SerializeField]
    SubtitleSettings clearPreset;

    [Space]
    [SerializeField]
    Sprite checkedBoxSprite;
    [SerializeField]
    Sprite uncheckedBoxSprite;

    [Space]
    [SerializeField]
    Slider sizeSlider;
    [SerializeField]
    Slider opacitySlider;
    [SerializeField]
    GameObject letterboxOpacityPanel;
    [SerializeField]
    Image outlineCheckImage;
    [SerializeField]
    Image letterBoxCheckImage;
    [SerializeField]
    Image colorCodeCheckImage;
    [SerializeField]
    Image nameTagCheckImage;
    [SerializeField]
    Button[] fontChoiceButtons;
    [SerializeField]
    SubtitleInk subtitlePreview;

    [Space]
    [SerializeField]
    Color checkedColor;
    [SerializeField]
    Color uncheckedColor;


    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateVisualsCompletely();
    }

    void SetToPreset(ref SubtitleSettings setTo)
    {
        currentSettings.contrastHandling = setTo.contrastHandling;
        currentSettings.fontChoice = setTo.fontChoice;
        currentSettings.fontSize = setTo.fontSize;
        currentSettings.letterBoxOpacity = setTo.letterBoxOpacity;
        currentSettings.speakerIndicators = setTo.speakerIndicators;
        UpdateVisualsCompletely();
    }

    public void SetToArtPreset()
    {
        SetToPreset(ref artPreset);
    }

    public void SetToClearPreset()
    {
        SetToPreset(ref clearPreset);
    }

    public void SetOpacity()
    {
        currentSettings.letterBoxOpacity = opacitySlider.value;
        UpdateSubtitle();
    }

    public void SetFontSize()
    {
        currentSettings.fontSize = sizeSlider.value;
        UpdateSubtitle();
    }

    public void SetFontChoice(int style)
    {
        currentSettings.fontChoice = (SubtitleSettings.FontStyle)style;
        fontChoiceButtons[style].interactable = false;
        fontChoiceButtons[style == 0 ? 1 : 0].interactable = true;
        UpdateSubtitle();
    }

    public void ToggleOutline()
    {
        bool intended = !currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.OUTLINES);

        if(intended)
        {
            currentSettings.contrastHandling |= SubtitleSettings.TextContrast.OUTLINES;
        }
        else
        {
            currentSettings.contrastHandling &= SubtitleSettings.TextContrast.LETTERBOX;
        }

        outlineCheckImage.sprite = intended ? checkedBoxSprite : uncheckedBoxSprite;
        outlineCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = intended ? checkedColor : uncheckedColor;
        UpdateSubtitle();
    }

    public void ToggleLetterBox()
    {
        bool intended = !currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.LETTERBOX);

        if (intended)
        {
            currentSettings.contrastHandling |= SubtitleSettings.TextContrast.LETTERBOX;
        }
        else
        {
            currentSettings.contrastHandling &= SubtitleSettings.TextContrast.OUTLINES;
        }

        letterBoxCheckImage.sprite = intended ? checkedBoxSprite : uncheckedBoxSprite;
        letterBoxCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = intended ? checkedColor : uncheckedColor;
        letterboxOpacityPanel.SetActive(intended);
        UpdateSubtitle();
    }

    public void ToggleColorCoding()
    {
        bool intended = !currentSettings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.COLORS);

        if (intended)
        {
            currentSettings.speakerIndicators |= SubtitleSettings.SpeakerIndicators.COLORS;
        }
        else
        {
            currentSettings.speakerIndicators &= SubtitleSettings.SpeakerIndicators.NAME_TAGS;
        }

        colorCodeCheckImage.sprite = intended ? checkedBoxSprite : uncheckedBoxSprite;
        colorCodeCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = intended ? checkedColor : uncheckedColor;
        UpdateSubtitle();
    }

    public void ToggleNameTags()
    {
        bool intended = !currentSettings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.NAME_TAGS);

        if (intended)
        {
            currentSettings.speakerIndicators |= SubtitleSettings.SpeakerIndicators.NAME_TAGS;
        }
        else
        {
            currentSettings.speakerIndicators &= SubtitleSettings.SpeakerIndicators.COLORS;
        }

        nameTagCheckImage.sprite = intended ? checkedBoxSprite : uncheckedBoxSprite;
        nameTagCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = intended ? checkedColor : uncheckedColor;
        UpdateSubtitle();
    }

    void UpdateVisualsCompletely()
    {
        outlineCheckImage.sprite = currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.OUTLINES) ? checkedBoxSprite : uncheckedBoxSprite;
        outlineCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.OUTLINES) ? checkedColor : uncheckedColor;
        letterBoxCheckImage.sprite = currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.LETTERBOX) ? checkedBoxSprite : uncheckedBoxSprite;
        letterBoxCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.LETTERBOX) ? checkedColor : uncheckedColor;
        colorCodeCheckImage.sprite = currentSettings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.COLORS) ? checkedBoxSprite : uncheckedBoxSprite;
        colorCodeCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = currentSettings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.COLORS) ? checkedColor : uncheckedColor;
        nameTagCheckImage.sprite = currentSettings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.NAME_TAGS) ? checkedBoxSprite : uncheckedBoxSprite;
        nameTagCheckImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().color = currentSettings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.NAME_TAGS) ? checkedColor : uncheckedColor;

        letterboxOpacityPanel.SetActive(currentSettings.contrastHandling.HasFlag(SubtitleSettings.TextContrast.LETTERBOX));

        sizeSlider.value = currentSettings.fontSize;
        opacitySlider.value = currentSettings.letterBoxOpacity;

        int style = (int)currentSettings.fontChoice;
        fontChoiceButtons[style].interactable = false;
        fontChoiceButtons[style == 0 ? 1 : 0].interactable = true;
        UpdateSubtitle();
    }

    private void UpdateSubtitle()
    {
        subtitlePreview.DeliverSubtitles(subtitlePreview.testStartingInkFile);
    }
}
