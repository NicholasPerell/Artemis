using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Perell
[CreateAssetMenu(fileName = "New Narrative Bundle", menuName = "Narrative/Narrative Bundle")]
public class NarrativeBundle : ScriptableObject
{
    [SerializeField]
    NarrativeDataPoint[] narrativeDataPoints;
    [Space]
    [SerializeField]
    NarrativePriorityQueues sendTo;
    bool sentOut = false;

    public void Init()
    {
        sentOut = false;
    }

    public bool SendBundle()
    {
        bool success = !sentOut;
        if(success)
        {
            sendTo.RecieveBundle(narrativeDataPoints);
            sentOut = true;
        }
        return success;
    }
}
