using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    int maxHealth = 1;
    [SerializeField]
    float health;
    [SerializeField]
    Animator anim;

    public event UnityAction<EnemyHealth> EnemyDied;

    private void OnEnable()
    {
        health = maxHealth;
    }

    private void OnDisable()
    {

    }

    public void TakeDamage(float damage)
    {
        anim.SetTrigger("Hurt");
        health -= damage;
        if (health <= 0)
        {
            EnemyDied?.Invoke(this);
            transform.parent.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Ouch!")]
    private void TestDamage()
    {
        TakeDamage(maxHealth);
    }

}
