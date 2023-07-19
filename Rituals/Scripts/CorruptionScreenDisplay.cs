using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.Rituals
{
    public class CorruptionScreenDisplay : MonoBehaviour
    {
        [SerializeField]
        PlayerCorruption playerCorruption;

        [SerializeField]
        Image sliceDisplay;

        [Space]
        [SerializeField]
        [Min(0)]
        float startPixelMultiplier;
        [SerializeField]
        [Min(0)]
        float endPixelMultiplier;

        [Space]
        [SerializeField]
        [Min(0.1f)]
        float checkInterval;
        [SerializeField]
        float corruptionChance;
        [SerializeField]
        [Min(0.0001f)]
        float baseChance;
        [SerializeField]
        float corruptionTimeOfCertainty;
        [SerializeField]
        float timeFilling;
        [SerializeField]
        float baseAmount;

        [Space]
        [SerializeField]
        float waveFrequency;
        [SerializeField]
        float waveAmplitude;

        [Space]
        [SerializeField]
        bool maxOut;
        [SerializeField]
        [Range(0 + float.Epsilon, 1)]
        float lerpToMax = .75f;

        private void OnEnable()
        {
            if(playerCorruption != null)
            {
                baseChance = playerCorruption.GetBaseChance();
                checkInterval = playerCorruption.GetCheckInterval();

                playerCorruption.ChangedCorruptionChance += SetNewCorruptionChance;
                playerCorruption.Corrupted += SetMaxCorruptionDisplay;
                playerCorruption.StartBuildUp += BeginBuildUp;
            }

            BeginBuildUp();
        }

        private void OnDisable()
        {
            if (playerCorruption != null)
            {
                baseChance = playerCorruption.GetBaseChance();
                checkInterval = playerCorruption.GetCheckInterval();

                playerCorruption.ChangedCorruptionChance -= SetNewCorruptionChance;
                playerCorruption.Corrupted -= SetMaxCorruptionDisplay;
                playerCorruption.StartBuildUp -= BeginBuildUp;
            }
        }

        // Update is called once per frame
        void Update()
        {
            timeFilling += Time.deltaTime;

            if (!maxOut)
            {
                sliceDisplay.pixelsPerUnitMultiplier = Mathf.Lerp(startPixelMultiplier, endPixelMultiplier, baseAmount + (timeFilling / corruptionTimeOfCertainty * (1 - baseAmount))) + Mathf.Cos(Time.time * waveFrequency) * waveAmplitude * .5f;
            }
            else
            {
                sliceDisplay.pixelsPerUnitMultiplier = Mathf.Lerp(sliceDisplay.pixelsPerUnitMultiplier, endPixelMultiplier - waveAmplitude * .5f, lerpToMax);
            }
        }

        void SetNewCorruptionChance(float deltaChance)
        {
            corruptionChance += deltaChance;

            float oldTimeLeft = corruptionTimeOfCertainty - timeFilling;
            float newTimeLeft = checkInterval / corruptionChance;

            if (newTimeLeft < oldTimeLeft)
            {
                baseAmount = baseAmount + (timeFilling / corruptionTimeOfCertainty * (1 - baseAmount));
                corruptionTimeOfCertainty = checkInterval / corruptionChance;
                timeFilling = 0;
            }
        }

        void BeginBuildUp()
        {
            maxOut = false;
            timeFilling = 0;
            corruptionChance = baseChance;
            corruptionTimeOfCertainty = checkInterval / corruptionChance;
            baseAmount = 0;
        }

        void SetMaxCorruptionDisplay()
        {
            maxOut = true;
        }
    }
}