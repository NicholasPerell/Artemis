using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Perell
[CreateAssetMenu(fileName = "New Artemis Narrative Priority Queue", menuName = "Artemis/Narrative Priority Queue")]
public class ArtemisNarrativePriorityQueues : ScriptableObject
{
    enum WhenEmpty
    {
        END,
        REFRESH_GENERAL,
        REFRESH_ALL
    }

    [SerializeField]
    ArtemisNarrativeSystem system;
    [Space]
    [SerializeField]
    List<ArtemisNarrativeDataPoint> defaultContents;
    [SerializeField]
    [Tooltip("When two items have the same priority value over 0, does the priority queue prefer the most recently added narrative data point?")]
    bool recencyBias;
    [SerializeField]
    WhenEmpty loopSettings;

    [Space]
    [SerializeField]
    PriorityQueue<ArtemisNarrativeDataPoint> priorityQueue; //for reactivity and recency. Priority value > 0
    [SerializeField]
    List<ArtemisNarrativeDataPoint> generalPool; //for when there's nothing w/ priority. Priority value == 0


    public bool IsEmpty { get { return priorityQueue.IsEmpty() && generalPool.Count == 0; } }

    [ContextMenu("Init")]
    public void Init()
    {
        Refresh(true);
    }

    void Refresh(bool includePriorityQueue)
    {
        generalPool = new List<ArtemisNarrativeDataPoint>();
        generalPool.AddRange(defaultContents);
        priorityQueue = new PriorityQueue<ArtemisNarrativeDataPoint>(true);

        for (int i = generalPool.Count - 1; i > 1; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            ArtemisNarrativeDataPoint tmp = generalPool[rnd];
            generalPool[rnd] = generalPool[i];
            generalPool[i] = tmp;
        }

        for (int i = generalPool.Count - 1; i >= 0; i--)
        {
            ArtemisNarrativeDataPoint tmp = generalPool[i];
            if(tmp.IsPriority())
            {
                if(includePriorityQueue)
                {
                    priorityQueue.Enqueue(tmp, -tmp.GetPriority());
                }
                generalPool.RemoveAt(i);
            }
        }
    }

    public void RecieveBundle(ArtemisNarrativeDataPoint[] narrativeDataPoints)
    {
        foreach (ArtemisNarrativeDataPoint e in narrativeDataPoints)
        {
            RecieveDataPoint(e);
        }
    }

    void RecieveDataPoint(ArtemisNarrativeDataPoint dataPoint)
    {
        if(dataPoint.IsPriority())
        {
            priorityQueue.Enqueue(dataPoint, -dataPoint.GetPriority());
        }
        else
        {
            int rnd = UnityEngine.Random.Range(0, generalPool.Count);
            generalPool.Insert(rnd, dataPoint);
        }
    }

    [ContextMenu("Attempt Delivery")]
    public bool AttemptDelivery()
    {
        if(IsEmpty)
        {
            return false;
        }

        bool success = false;
        Stack<ArtemisNarrativeDataPoint> skipped = new Stack<ArtemisNarrativeDataPoint>();
        ArtemisNarrativeDataPoint possibleDeliverable;

        if (priorityQueue.IsEmpty()) //General Pool
        {
            possibleDeliverable = generalPool[0];
            generalPool.RemoveAt(0);
        }
        else //Priority Queue
        {
            possibleDeliverable = priorityQueue.Dequeue();
        }

        success = system.MeetsConditions(possibleDeliverable);
        while (!success && (!priorityQueue.IsEmpty() || generalPool.Count > 0))
        {
            skipped.Push(possibleDeliverable);

            if (priorityQueue.IsEmpty()) //General Pool
            {
                possibleDeliverable = generalPool[0];
                generalPool.RemoveAt(0);
            }
            else //Priority Queue
            {
                possibleDeliverable = priorityQueue.Dequeue();
            }

            success = system.MeetsConditions(possibleDeliverable);
        }

        if (success)
        {
            success = possibleDeliverable.Deliver(system);
            if (!success)
            {
                RecieveDataPoint(possibleDeliverable);
            }
            else
            {
                Debug.Log("Delivered " + possibleDeliverable.name);
            }
        }
        else
        {
            RecieveDataPoint(possibleDeliverable);
        }

        while(skipped.Count > 0)
        {
            RecieveDataPoint(skipped.Pop());
        }
        
        return success;
    }

    public void IgnoreSuccessAttemptDelivery()
    {
        bool temp = AttemptDelivery();
    }
}