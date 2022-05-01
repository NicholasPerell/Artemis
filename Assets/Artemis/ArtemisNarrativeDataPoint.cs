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
    ArtemisPreDictionarySystem deliverySystem;
    [Space]
    [SerializeField]
    [Min(0)]
    int priorityValue;
    [SerializeField]
    string[] flagsToMeet;
    [SerializeField]
    string[] flagsToAvoid;
    [SerializeField]
    WhenAlreadyVoicePlaying whenAlreadyVoicePlaying = WhenAlreadyVoicePlaying.QUEUE;

    public void Rewrite(string _id, ArtemisPreDictionarySystem _systemScriptable, int _priorityValue, string[] _flagsToMeet, string[] _flagsToAvoid, WhenAlreadyVoicePlaying _whenAlreadyVoicePlaying)
    {
        id = _id;
        deliverySystem = _systemScriptable;
        priorityValue = _priorityValue; 
        flagsToMeet = _flagsToMeet; 
        flagsToAvoid = _flagsToAvoid;
        whenAlreadyVoicePlaying = _whenAlreadyVoicePlaying;
    }

    [ContextMenu("deliver")]
    public void nah()
    {
        Debug.Log(deliverySystem.GetType());

        deliverySystem.Send();
    }

    public bool Deliver(ArtemisNarrativeSystem system)
    {
        bool success = false;

        

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
