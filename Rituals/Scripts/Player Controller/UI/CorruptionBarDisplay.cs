using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionBarDisplay : MonoBehaviour
{
    [SerializeField]
    PlayerCorruption playerCorruption;

    [SerializeField]
    Image barDisplay;
    [SerializeField]
    Image barShine;

    float checkInterval;
    float corruptionChance;
    float baseChance;
    float corruptionTimeOfCertainty;
    float timeFilling;
    float timeSinceCheck;
    float baseAmount;


    private void OnEnable()
    {
        baseChance = playerCorruption.GetBaseChance();
        checkInterval = playerCorruption.GetCheckInterval();
        playerCorruption.ChangedCorruptionChance += RespondToChangedCorruptionChance;
        playerCorruption.Corrupted += RespondToCorrupted;
        playerCorruption.StartBuildUp += RespondToStartBuildUp;

        timeSinceCheck = 0;
        timeFilling = 0;
        corruptionChance = baseChance;
        corruptionTimeOfCertainty = checkInterval / corruptionChance;
        baseAmount = 0;
    }

    private void RespondToStartBuildUp()
    {
        timeSinceCheck = 0;
        timeFilling = 0;
        corruptionChance = baseChance;
        corruptionTimeOfCertainty = checkInterval / corruptionChance;
        baseAmount = 0;
    }

    private void RespondToCorrupted()
    {
        baseAmount = 1;
        timeSinceCheck = -1;
        barShine.fillAmount = 0;
    }

    private void RespondToChangedCorruptionChance(float newChance)
    {
        SetNewCorruptionChance(newChance);
    }

    private void OnDisable()
    {
        playerCorruption.ChangedCorruptionChance -= RespondToChangedCorruptionChance;
        playerCorruption.Corrupted -= RespondToCorrupted;
        playerCorruption.StartBuildUp -= RespondToStartBuildUp;
    }

    // Update is called once per frame
    void Update()
    {
        timeFilling += Time.deltaTime;
        barDisplay.fillAmount = baseAmount + (timeFilling / corruptionTimeOfCertainty * (1 - baseAmount));

        if (timeSinceCheck >= 0)
        {
            timeSinceCheck += Time.deltaTime;
            while (timeSinceCheck >= checkInterval)
            {
                timeSinceCheck -= checkInterval;
            }
            barShine.fillAmount = barDisplay.fillAmount * timeSinceCheck / checkInterval;
        }
    }

    void SetNewCorruptionChance(float newChance)
    {
        corruptionChance = newChance;

        float oldTimeLeft = corruptionTimeOfCertainty - timeFilling;
        float newTimeLeft = checkInterval / corruptionChance;

        if (newTimeLeft < oldTimeLeft)
        {
            corruptionTimeOfCertainty = checkInterval / corruptionChance;
            timeFilling = 0;
            baseAmount = barDisplay.fillAmount;
        }
    }


}
