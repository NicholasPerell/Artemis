using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCorruption : MonoBehaviour
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


    public event UnityAction Corrupted;
    public event UnityAction StartBuildUp;
    public event UnityAction<float> ChangedCorruptionChance;

    private void OnEnable()
    {
        StartCorruptionBuildUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isCorrupted)
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

        if(timeBeforeCheck > checkInterval)
        {
            timeBeforeCheck = 0;
            if(timeSinceStart > gracePeriod)
            {
                isCorrupted = corruptionChance > Random.value;
                if(isCorrupted)
                {
                    Corrupted?.Invoke();
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
