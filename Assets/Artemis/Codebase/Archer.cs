using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative Priority Queue", menuName = "Artemis/Narrative Priority Queue")]
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
        private List<BundleLog> bundleHistory;

        public bool IsEmpty { get { return priorityQueue.IsEmpty() && generalPool.Count == 0; } }

        [ContextMenu("Init")]
        public void Init()
        {
            Refresh(true);
        }

        void Refresh(bool includePriorityQueue)
        {
            generalPool = new List<Arrow>();
            generalPool.AddRange(defaultContents);
            priorityQueue = new PriorityQueue<Arrow>(true);

            for (int i = generalPool.Count - 1; i > 1; i--)
            {
                int rnd = UnityEngine.Random.Range(0, i + 1);
                Arrow tmp = generalPool[rnd];
                generalPool[rnd] = generalPool[i];
                generalPool[i] = tmp;
            }

            for (int i = generalPool.Count - 1; i >= 0; i--)
            {
                Arrow tmp = generalPool[i];
                if (tmp.IsPriority())
                {
                    if (includePriorityQueue)
                    {
                        priorityQueue.Enqueue(tmp, -tmp.GetPriority());
                    }
                    generalPool.RemoveAt(i);
                }
            }
        }

        public void RecieveBundle(Arrow[] narrativeDataPoints)
        {
            foreach (Arrow e in narrativeDataPoints)
            {
                RecieveDataPoint(e);
            }
        }

        public void RecieveDataPoint(Arrow dataPoint)
        {
            if (dataPoint.IsPriority())
            {
                priorityQueue.Enqueue(dataPoint, -dataPoint.GetPriority());
            }
            else
            {
                int rnd = UnityEngine.Random.Range(0, generalPool.Count);
                generalPool.Insert(rnd, dataPoint);
            }
        }

        [ContextMenu("Attempt Delivery")]
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

        public void SetChoosingSamePriority(Archer.ChooseSamePriority _chooseSamePriority)
        {
            chooseSamePriority = _chooseSamePriority;
        }

        public Archer.ChooseSamePriority GetChoosingSamePriority()
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
            foreach (Arrow e in toDump.arrows)
            {
                RecieveDataPoint(e);
            }
        }

        public void RemoveBundle(ArrowBundle toDrop)
        {
            RemoveArrowsOfBundle(toDrop);
            LogBundleHistory(toDrop, false);
        }

        private void RemoveArrowsOfBundle(ArrowBundle toDrop)
        {
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