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
            //TODO: Allow for more than the global states to be loaded
            //TODO: Check for ANY type values
            FlagState[] globalStates = Goddess.instance.globallyLoadedStates;
            int[] startIndex = new int[globalStates.Length];
            for (int i = 0; i < startIndex.Length; i++)
            {
                startIndex[i] = 0;
            }

            FlagID targetId = FlagID.INVALID;
            Criterion targetCriterion;
            Flag targetFlag;
            bool located = false;
            for(int i = 0; i < rule.Count; i++)
            {
                targetId = rule[i].Key;
                targetCriterion = rule[i].Value;

                located = false;
                for (int j = 0; j < globalStates.Length && !located; j++)
                {
                    if(globalStates[j].flagsUsed.LinearSearch(targetId, ref startIndex[j], out targetFlag))
                    {
                        located = true;
                        if(!targetCriterion.Compare(targetFlag.GetValue()))
                        {
                            //The criterion was failed to be met!

                            if(name == "Test_Debug_000")
                            {
                                Debug.LogError("0 failed rule " + targetCriterion.GetStringRepresentation());
                            }
                            return false;
                        }
                    }
                }

                if(!located)
                {
                    //Flag not found in any state!

                    if (name == "Test_Debug_000")
                    {
                        Debug.LogError("0 couldn't find " + targetId.ToString());
                    }
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
