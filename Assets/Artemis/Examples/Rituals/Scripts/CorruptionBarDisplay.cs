using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CorruptionBarDisplay : MonoBehaviour
{
    [SerializeField]
    Image barDisplay;

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

        barDisplay.fillAmount = baseAmount + (timeFilling / corruptionTimeOfCertainty * (1 - baseAmount));
    }

    [ContextMenu("ChangeStuffs")]
    void SetNewCorruptionChance()
    {
        float oldTimeLeft = corruptionTimeOfCertainty - timeFilling;
        float newTimeLeft = checkInterval / corruptionChance;

        if(newTimeLeft < oldTimeLeft)
        {
            corruptionTimeOfCertainty = checkInterval / corruptionChance;
            timeFilling = 0;
            baseAmount = barDisplay.fillAmount;
        }
    }


}
