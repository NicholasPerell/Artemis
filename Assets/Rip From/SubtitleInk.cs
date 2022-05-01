using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using Ink.Runtime;
using System;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
[System.Serializable]
public struct SpeakerColorPair
{
    public string speakerID;
    public Color color;
}

public class SubtitleInk : MonoBehaviour
{
    [SerializeField]
    ArtemisNarrativeSystem narrativeSystem;
    [SerializeField]
    SpeakerColorPair[] speakerToColorKey;

    [SerializeField]
    SubtitleSettings settings;

    [Space]
    [SerializeField]
    [Tooltip("Ink file ran at the very start of the scene. For testing purposes only—leave as null if you are not looking to test a specific subtitle.")]
    public TextAsset testStartingInkFile;

    [Space]
    public GameObject fullDisplay;
    public TextMeshProUGUI textDisplayed;

    [SerializeField]
    [Range(0,1)]
    int index;

    //Story currentDelivery;
    string currentSpeaker;
    float timer = 0;

    [SerializeField]
    List<ArtemisNarrativeDataPoint> queuedVoiceLines;

    private void Start()
    {
        queuedVoiceLines = new List<ArtemisNarrativeDataPoint>();
        //narrativeSystem.subtitleInk[index] = this;

        if (testStartingInkFile)
        {
            DeliverSubtitles(testStartingInkFile);
        }
    }

    public bool DeliverSubtitles(TextAsset inkFile)
    {
        //if (currentDelivery != null && (currentDelivery.canContinue || timer > 0))
        //{
        //    return false;
        //}

        timer = 0;
        currentSpeaker = "";
        SetColorToSpeaker(currentSpeaker);
        //currentDelivery = new Story(inkFile.text);
        fullDisplay.SetActive(true);
        NextBar();
        return true;
    }

    void Update()
    {
        RunTimer();
        //if(queuedVoiceLines.Count > 0 && !(currentDelivery != null && (currentDelivery.canContinue || timer > 0)))
        //{
        //    queuedVoiceLines[0].Deliver(narrativeSystem);
        //    queuedVoiceLines.RemoveAt(0);
        //}
    }

    void RunTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //if (currentDelivery.canContinue)
                //{
                //    NextBar();
                //}
                //else
                //{
                //    EndOfDelivery();
                //}
            }
        }
    }

    void NextBar()
    {
        //string currentBar = currentDelivery.Continue();
        ParseTags();
        //DisplayBar(currentBar);
    }

    void EndOfDelivery()
    {
        currentSpeaker = "";
        textDisplayed.text = "";
        textDisplayed.color = Color.white;
        fullDisplay.SetActive(false);
        timer = 0;
    }

    void ParseTags()
    {
        //foreach (string e in currentDelivery.currentTags)
        //{
        //    //[0]: information type
        //    //[1]: data assigned
        //    string[] tag = e.Split(' ');

        //    switch (tag[0])
        //    {
        //        case "speaker":
        //            currentSpeaker = tag[1];
        //            if (settings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.COLORS))
        //            {
        //                SetColorToSpeaker(currentSpeaker);
        //            }
        //            break;
        //        case "time":
        //            timer += (float)double.Parse(tag[1]);
        //            if(timer <= 0)
        //            {
        //                NextBar();
        //            }
        //            break;
        //    }
        //}
    }

    void DisplayBar(string currentBar)
    {
        string tmp = "";

        if (settings.speakerIndicators.HasFlag(SubtitleSettings.SpeakerIndicators.NAME_TAGS))
        {
            tmp += currentSpeaker + ": ";
        }

        tmp += currentBar.Replace('^', '\n');

        textDisplayed.text = tmp;
    }

    void SetColorToSpeaker(string id)
    {
        textDisplayed.color = Color.white;
        foreach (SpeakerColorPair e in speakerToColorKey)
        {
            if (e.speakerID == id)
            {
                textDisplayed.color = e.color;
                break;
            }
        }
    }

    public void Enqueue(ArtemisNarrativeDataPoint data)
    {
        queuedVoiceLines.Add(data);
    }
}