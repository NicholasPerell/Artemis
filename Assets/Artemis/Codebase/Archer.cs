using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Artemis
{
    public class Archer : ScriptableObject
    {
        public struct BundleLog
        {
            public ArrowBundle bundle;
            public bool isAdding;

            public BundleLog(ArrowBundle _bundle, bool _isAdding)
            {
                bundle = _bundle;
                isAdding = _isAdding;
            }
        }

        public enum ChooseSamePriority
        {
            QUEUE,
            STACK,
            RANDOM
        }

        //New Listings
        [SerializeField]
        List<Arrow> overallData = new List<Arrow>();
        [SerializeField]
        List<FlagID> partitioningFlags = new List<FlagID>();
        [SerializeField]
        List<FlagID> tempPartitioningFlags = new List<FlagID>();
        [SerializeField]
        SortedStrictDictionary<string, List<Arrow>> partitionedData = new SortedStrictDictionary<string, List<Arrow>>();

        //TODO: Scrap
        [HideInInspector]
        PriorityQueue<Arrow> priorityQueue; //for reactivity and recency. Priority value > 0
        [HideInInspector]
        List<Arrow> generalPool; //for when there's nothing w/ priority. Priority value == 0

        //When Empty
        [HideInInspector]
        public bool loops;
        [HideInInspector]
        public bool includeBundlesInLoop;
        [HideInInspector]
        public bool includeHigherPrioritiesInLoop;

        //Delete Arrows?
        [HideInInspector]
        public bool discardArrowsAfterUse = true;

        //Non-Value Priorities
        [HideInInspector]
        Archer.ChooseSamePriority chooseSamePriority;
        [HideInInspector]
        bool recencyBias;

        //Init Contents
        [SerializeField]
        public List<Arrow> defaultContents;

        //Bundles
        [HideInInspector]
        public ArrowBundle tempArrowBundle;
        [HideInInspector]
        private List<BundleLog> bundleHistory = new List<BundleLog>();

        public bool IsEmpty { get { return overallData.Count == 0; } }

        public void Init()
        {
            Refresh(true,false);
        }

        private void Refresh(bool includeNonZeroPriority, bool includeBundles)
        {
            overallData = new List<Arrow>();
            partitionedData = new SortedStrictDictionary<string, List<Arrow>>();

            foreach (Arrow arrow in defaultContents)
            {
                if(arrow.GetPriority() == 0 || includeNonZeroPriority)
                RecieveDataPoint(arrow);
            }

            if (includeBundles)
            {
                foreach(BundleLog log in bundleHistory)
                {
                    if(log.isAdding)
                    {
                        DumpArrowsOfBundle(log.bundle, includeNonZeroPriority);
                    }
                    else
                    {
                        DropArrowsOfBundle(log.bundle);
                    }
                }
            }
            else
            {
                bundleHistory.Clear();
            }
        }

        public void SetToLoopedState()
        {
            Refresh(includeHigherPrioritiesInLoop, includeBundlesInLoop);
        }
        
        public void ReturnDataPoint(Arrow dataPoint)
        {
            if (discardArrowsAfterUse)
            {
                RecieveDataPoint(dataPoint, true);
            }
        }

        public void RecieveDataPoint(Arrow dataPoint, bool returningArrow = false)
        {
            //Overall Data
            InsertDataPointIntoList(dataPoint, overallData, returningArrow);

            //Partitioned Data
            if(partitioningFlags.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                float value;
                foreach (FlagID id in partitioningFlags)
                {
                    dataPoint.TryGetFlagEqualsValue(id, out value);
                    stringBuilder.Append(value);
                    stringBuilder.Append('#');
                }
                string key = stringBuilder.ToString();
                if (!partitionedData.HasKey(key))
                {
                    partitionedData.Add(key, new List<Arrow>());
                }
                InsertDataPointIntoList(dataPoint, partitionedData[key], returningArrow);
            }
        }

        private void InsertDataPointIntoList(Arrow dataPoint, List<Arrow> list, bool returningArrow)
        {
            if (list.Count != 0)
            {
                int i;
                if (dataPoint.IsPriority())
                {
                    for (i = 0; i < list.Count; i++)
                    {
                        bool insertable;
                        if (recencyBias || returningArrow)
                        {
                            insertable = list[i].GetPriority() <= dataPoint.GetPriority();
                        }
                        else
                        {
                            insertable = list[i].GetPriority() < dataPoint.GetPriority();
                        }

                        if (insertable)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (i = 0; i < list.Count; i++)
                    {
                        if (!list[i].IsPriority())
                        {
                            i = UnityEngine.Random.Range(i, list.Count + 1);
                            break;
                        }
                    }
                }
                list.Insert(i, dataPoint);
            }
            else
            {
                list.Add(dataPoint);
            }
        }

        public bool AttemptDelivery(FlagBundle[] importedStates)
        {
            //TODO:
            // - Use PartitioningFlags & PartitionedData
            // - Allow ANY/ALL values

            bool success = false;

            if (!IsEmpty)
            {
                if(importedStates == null)
                {
                    importedStates = new FlagBundle[0];
                }

                List<Arrow> bucketToUse = overallData;

                if(partitioningFlags.Count > 0)
                {
                    //TODO: Determine the bucket using the flags
                    FlagBundle[] globalStates = Goddess.instance.globallyLoadedFlagBundles;
                    int[] globalIndecies = new int[globalStates.Length];
                    int[] importedIndecies = new int[importedStates.Length];
                    Array.Fill(globalIndecies, 0);
                    Array.Fill(importedIndecies, 0);

                    StringBuilder stringBuilder = new StringBuilder();
                    float value = -1;
                    bool located;
                    Flag targetFlag;
                    foreach (FlagID targetId in partitioningFlags)
                    {
                        located = false;
                        for (int j = 0; j < globalStates.Length && !located; j++)
                        {
                            if (globalStates[j].flagsUsed.LinearSearch(targetId, ref globalIndecies[j], out targetFlag))
                            {
                                located = true;
                                value = targetFlag.GetValue();
                            }
                        }
                        for (int j = 0; j < importedStates.Length && !located; j++)
                        {
                            if (importedStates[j].flagsUsed.LinearSearch(targetId, ref importedIndecies[j], out targetFlag))
                            {
                                located = true;
                                value = targetFlag.GetValue();
                            }
                        }

                        if (!located)
                        {
                            value = -1;
                        }
                        stringBuilder.Append(value);
                        stringBuilder.Append('#');
                    }
                    string key = stringBuilder.ToString();
                    if (partitionedData.HasKey(key))
                    {
                        bucketToUse = partitionedData[key];
                    }
                    else
                    {
                        bucketToUse = new List<Arrow>();
                    }
                }

                bool flagMeetsConditions;
                int i = 0;
                if (chooseSamePriority == ChooseSamePriority.RANDOM) //RANDOM
                {
                    List<int> foundArrowIndecies = new List<int>();
                    int highestPriorityFound = -1;
                    bool anyArrowFound = false;
                    for (; i < bucketToUse.Count; i++)
                    {
                        if (!bucketToUse[i].IsPriority())
                        {
                            break;
                        }
                        if (anyArrowFound && highestPriorityFound > bucketToUse[i].GetPriority())
                        {
                            break;
                        }
                        flagMeetsConditions = bucketToUse[i].CondtionsMet(importedStates);
                        if (flagMeetsConditions)
                        {
                            foundArrowIndecies.Add(i);
                            highestPriorityFound = bucketToUse[i].GetPriority();
                            anyArrowFound = true;
                        }
                    }

                    if (anyArrowFound)
                    {
                        int usedArrowIndex = foundArrowIndecies[UnityEngine.Random.Range(0, foundArrowIndecies.Count)];
                        success = bucketToUse[usedArrowIndex].Fire(this, importedStates);
                        if (success && discardArrowsAfterUse)
                        {
                            if (!ReferenceEquals(bucketToUse, overallData))
                            {
                                overallData.Remove(bucketToUse[usedArrowIndex]);
                            }
                            bucketToUse.RemoveAt(usedArrowIndex);
                        }
                    }
                }
                if ((chooseSamePriority != ChooseSamePriority.RANDOM) //QUEUE, STACK...
                    || (!success && i < bucketToUse.Count && !bucketToUse[i].IsPriority())) //...or RANDOM has reached the priority 0 options, which are already jumbled up
                {
                    for (; i < bucketToUse.Count; i++)
                    {
                        flagMeetsConditions = bucketToUse[i].CondtionsMet(importedStates);
                        if (flagMeetsConditions)
                        {
                            success = bucketToUse[i].Fire(this, importedStates);
                            if (success && discardArrowsAfterUse)
                            {
                                if (!ReferenceEquals(bucketToUse, overallData))
                                {
                                    overallData.Remove(bucketToUse[i]);
                                }
                                bucketToUse.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }

            if (IsEmpty && loops)
            {
                SetToLoopedState();
            }

            return success;
        }

        public void IgnoreSuccessAttemptDelivery()
        {
            bool temp = AttemptDelivery(null);
        }

        public void SetChoosingSamePriority(ChooseSamePriority _chooseSamePriority)
        {
            chooseSamePriority = _chooseSamePriority;
            if((_chooseSamePriority == ChooseSamePriority.QUEUE && recencyBias)
                || (_chooseSamePriority == ChooseSamePriority.STACK && !recencyBias))
            {
                FlipRecencyBias();
            }
        }

        public void Repartition()
        {
            partitionedData = new SortedStrictDictionary<string, List<Arrow>>();

            //Validate flags
            partitioningFlags = new List<FlagID>(tempPartitioningFlags.Distinct().ToList());
            partitioningFlags.Remove(FlagID.INVALID);
            for (int i = partitioningFlags.Count - 1; i >= 0; i--)
            {
                if(Goddess.instance.GetFlagValueType(partitioningFlags[i]) != Flag.ValueType.SYMBOL)
                {
                    partitioningFlags.RemoveAt(i);
                }
            }
            partitioningFlags.Sort();

            if (partitioningFlags.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                float value;
                string key;
                foreach (Arrow arrow in overallData)
                {
                    stringBuilder.Clear();
                    foreach (FlagID id in partitioningFlags)
                    {
                        arrow.TryGetFlagEqualsValue(id, out value);
                        stringBuilder.Append((int)value);
                        stringBuilder.Append('#');
                    }
                    key = stringBuilder.ToString();

                    if (!partitionedData.HasKey(key))
                    {
                        partitionedData.Add(key, new List<Arrow>());
                    }

                    partitionedData[key].Add(arrow);
                }
            }

            tempPartitioningFlags = new List<FlagID>(partitioningFlags);
        }

        private void FlipRecencyBias()
        {
            recencyBias = !recencyBias;

            FlipArrowList(overallData);
            for(int i = 0; i < partitionedData.Count; i++)
            {
                FlipArrowList(partitionedData[i].Value);
            }
        }

        private void FlipArrowList(List<Arrow> toFlip)
        {
            if(toFlip.Count <= 1)
            {
                return;
            }

            int targetIndex = 0;
            int targetPriority = toFlip[0].GetPriority();
            int tempPriority;
            int i;
            for (i = 1; i < toFlip.Count; i++)
            {
                tempPriority = toFlip[i].GetPriority();
                if (targetPriority != tempPriority)
                {
                    toFlip.Reverse(targetIndex, i - targetIndex);
                    targetIndex = i;
                    targetPriority = toFlip[i].GetPriority();
                }
            }
            toFlip.Reverse(targetIndex, i - targetIndex);
        }

        public ChooseSamePriority GetChoosingSamePriority()
        {
            return chooseSamePriority;
        }

        private void CleanBundleList()
        {
            for (int i = bundleHistory.Count - 1; i >= 0; i--)
            {
                if (bundleHistory[i].bundle == null)
                {
                    bundleHistory.RemoveAt(i);
                }
            }
        }

        public void DumpBundle(ArrowBundle toDump)
        {
            DumpArrowsOfBundle(toDump);
            LogBundleHistory(toDump, true);
        }

        private void DumpArrowsOfBundle(ArrowBundle toDump, bool includeNonZeroPriority = true)
        {
            if(toDump == null)
            {
                return;
            }

            foreach (Arrow e in toDump.arrows)
            {
                if (includeNonZeroPriority || !e.IsPriority())
                {
                    RecieveDataPoint(e);
                }
            }
        }

        public void DropBundle(ArrowBundle toDrop)
        {
            DropArrowsOfBundle(toDrop);
            LogBundleHistory(toDrop, false);
        }

        //Should it remove all instances of that arrow?
        private void DropArrowsOfBundle(ArrowBundle toDrop)
        {
            if (toDrop == null)
            {
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();
            float value;
            foreach (Arrow e in toDrop.arrows)
            {
                if(overallData.Remove(e))
                {
                    stringBuilder.Clear();
                    foreach (FlagID id in partitioningFlags)
                    {
                        e.TryGetFlagEqualsValue(id, out value);
                        stringBuilder.Append(value);
                        stringBuilder.Append('#');
                    }
                    string key = stringBuilder.ToString();
                    if (partitionedData.HasKey(key))
                    {
                        partitionedData[key].Remove(e);
                    }
                }
            }
        }

        private void LogBundleHistory(ArrowBundle bundle, bool isAdding)
        {
            if (bundleHistory == null)
            {
                bundleHistory = new List<BundleLog>();
            }

            if (bundle != null)
            {
                bool inverseExists = false;

                for (int i = 0; i < bundleHistory.Count; i++)
                {
                    if (bundleHistory[i].bundle == bundle && bundleHistory[i].isAdding != isAdding)
                    {
                        inverseExists = true;
                        bundleHistory.RemoveAt(i);
                        break;
                    }
                }

                if (!inverseExists)
                {
                    bundleHistory.Add(new BundleLog(bundle, isAdding));
                }
            }
            else
            {
                CleanBundleList();
            }
        }

        [ContextMenu("Clear Bundle History")]
        private void ClearBundleHistory()
        {
            bundleHistory.Clear();
        }

        public List<BundleLog> GetBundleHistory()
        {
            if (bundleHistory == null)
            {
                bundleHistory = new List<BundleLog>();
            }

            return bundleHistory;
        }
    }
}