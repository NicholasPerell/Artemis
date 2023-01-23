using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    [CreateAssetMenu(fileName = "New Mockup", menuName = "Artemis/Mockup")]
    public class MockUpArcher : ScriptableObject
    {
        [Header("When Empty")]
        public bool loops;
        public bool includeBundlesInLoop;
        public bool includeHigherPrioritiesInLoop;

        [SerializeField]
        Archer.ChooseSamePriority chooseSamePriority;
        [SerializeField]
        bool recencyBias;

        [SerializeField]
        public List<Arrow> defaultContents;

        public void SetChoosingSamePriority(Archer.ChooseSamePriority _chooseSamePriority)
        {
            chooseSamePriority = _chooseSamePriority;
        }

        public Archer.ChooseSamePriority GetChoosingSamePriority()
        {
            return chooseSamePriority;
        }
    }
}