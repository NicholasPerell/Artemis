using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Perell
[CreateAssetMenu(fileName = "New Artemis Narrative Data Point", menuName = "Artemis/Narrative Data Point")]
public class ArtemisNarrativeDataPoint : ScriptableObject
{ 
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
    ArtemisPreDictionaryDeliverySystem deliverySystem;
    [Space]
    [SerializeField]
    [Min(0)]
    int priorityValue;
    [SerializeField]
    ArtemisFlag[] flagsToMeet;
    [SerializeField]
    ArtemisFlag[] flagsToAvoid;
    [SerializeField]
    WhenAlreadyVoicePlaying whenAlreadyVoicePlaying = WhenAlreadyVoicePlaying.QUEUE;

    public void Rewrite(string _id, ArtemisPreDictionaryDeliverySystem _systemScriptable, int _priorityValue, ArtemisFlag[] _flagsToMeet, ArtemisFlag[] _flagsToAvoid, WhenAlreadyVoicePlaying _whenAlreadyVoicePlaying)
    {
        id = _id;
        deliverySystem = _systemScriptable;
        priorityValue = _priorityValue; 
        flagsToMeet = _flagsToMeet; 
        flagsToAvoid = _flagsToAvoid;
        whenAlreadyVoicePlaying = _whenAlreadyVoicePlaying;
    }

    public bool Deliver(ArtemisNarrativePriorityQueues sender)
    {
        return deliverySystem.ProcessDataPoint(this, sender);
    }

    public bool IsPriority()
    {
        return priorityValue > 0;
    }

    public int GetPriority()
    {
        return priorityValue;
    }

    public bool CondtionsMet()
    {
        for (int i = 0; i < flagsToMeet.Length; i++)
        {
            if (!flagsToMeet[i].GetValue())
            {
                return false;
            }
        }

        for (int i = 0; i < flagsToAvoid.Length; i++)
        {
            if (flagsToAvoid[i].GetValue())
            {
                return false;
            }
        }

        return true;
    }

    public WhenAlreadyVoicePlaying GetWhenBusyDescision()
    {
        return whenAlreadyVoicePlaying;
    }
}
