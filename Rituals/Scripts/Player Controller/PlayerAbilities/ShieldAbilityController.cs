using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class ShieldAbilityController : PlayerAbilityController
    {
        [SerializeField]
        [Range(0,360)]
        float angleOfProtection;
        [SerializeField]
        [Min(0)]
        float manaPerSecond;
        [SerializeField]
        [Range(0, 1)]
        float speedMultiplier;

        protected override void OnInitialized()
        {
            transform.position = playerController.PlayerPosition;
            SetForward();
            ActivateProtection();
        }

        private void Update()
        {
            SetForward();
            SpendMana();
        }

        private void SetForward()
        {
            transform.forward = playerController.MousePosition - playerController.PlayerPosition;
        }

        private void SpendMana()
        {
            if(playerController.Mana.SpendMana(manaPerSecond * Time.deltaTime))
            {
                ActivateProtection();
            }
            else
            {
                DeactivateProtection();
            }
        }

        protected override void OnRelease()
        {
            DeactivateProtection();
        }

        private void ActivateProtection()
        {
            playerController.Movement.AddSpeedMultiplier(this.GetType(), speedMultiplier);
            playerController.Health.SetBlocker(transform.forward, angleOfProtection);
        }

        private void DeactivateProtection()
        {
            playerController.Movement.RemoveSpeedMultiplier(this.GetType());
            playerController.Health.RemoverBlocker();
            Destroy(this.gameObject);
        }
    }
}