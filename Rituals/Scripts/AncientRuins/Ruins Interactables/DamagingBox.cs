using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class DamagingBox : MonoBehaviour
    {
        [SerializeField]
        HealthEffectSource damageSource;

        [SerializeField]
        int damageAmount;

        public static event UnityAction<int, HealthEffectSource, Vector3> DamageDelt;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                DamageDelt?.Invoke(-damageAmount, damageSource, transform.position);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                DamageDelt?.Invoke(-damageAmount, damageSource, transform.position);
            }
        }
    }
}
