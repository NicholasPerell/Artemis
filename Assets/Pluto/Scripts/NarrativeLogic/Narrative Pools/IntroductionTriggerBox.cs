using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// script below, unless otherwise stated, created by Nicholas Perell, standard copyright and usage applies.
public class IntroductionTriggerBox : MonoBehaviour
{
    [SerializeField]
    NarrativePriorityQueues introduction;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if(introduction.IsEmpty)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(!introduction.IsEmpty)
        {
            introduction.AttemptDelivery();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
