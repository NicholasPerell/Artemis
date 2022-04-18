using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Perell
[CreateAssetMenu(fileName = "New Email Full Collection", menuName = "Narrative/Narrative Data Point")]
public class NarrativeDataPoint : ScriptableObject
{ 
    // TO DO: find a way to have the delivery type become flexible for different delivery types

    public enum DeliveryType
    {
        EMAIL = 0,
        VOICE = 1
    }

    public enum WhenAlreadyVoicePlaying
    {
        CANCEL = 0,
        QUEUE = 1,
        INTERRUPT = 2,
        DELETE = 3,
        FRONT_OF_QUEUE = 4
    }

    [SerializeField]
    string id;
    [SerializeField]
    DeliveryType deliveryType;
    [Space]
    [SerializeField]
    [Min(0)]
    int priorityValue;
    [SerializeField]
    string[] flagsToMeet;
    [SerializeField]
    string[] flagsToAvoid;
    [Space]
    [Header ("Voice Only Data")]
    [SerializeField]
    TextAsset voiceSubtitles;
    // TO DO: Update the datapoints to have this as an enum or bool that can be changed
    //[SerializeField]
    WhenAlreadyVoicePlaying whenAlreadyVoicePlaying = WhenAlreadyVoicePlaying.QUEUE;

    public bool Deliver(NarrativeSystem system)
    {
        bool success = false;

        switch (deliveryType)
        {
            case DeliveryType.EMAIL:
                //FakeOSEventManager.Instance.AddNewEmailToTheList(id);
                success = true;
                break;
            case DeliveryType.VOICE:
                bool subtitleSent = system.SendSubtitleIn(voiceSubtitles);
                if (subtitleSent)
                {
                    //FindObjectOfType<ScriptUsageProgrammerSounds>(true).PlayDialogue(id);
                    success = true;
                }
                else
                {
                    switch (whenAlreadyVoicePlaying)
                    {
                        case WhenAlreadyVoicePlaying.CANCEL:
                            break;
                        case WhenAlreadyVoicePlaying.QUEUE:
                            //system.Enqueue(this);
                            break;
                        case WhenAlreadyVoicePlaying.INTERRUPT:
                            break;
                        case WhenAlreadyVoicePlaying.DELETE:
                            break;
                        case WhenAlreadyVoicePlaying.FRONT_OF_QUEUE:
                            break;
                        default:
                            break;
                    }
                        success = true;
                }
                break;
        }
        return success;
    }

    public bool IsPriority()
    {
        return priorityValue > 0;
    }

    public int GetPriority()
    {
        return priorityValue;
    }

    public void GetFlags(out string[] requiredTrue, out string[] requiredFalse)
    {
        requiredTrue = flagsToMeet;
        requiredFalse = flagsToAvoid;
    }
}
