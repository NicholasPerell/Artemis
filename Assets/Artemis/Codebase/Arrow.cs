using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative Data Point", menuName = "Artemis/Narrative Data Point")]
    public class Arrow : ScriptableObject
    {
        public enum HowPriorityCalculated
        {
            SET_VALUE = 0,
            CRITERIA = 1,
            SUM = 2
        }

        public enum HowToHandleBusy
        {
            CANCEL = 0,
            QUEUE = 1,
            INTERRUPT = 2,
            INTERRUPT_CLEAR_QUEUE = 3,
            DELETE = 4,
            FRONT_OF_QUEUE = 5
        }

        [SerializeField]
        int id;
        [SerializeField]
        PreDictionaryFletcher fletcher;

        [Space]
        [SerializeField]
        [Min(0)]
        int priorityValue;
        [SerializeField]
        SortedStrictDictionary<FlagID, Criterion> rule;
        [SerializeField]
        HowToHandleBusy howToHandleBusy = HowToHandleBusy.QUEUE;
        [SerializeField]
        private HowPriorityCalculated howPriorityCalculated = HowPriorityCalculated.SET_VALUE;

#if UNITY_EDITOR
        public void Rewrite(int _id, PreDictionaryFletcher _systemScriptable, int _priorityValue, SortedStrictDictionary<FlagID,Criterion> _rule, HowToHandleBusy _howToHandleBusy, HowPriorityCalculated _howPriorityCalculated)
        {
            id = _id;
            fletcher = _systemScriptable;
            priorityValue = _priorityValue;
            rule = _rule;
            howToHandleBusy = _howToHandleBusy;
            howPriorityCalculated = _howPriorityCalculated;

            if(howPriorityCalculated == HowPriorityCalculated.CRITERIA)
            {
                priorityValue = rule.Count;
            }
            else if(howPriorityCalculated == HowPriorityCalculated.SUM)
            {
                priorityValue += rule.Count;
            }
        }
#endif

        public void IgnoreSuccessAttemptLoneDelivery()
        {
            bool success = AttemptLoneDelivery(null);
        }

        public bool AttemptLoneDelivery(FlagBundle[] importedStates, FlagID[] all = null)
        {
            bool success = false;

            if(CondtionsMet(importedStates, all))
            {
                success = Fire(null, importedStates, all);
            }

            return success;
        }

        public bool Fire(Archer sender, FlagBundle[] importedStates, FlagID[] all = null)
        {
            return fletcher.ProcessArrow(this, sender, importedStates, all);
        }

        public bool IsPriority()
        {
            return priorityValue > 0;
        }

        public int GetPriority()
        {
            return priorityValue;
        }

        public bool CondtionsMet(FlagBundle[] importedStates, FlagID[] all = null)
        {
            //Null checks
            if(importedStates == null)
            {
                importedStates = new FlagBundle[0];
            }
            if (all == null)
            {
                all = new FlagID[0];
            }
            Array.Sort(all);

            FlagBundle[] globalStates = Goddess.instance.globallyLoadedFlagBundles;
            int[] globalIndecies = new int[globalStates.Length];
            int[] importedIndecies = new int[importedStates.Length];
            Array.Fill(globalIndecies, 0);
            Array.Fill(importedIndecies, 0);

            FlagID targetID = FlagID.INVALID;
            Criterion targetCriterion;
            Flag targetFlag;
            bool located = false;
            int allIndex = 0;
            for(int i = 0; i < rule.Count; i++)
            {
                targetID = rule.GetTupleAtIndex(i).Key;
                targetCriterion = rule.GetTupleAtIndex(i).Value;

                located = false;
                for (; allIndex < all.Length && !located; allIndex++)
                {
                    int cmp = targetID.CompareTo(all[allIndex]);
                    if (cmp == 0)
                    {
                        located = true;
                    }
                    else if(cmp < 0)
                    {
                        break;
                    }
                }
                for (int j = 0; j < globalStates.Length && !located; j++)
                {
                    if(globalStates[j].flagsUsed.LinearSearch(targetID, ref globalIndecies[j], out targetFlag))
                    {
                        located = true;
                        if(!targetCriterion.Compare(targetFlag.GetValue()))
                        {
                            //The criterion was failed to be met!
                            return false;
                        }
                    }
                }
                for (int j = 0; j < importedStates.Length && !located; j++)
                {
                    if (importedStates[j].flagsUsed.LinearSearch(targetID, ref importedIndecies[j], out targetFlag))
                    {
                        located = true;
                        if (!targetCriterion.Compare(targetFlag.GetValue()))
                        {
                            //The criterion was failed to be met!
                            return false;
                        }
                    }
                }

                if (!located)
                {
                    //Flag not found in any state!
                    return false;
                }
            }

            return true;
        }

#if UNITY_EDITOR
        public string RecieveRuleStringRepresentation()
        {
            string rtn = "";

            for(int i = 0; i < rule.Count; i++)
            {
                rtn += rule.GetTupleAtIndex(i).Value.GetStringRepresentation();
                if(i < rule.Count - 1)
                {
                    rtn += "\n";
                }
            }

            return rtn;
        }
#endif

        public bool TryGetFlagEqualsValue(FlagID id, out float value)
        {
            bool success = false;
            value = -1;

            if(rule.HasKey(id))
            {
                Criterion criterion = rule[id];
                if(criterion.GetComparisonType() == CriterionComparisonType.EQUALS)
                {
                    value = criterion.getA();
                    success = true;
                }
            }

            return success;
        }

        public int GetRuleSize()
        {
            return rule.Count;
        }

        public HowToHandleBusy GetWhenBusyDescision()
        {
            return howToHandleBusy;
        }

        public PreDictionaryFletcher GetFletcher()
        {
            return fletcher;
        }

        public int GetArrowID()
        {
            return id;
        }

        public Type GetSymbolType()
        {
            return fletcher.GetSymbolType();
        }

        public HowPriorityCalculated GetHowPriorityCalculated()
        {
            return howPriorityCalculated;
        }
    }
}
