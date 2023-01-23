using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    [CreateAssetMenu(fileName = "New Mockup", menuName = "Artemis/Mockup")]
    public class MockUpArcher : ScriptableObject
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

        //WHen Empty
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

        public void SetChoosingSamePriority(Archer.ChooseSamePriority _chooseSamePriority)
        {
            chooseSamePriority = _chooseSamePriority;
        }

        public Archer.ChooseSamePriority GetChoosingSamePriority()
        {
            return chooseSamePriority;
        }


        /// <summary>
        /// Cleans out any null Keys or Values
        /// </summary>
        private void CleanBBundleList()
        {
            for (int i = bundleHistory.Count - 1; i >= 0; i--)
            {
                if (bundleHistory[i].Equals(null))
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

        }

        public void RemoveBundle(ArrowBundle toDrop)
        {
            RemoveArrowsOfBundle(toDrop);
            LogBundleHistory(toDrop, false);
        }

        private void RemoveArrowsOfBundle(ArrowBundle toDrop)
        {

        }

        private void LogBundleHistory(ArrowBundle bundle, bool isAdding)
        {
            if (bundleHistory == null)
            {
                bundleHistory = new List<MockUpArcher.BundleLog>();
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
                    bundleHistory.Add(new MockUpArcher.BundleLog(bundle, isAdding));
                }
            }
            else
            {
                CleanBBundleList();
            }
        }

        [ContextMenu("Clear Bundle History")]
        private void ClearBundleHistory()
        {
            bundleHistory.Clear();
        }

        public List<MockUpArcher.BundleLog> GetBundleHistory()
        {
            if (bundleHistory == null)
            {
                bundleHistory = new List<MockUpArcher.BundleLog>();
            }

            return bundleHistory;
        }
    }
}