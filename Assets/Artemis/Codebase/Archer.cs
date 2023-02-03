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

        [System.Serializable]
        private struct ComparableIntArray : System.IComparable
        {
            [SerializeField]
            private int[] mArray;

            public ComparableIntArray(int[] array)
            {
                mArray = array;
            }

            private int CompareToSame(ComparableIntArray obj)
            {
                if(mArray.Length.CompareTo(obj.mArray.Length) != 0)
                {
                    return mArray.Length.CompareTo(obj.mArray.Length);
                }

                for(int i = 0; i < mArray.Length; i++)
                {
                    if (mArray[i].CompareTo(obj.mArray[i]) != 0)
                    {
                        return mArray[i].CompareTo(obj.mArray[i]);
                    }
                }

                return 0;
            }

            public int CompareTo(object obj)
            {
                return CompareToSame((ComparableIntArray)obj);
            }
        }

        //New Listings
        [SerializeField]
        List<Arrow> overallData = new List<Arrow>();
        [SerializeField]
        List<FlagID> partitioningFlags = new List<FlagID>();
        [SerializeField]
        List<FlagID> tempPartitioningFlags = new List<FlagID>();
        [SerializeField]
        SortedStrictDictionary<ComparableIntArray, OrderedArrowList> partitionedData = new SortedStrictDictionary<ComparableIntArray, OrderedArrowList>();

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

        [SerializeField]
        private uint mInsertionOrder;

        [System.Serializable]
        private struct OrderedArrowList
        {
            public List<Arrow> mArrows;
            public List<uint> mOrder;

            public static OrderedArrowList Init()
            {
                OrderedArrowList tmp;
                tmp.mArrows = new List<Arrow>();
                tmp.mOrder = new List<uint>();
                return tmp;
            }
        }

        public bool IsEmpty { get { return overallData.Count == 0; } }

        public void Init()
        {
            Refresh(true,false);
        }

        private void Refresh(bool includeNonZeroPriority, bool includeBundles)
        {
            mInsertionOrder = 0;
            overallData = new List<Arrow>();
            partitionedData = new SortedStrictDictionary<ComparableIntArray, OrderedArrowList>();

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
                float value;
                int[] array = new int[partitioningFlags.Count];
                for (int i = 0; i < partitioningFlags.Count; i++)
                {
                    dataPoint.TryGetFlagEqualsValue(partitioningFlags[i], out value);
                    array[i] = (int)value;
                }
                ComparableIntArray key = new ComparableIntArray(array);
                if (!partitionedData.HasKey(key))
                {
                    partitionedData.Add(key, OrderedArrowList.Init());
                }
                OrderedArrowList bucket = partitionedData[key];
                InsertDataPointIntoList(dataPoint, bucket.mArrows, returningArrow, bucket.mOrder);
            }

            mInsertionOrder++;
        }

        private void InsertDataPointIntoList(Arrow dataPoint, List<Arrow> list, bool returningArrow, List<uint> orders = null)
        {
            if(list != null)
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
                if(orders != null)
                {
                    orders.Insert(i, mInsertionOrder);
                }
            }
            else
            {
                list.Add(dataPoint);
                if (orders != null)
                {
                    orders.Add(mInsertionOrder);
                }
            }
        }

        public bool AttemptDelivery(FlagBundle[] importedStates, FlagID[] all = null)
        {
            bool success = false;

            if (!IsEmpty)
            {
                //Null check importedStates
                if(importedStates == null)
                {
                    importedStates = new FlagBundle[0];
                }

                //Prep for use of ALL
                List<FlagID> partitioningFlagsAlled = null;
                if (all == null) //Null check
                {
                    all = new FlagID[0];
                }
                else if(all.Length > 0) //Determine if there are any partition flags being set to ALL
                {
                    partitioningFlagsAlled = all.Distinct().Intersect(partitioningFlags).ToList();
                }

                //Determine bucket(s) in use
                List<List<Arrow>> bucketsToUse = new List<List<Arrow>>();
                List<List<uint>> ordersToUse = new List<List<uint>>();
                bucketsToUse.Add(overallData);

                if(partitioningFlags.Count > 0)
                {
                    bucketsToUse.Clear();

                    FlagBundle[] globalStates = Goddess.instance.globallyLoadedFlagBundles;
                    int[] globalIndecies = new int[globalStates.Length];
                    int[] importedIndecies = new int[importedStates.Length];
                    Array.Fill(globalIndecies, 0);
                    Array.Fill(importedIndecies, 0);
                    StringBuilder stringBuilder = new StringBuilder();
                    ComparableIntArray key;

                    if (partitioningFlagsAlled == null)
                    {

                        float value = -1;
                        bool located;
                        Flag targetFlag;
                        FlagID targetId;

                        int[] array = new int[partitioningFlags.Count];

                        for(int i = 0; i < partitioningFlags.Count; i++)
                        {
                            targetId = partitioningFlags[i];
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
                            array[i] = (int)value;
                        }
                        key = new ComparableIntArray(array);
                        if (partitionedData.HasKey(key))
                        {
                            bucketsToUse.Add(partitionedData[key].mArrows);
                            ordersToUse.Add(partitionedData[key].mOrder);
                        }
                        else
                        {
                            bucketsToUse.Add(new List<Arrow>());
                        }
                    }
                    else
                    {
                        int[] keyParts = new int[partitioningFlags.Count];
                        int[] indeciesOfPartitioned = new int[partitioningFlags.Count];
                        int[] indeciesOfAlled = new int[partitioningFlagsAlled.Count];
                        Array[] possibleAlledValues = new Array[partitioningFlags.Count];


                        //Determine the set value for non-ALL partitioning flags
                        for (int i = 0; i < indeciesOfPartitioned.Length; i++)
                        {
                            indeciesOfPartitioned[i] = partitioningFlagsAlled.IndexOf(partitioningFlags[i]);
                        }
                        for (int i = 0; i < indeciesOfAlled.Length; i++)
                        {
                            indeciesOfAlled[i] = partitioningFlags.IndexOf(partitioningFlagsAlled[i]);
                        }

                        float value = -1;
                        bool located;
                        Flag targetFlag;
                        FlagID targetID;

                        //Set keyParts for non-ALL partitoning flags
                        for (int idIndex = 0; idIndex < keyParts.Length; idIndex++)
                        {
                            if (indeciesOfPartitioned[idIndex] == -1)
                            {
                                located = false;
                                targetID = partitioningFlags[idIndex];
                                for (int j = 0; j < globalStates.Length && !located; j++)
                                {
                                    if (globalStates[j].flagsUsed.LinearSearch(targetID, ref globalIndecies[j], out targetFlag))
                                    {
                                        located = true;
                                        value = targetFlag.GetValue();
                                    }
                                }
                                for (int j = 0; j < importedStates.Length && !located; j++)
                                {
                                    if (importedStates[j].flagsUsed.LinearSearch(targetID, ref importedIndecies[j], out targetFlag))
                                    {
                                        located = true;
                                        value = targetFlag.GetValue();
                                    }
                                }

                                if (!located)
                                {
                                    value = -1;
                                }

                                keyParts[idIndex] = (int)value;
                            }
                        }

                        //Go through all the varations of keys under the ALL partitoning flags
                        int product = 1;
                        for (int i = 0; i < partitioningFlagsAlled.Count; i++)
                        {
                            possibleAlledValues[i] = Enum.GetValues(Goddess.instance.GetFlagSymbolType(partitioningFlagsAlled[i]));
                            product *= possibleAlledValues[i].Length;
                        }
                        for(int i = 0; i < product; i++)
                        {
                            int scratch = i;
                            for(int j = 0; j < partitioningFlagsAlled.Count; j++)
                            {
                                int currentAlledValue = scratch % possibleAlledValues[j].Length;
                                scratch /= possibleAlledValues[j].Length;

                                keyParts[indeciesOfAlled[j]] = (int)possibleAlledValues[j].GetValue(currentAlledValue);
                            }

                            key = new ComparableIntArray(keyParts);

                            if (partitionedData.HasKey(key))
                            {
                                bucketsToUse.Add(partitionedData[key].mArrows);
                                ordersToUse.Add(partitionedData[key].mOrder);
                            }
                        }

                        if(bucketsToUse.Count == 0)
                        {
                            bucketsToUse.Add(new List<Arrow>());
                        }
                    }
                }

                bool flagMeetsConditions;
                int arrowIndex = 0;
                List<int> arrowsToConsider = new List<int>();
                List<int> bucketsToConsider = new List<int>();
                bool singleBucket = bucketsToUse.Count == 1;
                bool anyArrowFound = false;
                List<Arrow> currentBucket;
                Arrow currentArrow;
                for (int bucketIndex = 0; bucketIndex < bucketsToUse.Count; bucketIndex++)
                {
                    currentBucket = bucketsToUse[bucketIndex];

                    arrowIndex = 0;
                    if (chooseSamePriority == ChooseSamePriority.RANDOM) //RANDOM
                    {
                        int highestPriorityFound;
                        if (arrowsToConsider.Count == 0)
                        {
                            highestPriorityFound = -1;
                        }
                        else
                        {
                            highestPriorityFound = bucketsToUse[bucketsToConsider[0]][arrowsToConsider[0]].GetPriority();
                        }

                        for (; arrowIndex < currentBucket.Count; arrowIndex++)
                        {
                            currentArrow = currentBucket[arrowIndex];
                            if (singleBucket && !currentArrow.IsPriority())
                            {
                                break;
                            }
                            if (anyArrowFound && highestPriorityFound > currentArrow.GetPriority())
                            {
                                break;
                            }
                            flagMeetsConditions = currentArrow.CondtionsMet(importedStates, all);
                            if (flagMeetsConditions)
                            {
                                if(highestPriorityFound < currentArrow.GetPriority() && arrowsToConsider.Count != 0)
                                {
                                    arrowsToConsider.Clear();
                                    bucketsToConsider.Clear();
                                }
                                arrowsToConsider.Add(arrowIndex);
                                bucketsToConsider.Add(bucketIndex);

                                highestPriorityFound = currentArrow.GetPriority();
                                anyArrowFound = true;
                            }
                        }

                        if (anyArrowFound && singleBucket)
                        {
                            int usedArrowIndex = arrowsToConsider[UnityEngine.Random.Range(0, arrowsToConsider.Count)];
                            success = currentBucket[usedArrowIndex].Fire(this, importedStates, all);
                            if (success && discardArrowsAfterUse)
                            {
                                if (!ReferenceEquals(currentBucket, overallData))
                                {
                                    overallData.Remove(currentBucket[usedArrowIndex]);
                                }
                                currentBucket.RemoveAt(usedArrowIndex);
                            }
                        }
                    }
                    if ((chooseSamePriority != ChooseSamePriority.RANDOM) //QUEUE, STACK...
                        || (singleBucket && !success && arrowIndex < bucketsToUse[bucketIndex].Count && !bucketsToUse[bucketIndex][arrowIndex].IsPriority())) //...or RANDOM has reached the priority 0 options, which are already jumbled up
                    {
                        for (; arrowIndex < bucketsToUse[bucketIndex].Count; arrowIndex++)
                        {
                            currentArrow = currentBucket[arrowIndex];
                            if(arrowsToConsider.Count != 0)
                            {
                                if(bucketsToUse[bucketsToConsider[0]][arrowsToConsider[0]].GetPriority() > currentArrow.GetPriority())
                                {
                                    break;
                                }
                            }

                            flagMeetsConditions = currentArrow.CondtionsMet(importedStates, all);
                            if (flagMeetsConditions)
                            {
                                if (singleBucket)
                                {
                                    success = currentArrow.Fire(this, importedStates, all);
                                    if (success && discardArrowsAfterUse)
                                    {
                                        if (!ReferenceEquals(currentBucket, overallData))
                                        {
                                            overallData.Remove(currentArrow);
                                        }
                                        currentBucket.RemoveAt(arrowIndex);
                                    }
                                    break;
                                }
                                else
                                {
                                    if (arrowsToConsider.Count != 0 && currentArrow.IsPriority())
                                    {
                                        bool higherPriority = bucketsToUse[bucketsToConsider[0]][arrowsToConsider[0]].GetPriority() < currentArrow.GetPriority();
                                        bool mostRecent = ordersToUse[bucketsToConsider[0]][arrowsToConsider[0]] < ordersToUse[bucketIndex][arrowIndex];
                                        if (higherPriority || mostRecent == recencyBias)
                                        {
                                            arrowsToConsider[0] = arrowIndex;
                                            bucketsToConsider[0] = bucketIndex;
                                        }
                                    }
                                    else
                                    {
                                        arrowsToConsider.Add(arrowIndex);
                                        bucketsToConsider.Add(bucketIndex);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                if(!singleBucket && arrowsToConsider.Count != 0)
                {
                    int usedIndex = UnityEngine.Random.Range(0, arrowsToConsider.Count);
                    Arrow usedArrow = bucketsToUse[bucketsToConsider[usedIndex]][arrowsToConsider[usedIndex]];
                    success = usedArrow.Fire(this, importedStates, all);
                    if (success && discardArrowsAfterUse)
                    {
                        if (!ReferenceEquals(bucketsToUse[bucketsToConsider[usedIndex]], overallData))
                        {
                            overallData.Remove(usedArrow);
                        }
                        bucketsToUse[bucketsToConsider[usedIndex]].RemoveAt(arrowsToConsider[usedIndex]);
                        ordersToUse[bucketsToConsider[usedIndex]].RemoveAt(arrowsToConsider[usedIndex]);
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
            mInsertionOrder = 0;
            partitionedData = new SortedStrictDictionary<ComparableIntArray, OrderedArrowList>();

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
                float value;
                int[] array;
                ComparableIntArray key;
                foreach (Arrow arrow in overallData)
                {
                    array = new int[partitioningFlags.Count];
                    for(int i = 0; i < partitioningFlags.Count; i++)
                    {
                        arrow.TryGetFlagEqualsValue(partitioningFlags[i], out value);
                        array[i] = (int)value;
                    }
                    key = new ComparableIntArray(array);
                    if (!partitionedData.HasKey(key))
                    {
                        partitionedData.Add(key, OrderedArrowList.Init());
                    }

                    OrderedArrowList bucket = partitionedData[key];
                    bucket.mArrows.Add(arrow);
                    bucket.mOrder.Add(mInsertionOrder);
                    mInsertionOrder++;
                }
            }

            tempPartitioningFlags = new List<FlagID>(partitioningFlags);
        }

        private void FlipRecencyBias()
        {
            recencyBias = !recencyBias;

            FlipArrowList(overallData);
            OrderedArrowList temp;
            for(int i = 0; i < partitionedData.Count; i++)
            {
                temp = partitionedData.GetTupleAtIndex(i).Value;
                FlipArrowList(temp.mArrows,temp.mOrder);
            }
        }

        private void FlipArrowList(List<Arrow> toFlip, List<uint> order = null)
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
                    if(order != null)
                    {
                        order.Reverse(targetIndex, i - targetIndex);
                    }
                    targetIndex = i;
                    targetPriority = toFlip[i].GetPriority();
                }
            }
            toFlip.Reverse(targetIndex, i - targetIndex);
            if (order != null)
            {
                order.Reverse(targetIndex, i - targetIndex);
            }
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

        private void DropArrowsOfBundle(ArrowBundle toDrop)
        {
            if (toDrop == null)
            {
                return;
            }

            float value;
            int[] array;
            ComparableIntArray key;
            foreach (Arrow e in toDrop.arrows)
            {
                if(overallData.Remove(e))
                {
                    array = new int[partitioningFlags.Count];
                    for (int i = 0; i < partitioningFlags.Count; i++)
                    {
                        e.TryGetFlagEqualsValue(partitioningFlags[i], out value);
                        array[i] = (int)value;
                    }
                    key = new ComparableIntArray(array);
                    if (partitionedData.HasKey(key))
                    {
                        OrderedArrowList bucket = partitionedData[key];
                        int index = bucket.mArrows.IndexOf(e);
                        bucket.mArrows.RemoveAt(index);
                        bucket.mOrder.RemoveAt(index);
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