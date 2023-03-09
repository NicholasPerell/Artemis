using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionScreenDisplay : MonoBehaviour
{
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
    float checkInterval;
    [SerializeField]
    float corruptionChance;
    [SerializeField]
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

    // Start is called before the first frame update
    void Start()
    {
        timeFilling = 0;
        corruptionChance = baseChance;
        corruptionTimeOfCertainty = checkInterval / corruptionChance;
        baseAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeFilling += Time.deltaTime;

        sliceDisplay.pixelsPerUnitMultiplier = Mathf.Lerp(startPixelMultiplier,endPixelMultiplier,baseAmount + (timeFilling / corruptionTimeOfCertainty * (1 - baseAmount))) + (Mathf.Cos(Time.time * waveFrequency) - .5f) * waveAmplitude;
    }

    [ContextMenu("ChangeStuffs")]
    void SetNewCorruptionChance()
    {
        float oldTimeLeft = corruptionTimeOfCertainty - timeFilling;
        float newTimeLeft = checkInterval / corruptionChance;

        if (newTimeLeft < oldTimeLeft)
        {
            baseAmount = baseAmount + (timeFilling / corruptionTimeOfCertainty * (1 - baseAmount));
            corruptionTimeOfCertainty = checkInterval / corruptionChance;
            timeFilling = 0;
        }
    }


}
