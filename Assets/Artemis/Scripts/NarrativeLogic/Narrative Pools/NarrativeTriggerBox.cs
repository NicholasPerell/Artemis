using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
/// <summary>
/// Triggers subtitle when player enters the collider area.
/// </summary>
public class NarrativeTriggerBox : MonoBehaviour
{
    [SerializeField]
    UnityEvent whenTriggered;

    bool triggered = false;

    private void Awake()
    {
        triggered = false;

        if(whenTriggered.GetPersistentEventCount() == 0)
        {
            Debug.LogError(name + " does not have anything on this event to send out. Destroying object.");
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            triggered = true;
        }
    }

    private void Update()
    {
        if(triggered)
        {
            whenTriggered.Invoke();
            Destroy(this.gameObject);
        }
    }
}
