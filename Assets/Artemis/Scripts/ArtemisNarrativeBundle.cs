using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Perell
[CreateAssetMenu(fileName = "New Artemis Narrative Bundle", menuName = "Artemis/Narrative Bundle")]
public class ArtemisNarrativeBundle : ScriptableObject
{
    [SerializeField]
    ArtemisNarrativeDataPoint[] narrativeDataPoints;
    [Space]
    [SerializeField]
    ArtemisNarrativePriorityQueues sendTo;
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
