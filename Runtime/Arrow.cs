using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Generated;
using Perell.Artemis.Debugging;
using Perell.Artemis.Saving;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Perell.Artemis
{
    public class Arrow : ScriptableObject, IBinaryReadWriteable
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
            ArtemisDebug.Instance.OpenReportLine(name + " ConditionsMet");

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

            FlagBundle[] globalStates = Goddess.instance.globallyLoadedFlagBundles.ToArray();
            int[] globalIndecies = new int[globalStates.Length];
            int[] importedIndecies = new int[importedStates.Length];
            Array.Fill(globalIndecies, 0);
            Array.Fill(importedIndecies, 0);

            ArtemisDebug.Instance.Report("globalStates: ").ReportLine(globalStates)
                .Report("importedStates: ").ReportLine(importedStates)
                .Report("all: ").ReportLine(all);

            FlagID targetID = FlagID.INVALID;
            Criterion targetCriterion;
            Flag targetFlag;
            bool located = false;
            int allIndex = 0;
            for(int i = 0; i < rule.Count; i++)
            {
                targetID = rule.GetTupleAtIndex(i).Key;
                targetCriterion = rule.GetTupleAtIndex(i).Value;
                ArtemisDebug.Instance.Report("Rule ").Report(i).Report(": ").ReportLine(targetCriterion.GetStringRepresentation());

                located = false;
                for (; allIndex < all.Length && !located; allIndex++)
                {
                    int cmp = targetID.CompareTo(all[allIndex]);
                    if (cmp == 0)
                    {
                        ArtemisDebug.Instance.Report(targetID).ReportLine(" located in all.");
                        located = true;
                    }
                    else if(cmp < 0)
                    {
                        break;
                    }
                }
                for (int j = 0; j < globalStates.Length && !located; j++)
                {
                    ArtemisDebug.Instance.Report("Linear Searching ").Report(globalStates[j]).Report(" starting at index ").Report(globalIndecies[j]).Report(" (").Report(globalStates[j].flagsUsed.GetTupleAtIndex(globalIndecies[j]).Key).ReportLine(")");
                    if (globalStates[j].flagsUsed.LinearSearch(targetID, ref globalIndecies[j], out targetFlag))
                    {
                        located = true;
                        if(!targetCriterion.Compare(targetFlag.GetValue()))
                        {
                            //The criterion was failed to be met!
                            ArtemisDebug.Instance.Report("The criterion (").Report(targetCriterion.GetStringRepresentation()).Report(") was failed to be met by ").Report(targetFlag).Report(" (").Report(targetFlag.GetValue()).Report(")");
                            ArtemisDebug.Instance.CloseReport();
                            return false;
                        }
                    }
                    ArtemisDebug.Instance.Report("Linear Searching index now set to ").ReportLine(globalIndecies[j]);
                }
                for (int j = 0; j < importedStates.Length && !located; j++)
                {
                    if (importedStates[j].flagsUsed.LinearSearch(targetID, ref importedIndecies[j], out targetFlag))
                    {
                        located = true;
                        if (!targetCriterion.Compare(targetFlag.GetValue()))
                        {
                            //The criterion was failed to be met!
                            ArtemisDebug.Instance.Report("The criterion (").Report(targetCriterion.GetStringRepresentation()).Report(") was failed to be met by ").Report(targetFlag).Report(" (").Report(targetFlag.GetValue()).Report(")");
                            ArtemisDebug.Instance.CloseReport();
                            return false;
                        }
                    }
                }

                if (!located)
                {
                    //Flag not found in any state!
                    ArtemisDebug.Instance.Report(targetID).Report(" Flag not found in any state!");
                    ArtemisDebug.Instance.CloseReport();
                    return false;
                }
            }

            ArtemisDebug.Instance.CloseReport();
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
            if(!fletcher)
            {
                return null;
            }

            return fletcher.GetSymbolType();
        }

        public HowPriorityCalculated GetHowPriorityCalculated()
        {
            return howPriorityCalculated;
        }

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            binaryWriter.Write(this.name);

            binaryWriter.Write(this.fletcher.name);

            binaryWriter.Write(id);
            binaryWriter.Write(priorityValue);

            SortedStrictDictionary<FlagID, Criterion>.Tuple tuple;
            binaryWriter.Write(rule.Count);
            for(int i = 0; i < rule.Count; i++)
            {
                tuple = rule.GetTupleAtIndex(i);
                binaryWriter.Write((int)tuple.Key);
                tuple.Value.WriteToBinary(ref binaryWriter);
            }

            binaryWriter.Write((int)howToHandleBusy);
            binaryWriter.Write((int)howPriorityCalculated);
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            this.name = binaryReader.ReadString();

            string fletcherName = binaryReader.ReadString();
            PreDictionaryFletcher[] fletchers = Resources.FindObjectsOfTypeAll<PreDictionaryFletcher>();
            
            for (int i = 0; i < fletchers.Length; i++)
            {
                if (fletchers[i].name == fletcherName)
                {
                    fletcher = fletchers[i];
                    break;
                }
            }

            id = binaryReader.ReadInt32();
            priorityValue = binaryReader.ReadInt32();

            rule ??= new SortedStrictDictionary<FlagID, Criterion>();
            rule.Clear();
            FlagID flagID;
            Criterion criterion = new Criterion();
            int ruleCount = binaryReader.ReadInt32();
            for (int i = 0; i < ruleCount; i++)
            {
                flagID = (FlagID)binaryReader.ReadInt32();
                criterion.ReadFromBinary(ref binaryReader);
                rule.Add(flagID, criterion);
            }

            howToHandleBusy = (HowToHandleBusy)binaryReader.ReadInt32();
            howPriorityCalculated = (HowPriorityCalculated)binaryReader.ReadInt32();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
