using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerCorruption : PossessableMonoBehaviour
    {
        [SerializeField]
        [Min(0.1f)]
        float checkInterval = 1;
        [SerializeField]
        float corruptionChance;
        [SerializeField]
        [Min(float.Epsilon)]
        float baseChance = 0.01f;
        [SerializeField]
        [Tooltip("When the player has started a run or freed themselves, we wait this long before beginning with corruption checks.")]
        float gracePeriod;

        float timeBeforeCheck;
        float timeSinceStart;

        [SerializeField]
        bool isCorrupted;

        public event UnityAction OnCorrupted;
        public event UnityAction StartBuildUp;
        public event UnityAction<float> ChangedCorruptionChance;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCorruptionBuildUp();
        }

        protected override void OnPossessed(bool isPossessed)
        {
            if (!isPossessed)
            {
                StartCorruptionBuildUp();
            }
        }

        void Update()
        {
            if (!isCorrupted)
            {
                CheckForCorruption();
            }
        }

        void StartCorruptionBuildUp()
        {
            timeBeforeCheck = 0;
            timeSinceStart = 0;
            isCorrupted = false;
            corruptionChance = baseChance;

            StartBuildUp?.Invoke();
        }

        void CheckForCorruption()
        {
            timeBeforeCheck += Time.deltaTime;
            timeSinceStart += Time.deltaTime;

            if (timeBeforeCheck > checkInterval)
            {
                timeBeforeCheck = 0;
                if (timeSinceStart > gracePeriod)
                {
                    isCorrupted = corruptionChance > Random.value;
                    if (isCorrupted)
                    {
                        OnCorrupted?.Invoke();
                    }
                }
            }
        }

        public void AddToCorruptionChance(float deltaChance)
        {
            corruptionChance += deltaChance;
            ChangedCorruptionChance?.Invoke(deltaChance);
        }

        public float GetBaseChance()
        {
            return baseChance;
        }

        public float GetCheckInterval()
        {
            return checkInterval;
        }
    }
}