using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    int maxHealth = 20;
    int health;

    public event UnityAction<int> HealthChanged;
    public event UnityAction tookDamage;
    public event UnityAction died;

    void Start()
    {
        RefreshHealth();
    }

    private void RefreshHealth()
    {
        health = maxHealth;
    }

    private void ChangeHealth(int deltaHealth)
    {
        health += deltaHealth;
        HealthChanged.Invoke(health);
        if(health <= 0)
        {
            died.Invoke();
        }
        else if(deltaHealth < 0)
        {
            tookDamage.Invoke();
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
