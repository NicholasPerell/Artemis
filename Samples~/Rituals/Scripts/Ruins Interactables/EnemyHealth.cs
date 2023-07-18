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
    //[SerializeField]
    //GameObject demonSoulPrefab;

    public event UnityAction<Transform> EnemyDied;

    private void OnEnable()
    {
        health = maxHealth;
    }

    private void OnDisable()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            /*if (!GameObject.FindObjectOfType<WizardController>().possessed)
            {
                Instantiate(demonSoulPrefab, transform.position, transform.rotation);
            }*/
            //Destroy(this.gameObject);
            EnemyDied?.Invoke(transform);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
