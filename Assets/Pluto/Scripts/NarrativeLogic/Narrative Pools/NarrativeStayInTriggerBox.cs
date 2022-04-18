using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
public class NarrativeStayInTriggerBox : MonoBehaviour
{
    [SerializeField]
    UnityEvent whenTimeElapsed;
    [SerializeField]
    float timeToElapse;
    [SerializeField]
    bool includeInitialEntrance;

    bool triggered = false;
    float timer = 0;

    private void Awake()
    {
        if (whenTimeElapsed.GetPersistentEventCount() == 0)
        {
            Debug.LogError(name + " does not have anything on this event to send out. Destroying object.");
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = timeToElapse;
                whenTimeElapsed.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!triggered && includeInitialEntrance)
            {
                whenTimeElapsed.Invoke();
                triggered = true;
            }
            timer = timeToElapse;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            timer = 0;
        }
    }
}
