using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class LobbyEntrance : MonoBehaviour
    {
        public event UnityAction OnEntered;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                OnEntered?.Invoke();
            }
        }
    }
}