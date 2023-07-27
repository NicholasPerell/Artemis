using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class DemonShieldAbiltiyController : PlayerAbilityController
    {
        [SerializeField]
        float timeShielding;

        protected override void OnInitialized()
        {
            transform.localPosition = Vector3.zero;
            playerController.Health.GiveTemporaryInvulnerability(timeShielding);
        }

        void Update()
        {
            RunTimer();
        }

        void RunTimer()
        {
            timeShielding -= Time.deltaTime;
            if (timeShielding <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}