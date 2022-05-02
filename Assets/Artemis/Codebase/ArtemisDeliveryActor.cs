using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtemisDeliveryActor<T> : MonoBehaviour
{
    [Space]
    [SerializeField]
    ArtemisDeliverySystem<T> deliverySystem;

    protected virtual void OnEnable()
    {
        deliverySystem.SetInSceneObject(this);
    }

    public abstract void Send(T data);

    public abstract bool IsBusy();

    public abstract void AbruptEnd();

    public void ReportEnd()
    {
        deliverySystem.ProcessEnd();
    }
}
