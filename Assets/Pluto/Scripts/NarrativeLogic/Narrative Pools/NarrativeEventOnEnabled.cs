using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
public class NarrativeEventOnEnabled : MonoBehaviour
{
    [SerializeField]
    UnityEvent whenTriggered;

    private void Awake()
    {
        if (whenTriggered.GetPersistentEventCount() == 0)
        {
            Debug.LogError(name + " does not have anything on this event to send out. Removing script from game object.");
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        whenTriggered.Invoke();
        //yep
    }
}
