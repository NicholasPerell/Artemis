using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    //[CreateAssetMenu(fileName = "New Artemis Narrative Bundle", menuName = "Artemis/Narrative Bundle")]
    public class ArrowBundle : ScriptableObject
    {
        [SerializeField]
        Arrow[] narrativeDataPoints;
        [Space]
        [SerializeField]
        Archer sendTo;
        bool sentOut = false;

        public void Init()
        {
            sentOut = false;
        }

        public bool SendBundle()
        {
            bool success = !sentOut;
            if (success)
            {
                sendTo.RecieveBundle(narrativeDataPoints);
                sentOut = true;
            }
            return success;
        }
    }
}