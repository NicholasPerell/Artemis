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

    [HideInInspector]
    public UnityEvent tookDamage;
    [HideInInspector]
    public UnityEvent died;

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
        if(health <= 0)
        {
            died.Invoke();
        }
        else if(deltaHealth < 0)
        {
            tookDamage.Invoke();
        }
    }
}
