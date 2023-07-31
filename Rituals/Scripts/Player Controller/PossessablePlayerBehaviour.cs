using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class PossessableMonoBehaviour : MonoBehaviour
    {
        bool isPossessed = false;
        public bool IsPossessed { get { return isPossessed; } }

        protected virtual void OnEnable()
        {
            AncientRuinsManager.OnPossessed += RespondToPossessed;
        }

        protected virtual void OnDisable()
        {
            AncientRuinsManager.OnPossessed -= RespondToPossessed;
        }

        private void RespondToPossessed(bool _isPossessed)
        {
            if (isPossessed != _isPossessed)
            {
                isPossessed = _isPossessed;
                OnPossessed(isPossessed);
            }
        }

        protected virtual void OnPossessed(bool isPossessed)
        {

        }
    }
}