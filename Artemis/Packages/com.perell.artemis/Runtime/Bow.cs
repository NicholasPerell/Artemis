using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis
{
    public abstract class PreTemplateBow : MonoBehaviour
    {

    }

    public abstract class Bow<T> : PreTemplateBow
    {
        [Space]
        [SerializeField]
        private Fletcher<T> deliverySystem;

        public event UnityAction onReportEnd;

        protected virtual void OnEnable()
        {
            deliverySystem.SetInSceneObject(this);
        }

        public abstract void Send(T data);

        public abstract bool IsBusy();

        public abstract void AbruptEnd();

        protected void ReportEnd()
        {
            onReportEnd?.Invoke();
            deliverySystem.ProcessEnd();
        }

        protected bool FletcherHasQueue()
        {
            return deliverySystem.IsSomethingToQueue();
        }

        protected Fletcher<T> GetFletcher()
        {
            return deliverySystem;
        }
    }
}
