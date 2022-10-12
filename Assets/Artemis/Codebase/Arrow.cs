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
        SortedStrictDictionary<FlagID, Criterion> rule;
        [SerializeField]
        HowToHandleBusy howToHandleBusy = HowToHandleBusy.QUEUE;

        public void Rewrite(string _id, PreDictionaryFletcher _systemScriptable, int _priorityValue, SortedStrictDictionary<FlagID,Criterion> _rule, HowToHandleBusy _howToHandleBusy)
        {
            id = _id;
            deliverySystem = _systemScriptable;
            priorityValue = _priorityValue;
            rule = _rule;
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
            SortedStrictDictionary<FlagID, Flag> everythingQueuery = Goddess.instance.idToFlag;
            int startIndex = 0;
            FlagID targetId = FlagID.INVALID;
            Criterion targetCriterion;
            Flag targetFlag;

            for(int i = 0; i < rule.Count; i++)
            {
                targetId = rule[i].Key;
                targetCriterion = rule[i].Value;

                if(!everythingQueuery.LinearSearch(targetId, ref startIndex, out targetFlag)
                    || !targetCriterion.Compare(targetFlag.GetValue()))
                {
                    //Flag either not found or the criterion was failed to be met!
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
