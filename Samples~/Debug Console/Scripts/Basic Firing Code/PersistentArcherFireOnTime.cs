using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example
{
    public class PersistentArcherFireOnTime : MonoBehaviour
    {
        [SerializeField]
        Archer archer;
        [Space]
        [SerializeField]
        FlagBundle[] importedStates;
        [SerializeField]
        FlagID[] all;
        [Space]
        [SerializeField]
        [Range(0.001f, 3.0f)]
        private float timeBetweenFires = 3.0f;
        private float timer;

        void Start()
        {
            timer = 0;
        }

        void Update()
        {
            if (timer < timeBetweenFires)
            {
                timer += Time.deltaTime;
                if (timer >= timeBetweenFires)
                {
                    WhenFiresCodedDebug();
                    timer = 0;
                }
            }
        }

        void WhenFiresCodedDebug()
        {
            archer.AttemptDelivery(importedStates, all);
        }

    }
}
