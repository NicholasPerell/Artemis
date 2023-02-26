using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestPathFinder : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        nav.SetDestination(target.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(nav.destination, target.position) > 1)
        {
            nav.SetDestination(target.position);
        }
        else if (Vector3.Distance(transform.position, target.position) < 3)
        {
            nav.isStopped = true;
        }
        else if(nav.isStopped)
        {
            nav.SetDestination(target.position);
        }
    }
}
