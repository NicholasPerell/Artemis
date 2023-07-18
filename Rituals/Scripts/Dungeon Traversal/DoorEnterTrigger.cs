using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorEnterTrigger : MonoBehaviour
{
    [Range(0, 3)]
    public int direction;

    [HideInInspector]
    public UnityEvent<int> doorEntered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            doorEntered.Invoke(direction);
        }
    }
}
