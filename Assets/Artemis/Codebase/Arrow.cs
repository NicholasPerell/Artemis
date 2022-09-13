using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative Data Point", menuName = "Artemis/Narrative Data Point")]
    public class Arrow : ScriptableObject
    {
        public enum HowToHandleBusy
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
        PreDictionaryFletcher deliverySystem;
        [Space]
        [SerializeField]
        [Min(0)]
        int priorityValue;
        [SerializeField]
        Flag[] flagsToMeet;
        [SerializeField]
        Flag[] flagsToAvoid;
        [SerializeField]
        HowToHandleBusy howToHandleBusy = HowToHandleBusy.QUEUE;

        public void Rewrite(string _id, PreDictionaryFletcher _systemScriptable, int _priorityValue, Flag[] _flagsToMeet, Flag[] _flagsToAvoid, HowToHandleBusy _howToHandleBusy)
        {
            id = _id;
            deliverySystem = _systemScriptable;
            priorityValue = _priorityValue;
            flagsToMeet = _flagsToMeet;
            flagsToAvoid = _flagsToAvoid;
            howToHandleBusy = _howToHandleBusy;
        }

        public bool Deliver(Archer sender)
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

        public HowToHandleBusy GetWhenBusyDescision()
        {
            return howToHandleBusy;
        }
    }
}
