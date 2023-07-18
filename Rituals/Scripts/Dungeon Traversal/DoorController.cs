using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] NavMeshObstacle navObstacle;
    public Animator doorDisplay;

    public void Open(bool includeDoor, int tier = 0)
    {
        doorDisplay.SetFloat("Tier",tier);
        boxCollider.enabled = false;
        navObstacle.enabled = false;
        if (includeDoor)
        {
            doorDisplay.SetTrigger("Open");
        }
        else
        {
            doorDisplay.SetTrigger("Hide");

        }
    }

    public void Close(bool includeDoor, int tier = 0)
    {
        doorDisplay.SetFloat("Tier",tier);
        boxCollider.enabled = true;
        navObstacle.enabled = true;
        if (includeDoor)
        {
            doorDisplay.SetTrigger("Close");
        }
        else
        {
            doorDisplay.SetTrigger("Hide");

        }
    }
}
