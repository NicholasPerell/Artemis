using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMana : MonoBehaviour
{
    [SerializeField]
    float manaMax;
    [SerializeField]
    float currentMana;
    [SerializeField]
    float manaRegenPerSecond;

    public event UnityAction<float> ManaChanged;

    private void OnEnable()
    {
        Refill();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMana < manaMax)
        {
            currentMana += manaRegenPerSecond * Time.deltaTime;
            if (currentMana > manaMax)
            {
                currentMana = manaMax;
            }
            ManaChanged?.Invoke(currentMana);
        }
    }

    void Refill()
    {
        currentMana = manaMax;
        ManaChanged?.Invoke(currentMana);
    }

    public void GainMana(float gained)
    {
        if (currentMana < manaMax)
        {
            currentMana += gained;
            if (currentMana > manaMax)
            {
                currentMana = manaMax;
            }
            ManaChanged?.Invoke(currentMana);
        }
    }

    public bool SpendMana(float spent)
    {
        if(spent > currentMana)
        {
            return false;
        }

        currentMana -= spent;
        ManaChanged?.Invoke(currentMana);

        return true;
    }

    public float GetMaxMana()
    {
        return manaMax;
    }
}
