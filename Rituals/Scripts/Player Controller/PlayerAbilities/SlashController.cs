using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class SlashController : PlayerAbilityController
    {
        [SerializeField]
        float attackTime;
        [SerializeField]
        float damage;

        protected override void OnInitialized()
        {
            transform.position = playerController.PlayerPosition;
            SetForward();
        }

        private void Update()
        {
            SetForward();
            RunTimers();
        }

        private void SetForward()
        {
            transform.forward = playerController.MousePosition - playerController.PlayerPosition;
        }

        private void RunTimers()
        {
            attackTime -= Time.deltaTime;
            if(attackTime <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Enemy")
            {
                collision.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }

    }
}