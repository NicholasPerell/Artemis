using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artemis
{
    public abstract class Bow : MonoBehaviour
    {

    }

    public abstract class Bow<T> : Bow
    {
        [Space]
        [SerializeField]
        Fletcher<T> deliverySystem;

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
}
