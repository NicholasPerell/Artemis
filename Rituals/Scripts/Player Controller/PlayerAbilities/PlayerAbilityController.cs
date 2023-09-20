using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public abstract class PlayerAbilityController : DestroyOnDisable
    {
        protected PlayerController playerController;

        public void Initialize(PlayerController _playerController)
        {
            playerController = _playerController;
            OnInitialized();
        }

        abstract protected void OnInitialized();

        public void Release()
        {
            OnRelease();
        }

        protected virtual void OnRelease()
        {

        }
    }
}