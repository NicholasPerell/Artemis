using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public abstract class PlayerAbilityController : MonoBehaviour
    {
        protected PlayerController playerController;

        public void Initialize(PlayerController _playerController)
        {
            playerController = _playerController;
            OnInitialized();
        }

        abstract protected void OnInitialized();

        public virtual void Release()
        {

        }
    }
}