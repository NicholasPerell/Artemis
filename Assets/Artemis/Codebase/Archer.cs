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
                        DumpArrowsOfBundle(log.bundle);
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

        public void SetLoopedState()
        {
            Refresh(includeHigherPrioritiesInLoop, includeBundlesInLoop);
        }

        public void RecieveDataPoint(Arrow dataPoint)
        {
            //Overall Data
            if (overallData.Count != 0)
            {
                int i;
                for (i = 0; i < overallData.Count; i++)
                {
                    bool insertable;
                    if(recencyBias)
                    {
                        insertable = overallData[i].GetPriority() <= dataPoint.GetPriority();
                    }
                    else
                    {
                        insertable = overallData[i].GetPriority() < dataPoint.GetPriority();
                    }

                    if(insertable)
                    {
                        break;
                    }
                }
                overallData.Insert(i, dataPoint);
            }
            else
            {
                overallData.Add(dataPoint);
            }

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

                List<Arrow> bucket = partitionedData[key];

                if (bucket.Count != 0)
                {
                    int i;
                    for (i = 0; i < bucket.Count; i++)
                    {
                        bool insertable;
                        if (recencyBias)
                        {
                            //TODO: Consider putting this and the overall version into a function
                            insertable = bucket[i].GetPriority() <= dataPoint.GetPriority();
                        }
                        else
                        {
                            insertable = bucket[i].GetPriority() < dataPoint.GetPriority();
                        }

                        if (insertable)
                        {
                            break;
                        }
                    }
                    bucket.Insert(i, dataPoint);
                }
                else
                {
                    bucket.Add(dataPoint);
                }
            }
        }

        public bool AttemptDelivery()
        {
            if (IsEmpty)
            {
                return false;
            }

            bool success = false;
            Stack<Arrow> skipped = new Stack<Arrow>();
            Arrow possibleDeliverable;

            if (priorityQueue.IsEmpty()) //General Pool
            {
                possibleDeliverable = generalPool[0];
                generalPool.RemoveAt(0);
            }
            else //Priority Queue
            {
                possibleDeliverable = priorityQueue.Dequeue();
            }

            success = possibleDeliverable.CondtionsMet();
            while (!success && (!priorityQueue.IsEmpty() || generalPool.Count > 0))
            {
                skipped.Push(possibleDeliverable);

                if (priorityQueue.IsEmpty()) //General Pool
                {
                    possibleDeliverable = generalPool[0];
                    generalPool.RemoveAt(0);
                }
                else //Priority Queue
                {
                    possibleDeliverable = priorityQueue.Dequeue();
                }

                success = possibleDeliverable.CondtionsMet();
            }

            if (success)
            {
                success = possibleDeliverable.Deliver(this);
                if (!success)
                {
                    RecieveDataPoint(possibleDeliverable);
                }
                else
                {
                    //Debug.Log("Delivered " + possibleDeliverable.name);
                }
            }
            else
            {
                RecieveDataPoint(possibleDeliverable);
            }

            while (skipped.Count > 0)
            {
                RecieveDataPoint(skipped.Pop());
            }

            return success;
        }

        public void IgnoreSuccessAttemptDelivery()
        {
            bool temp = AttemptDelivery();
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

                    Debug.Log(key + " " + arrow.name);

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

        private void DumpArrowsOfBundle(ArrowBundle toDump)
        {
            if(toDump == null)
            {
                return;
            }

            foreach (Arrow e in toDump.arrows)
            {
                RecieveDataPoint(e);
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

            foreach (Arrow e in toDrop.arrows)
            {
                //TODO
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