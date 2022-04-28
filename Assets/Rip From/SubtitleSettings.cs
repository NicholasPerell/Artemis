using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
[System.Serializable]
[CreateAssetMenu(menuName = "Subtitle Settings", fileName = "New Subtitle Settings")]
public class SubtitleSettings : ScriptableObject
{
    [System.Flags]
    public enum SpeakerIndicators
    {
        COLORS = 1,
        NAME_TAGS = 2
    }
    public SpeakerIndicators speakerIndicators;

    [System.Flags]
    public enum TextContrast
    {
        OUTLINES = 1,
        LETTERBOX = 2
    }
    public TextContrast contrastHandling;

    [Range(0.1f, 1.0f)]
    public float letterBoxOpacity = 1.0f;

    public enum FontStyle
    {
        ARTISTIC = 0,
        CLEAR = 1
    }
    [Space]
    public FontStyle fontChoice;
    [Range(23.0f, 92.0f)]
    public float fontSize = 46.0f;
}
