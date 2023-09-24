using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Perell.Artemis.Generated;
using Perell.Artemis.Saving;
using System.IO;
using Perell.Artemis.Debugging;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Perell.Artemis
{
    public class Archer : ScriptableObject, IBinaryReadWriteable
    {
        public struct BundleLog : IBinaryReadWriteable
        {
            public ArrowBundle bundle;
            public bool isAdding;

            public BundleLog(ArrowBundle _bundle, bool _isAdding)
            {
                bundle = _bundle;
                isAdding = _isAdding;
            }

            public void WriteToBinary(ref BinaryWriter binaryWriter)
            {
                binaryWriter.Write(isAdding);
                bundle.WriteToBinary(ref binaryWriter);
            }

            public void ReadFromBinary(ref BinaryReader binaryReader)
            {
                isAdding = binaryReader.ReadBoolean();

                bundle ??= ScriptableObject.CreateInstance<ArrowBundle>();
                bundle.ReadFromBinary(ref binaryReader);
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
        SortedStrictDictionary<ComparableIntArray, OrderedArrowList> partitionedData = new SortedStrictDictionary<ComparableIntArray, OrderedArrowList>();

        //When Empty
        [SerializeField]
        private bool loops;
        [SerializeField]
        private bool includeBundlesInLoop;
        [SerializeField]
        private bool includeHigherPrioritiesInLoop;

        //Delete Arrows?
        [SerializeField]
        private bool discardArrowsAfterUse = true;

        //Non-Value Priorities
        [SerializeField]
        Archer.ChooseSamePriority chooseSamePriority;
        [SerializeField]
        private bool recencyBias;

        //Init Contents
        [SerializeField]
        public List<Arrow> defaultContents;

        //Bundles
        [SerializeField]
        public ArrowBundle tempArrowBundle;
        [SerializeField]
        private List<BundleLog> bundleHistory = new List<BundleLog>();

        [SerializeField]
        private uint insertionOrder;

        [System.Serializable]
        private struct OrderedArrowList : IBinaryReadWriteable
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

            public void ReadFromBinary(ref BinaryReader binaryReader)
            {
                mArrows = binaryReader.ReadScriptableObjectList<Arrow>();
                mOrder.Clear();
                int mOrderCount = binaryReader.ReadInt32();
                for (int i = 0; i < mOrderCount; i++)
                {
                    mOrder.Add(binaryReader.ReadUInt32());
                }
            }

            public void WriteToBinary(ref BinaryWriter binaryWriter)
            {
                binaryWriter.Write(mArrows);
                binaryWriter.Write(mOrder.Count);
                for (int i = 0; i < mOrder.Count; i++)
                {
                    binaryWriter.Write(mOrder[i]);
                }
            }
        }

        public bool IsEmpty { get { return overallData.Count == 0; } }

        public void Init()
        {
            ArtemisDebug.Instance.OpenReportLine(name + ".Init()");
            Refresh(true,false);
            ArtemisDebug.Instance.ReportLine("Initialization Complete");
            ArtemisDebug.Instance.CloseReport();
        }

        private void Refresh(bool includeNonZeroPriority, bool includeBundles)
        {
            ArtemisDebug.Instance.OpenReportLine("Refresh()");
            ArtemisDebug.Instance.Report("Include Non-Zero Priority: ").ReportLine(includeNonZeroPriority?"true":"false")
                .Report("Include Bundles: ").ReportLine(includeBundles ? "true" : "false");
            insertionOrder = 0;
            overallData = new List<Arrow>();
            partitionedData = new SortedStrictDictionary<ComparableIntArray, OrderedArrowList>();

            ArtemisDebug.Instance.Report("Default Contents Count: ").ReportLine(defaultContents.Count);
            ArtemisDebug.Instance.Indents++;
            foreach (Arrow arrow in defaultContents)
            {
                ArtemisDebug.Instance.Report("Arrow ").Report(arrow.name).Report(", Priority ").ReportLine(arrow.GetPriority());
                if (arrow.GetPriority() == 0 || includeNonZeroPriority)
                {
                    RecieveArrow(arrow);
                }
            }
            ArtemisDebug.Instance.Indents--;


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
            ArtemisDebug.Instance.CloseReport();
        }

        public void SetToLoopedState()
        {
            Refresh(includeHigherPrioritiesInLoop, includeBundlesInLoop);
        }
        
        public void ReturnArrow(Arrow arrow)
        {
            if (discardArrowsAfterUse)
            {
                RecieveArrow(arrow, true);
            }
        }

        public void RecieveArrow(Arrow arrow, bool returningArrow = false)
        {
            ArtemisDebug.Instance.OpenReportLine(name + ".RecieveArrow()");
            ArtemisDebug.Instance.Report("Arrow: ").ReportLine(arrow.name);
            ArtemisDebug.Instance.Report("Returning Arrow: ").ReportLine(returningArrow?"true":"false");

            ArtemisDebug.Instance.ReportLine("Overall Data");
            //Overall Data
            InsertArrowIntoList(arrow, overallData, returningArrow);

            Debug.Log("RecieveArrow: partitioningFlags.Count: " + partitioningFlags.Count);

            //Partitioned Data
            if (partitioningFlags.Count != 0)
            {
                ArtemisDebug.Instance.ReportLine("Parititoned Data");
                float value;
                int[] array = new int[partitioningFlags.Count];
                ArtemisDebug.Instance.Report("Parititoning Key: ");
                for (int i = 0; i < partitioningFlags.Count; i++)
                {
                    arrow.TryGetFlagEqualsValue(partitioningFlags[i], out value);
                    array[i] = (int)value;
                    ArtemisDebug.Instance.Report(partitioningFlags[i]).Report(" ").Report(array[i]).Report("  ");
                }
                ArtemisDebug.Instance.ReportLine();
                ComparableIntArray key = new ComparableIntArray(array);
                if (!partitionedData.HasKey(key))
                {
                    ArtemisDebug.Instance.ReportLine("Creating a new OrderedArrowList for this key.");
                    partitionedData.Add(key, OrderedArrowList.Init());
                }
                OrderedArrowList bucket = partitionedData[key];
                InsertArrowIntoList(arrow, bucket.mArrows, returningArrow, bucket.mOrder);
            }

            insertionOrder++;
            ArtemisDebug.Instance.Report("Insertion Order increased to ").ReportLine(insertionOrder);
            ArtemisDebug.Instance.CloseReport();
        }

        private void InsertArrowIntoList(Arrow arrow, List<Arrow> list, bool returningArrow, List<uint> orders = null)
        {
            ArtemisDebug.Instance.OpenReportLine("InsertArrowIntoList")
                .Report("List: ").ReportLine(list != null ? "Size "+ list.Count : "null")
                .Report("Returning Arrow: ").ReportLine(returningArrow?"true":"false")
                .Report("Orders: ").ReportLine(orders);

            if (list != null)
            {
                if (list.Count != 0)
                {
                    ArtemisDebug.Instance.Report("Arrow ").Report(arrow.name).Report(" has a priority of ").ReportLine(arrow.GetPriority());
                    int i;
                    if (arrow.IsPriority())
                    {
                        if (!returningArrow)
                        {
                            ArtemisDebug.Instance.Report("Recency Bias: ").ReportLine(returningArrow ? "true" : "false");
                        }

                        ArtemisDebug.Instance.Indents++;
                        for (i = 0; i < list.Count; i++)
                        {
                            ArtemisDebug.Instance.Report(i).Report(": checking for ").Report(list[i].GetPriority()).Report(" <");

                            bool insertable;
                            if (recencyBias || returningArrow)
                            {
                                ArtemisDebug.Instance.Report("=");
                                insertable = list[i].GetPriority() <= arrow.GetPriority();
                            }
                            else
                            {
                                insertable = list[i].GetPriority() < arrow.GetPriority();
                            }

                            ArtemisDebug.Instance.Report(" ").ReportLine(arrow.GetPriority());

                            if (insertable)
                            {
                                break;
                            }
                        }
                        ArtemisDebug.Instance.Indents--;
                    }
                    else
                    {
                        for (i = 0; i < list.Count; i++)
                        {
                            if (!list[i].IsPriority())
                            {
                                ArtemisDebug.Instance.Report("Placing arrow in random point in the general pool (").Report(i).Report("–").Report(list.Count).ReportLine(")");
                                i = UnityEngine.Random.Range(i, list.Count + 1);
                                break;
                            }
                        }
                    }
                    ArtemisDebug.Instance.Report("Inserting arrow at ").ReportLine(i);
                    list.Insert(i, arrow);
                    if (orders != null)
                    {
                        ArtemisDebug.Instance.Report("Inserting order of ").Report(insertionOrder).Report(" at ").ReportLine(i);
                        orders.Insert(i, insertionOrder);
                    }
                }
                else
                {
                    ArtemisDebug.Instance.ReportLine("List is empty, simply adding to list.");
                    list.Add(arrow);
                    if (orders != null)
                    {
                        ArtemisDebug.Instance.ReportLine("Same for Orders.");
                        orders.Add(insertionOrder);
                    }
                }
            }

            ArtemisDebug.Instance.CloseReport();
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

                    FlagBundle[] globalStates = Goddess.instance.globallyLoadedFlagBundles.ToArray();
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
                        FlagID targetID;

                        int[] array = new int[partitioningFlags.Count];

                        for(int i = 0; i < partitioningFlags.Count; i++)
                        {
                            targetID = partitioningFlags[i];
                            located = false;
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

#if UNITY_EDITOR
        public void SetChoosingSamePriority(ChooseSamePriority _chooseSamePriority)
        {
            chooseSamePriority = _chooseSamePriority;
            if((_chooseSamePriority == ChooseSamePriority.QUEUE && recencyBias)
                || (_chooseSamePriority == ChooseSamePriority.STACK && !recencyBias))
            {
                FlipRecencyBias();
            }
        }
#endif

        public void Repartition()
        {
            insertionOrder = 0;
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
                    bucket.mOrder.Add(insertionOrder);
                    insertionOrder++;
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
            Debug.Log("Dump Bundle: " + toDump);
            DumpArrowsOfBundle(toDump);
            LogBundleHistory(toDump, true);
        }

        private void DumpArrowsOfBundle(ArrowBundle toDump, bool includeNonZeroPriority = true)
        {
            if(toDump == null)
            {
                Debug.Log("DumpArrowsOfBundle: " + toDump + "was null.");
                return;
            }

            foreach (Arrow arrow in toDump.GetArrows())
            {
                Debug.Log("DumpArrowsOfBundle: " + arrow);
                if (includeNonZeroPriority || !arrow.IsPriority())
                {
                    RecieveArrow(arrow);
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
            foreach (Arrow arrow in toDrop.GetArrows())
            {
                if(overallData.Remove(arrow))
                {
                    array = new int[partitioningFlags.Count];
                    for (int i = 0; i < partitioningFlags.Count; i++)
                    {
                        arrow.TryGetFlagEqualsValue(partitioningFlags[i], out value);
                        array[i] = (int)value;
                    }
                    key = new ComparableIntArray(array);
                    if (partitionedData.HasKey(key))
                    {
                        OrderedArrowList bucket = partitionedData[key];
                        int index = bucket.mArrows.IndexOf(arrow);
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
                //TODO: Evaluate if it's worth finding out a system for canceling out that doesn't break from drop-dump patterns
                /*bool inverseExists = false;

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
                }*/

                bundleHistory.Add(new BundleLog(bundle, isAdding));
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

        public void WriteToBinary(ref BinaryWriter binaryWriter)
        {
            //Overall Data
            overallData.WriteToBinary(ref binaryWriter);

            //Partitioned Data
            SortedStrictDictionary<ComparableIntArray, OrderedArrowList>.Tuple tuple;
            binaryWriter.Write(partitionedData.Count);
            for (int i = 0; i < partitionedData.Count; i++)
            {
                tuple = partitionedData.GetTupleAtIndex(i);
                tuple.Key.WriteToBinary(ref binaryWriter);
                tuple.Value.WriteToBinary(ref binaryWriter);
            }

            //BundleHistory
            bundleHistory.WriteToBinary(ref binaryWriter);
        }

        public void ReadFromBinary(ref BinaryReader binaryReader)
        {
            //Overall Data
            overallData = binaryReader.ReadScriptableObjectList<Arrow>();

            //Partitioned Data
            ComparableIntArray comparableIntArray = new ComparableIntArray();
            OrderedArrowList orderedArrowList = OrderedArrowList.Init();
            int partitionedDataCount = binaryReader.ReadInt32();
            for (int i = 0; i < partitionedDataCount; i++)
            {
                comparableIntArray.ReadFromBinary(ref binaryReader);
                orderedArrowList.ReadFromBinary(ref binaryReader);
                partitionedData.Add(comparableIntArray, orderedArrowList);
            }

            //BundleHistory
            bundleHistory = binaryReader.ReadList<BundleLog>();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}