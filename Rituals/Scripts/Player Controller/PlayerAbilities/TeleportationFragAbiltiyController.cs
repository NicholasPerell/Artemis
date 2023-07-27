using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class TeleportationFragAbiltiyController : PlayerAbilityController
    {
        [SerializeField]
        float damage;
        [SerializeField]
        float radius;

        [SerializeField]
        Animator animator;

        float timer;

        protected override void OnInitialized()
        {
            Teleport();
        }

        void Update()
        {
            RunTimer();
        }

        private void FixedUpdate()
        {
            DamageEnemiesInRadius();
        }

        private void Teleport()
        {
            Vector3 teleportTo = Vector3.zero;
            teleportTo.x = Mathf.Clamp(playerController.MousePosition.x, -6, 6);
            teleportTo.z = Mathf.Clamp(playerController.MousePosition.z, -3, 3);

            transform.position = teleportTo;
            playerController.transform.position = teleportTo;
        }

        private void DamageEnemiesInRadius()
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position, radius))
            {
                if (obj.tag == "Enemy")
                {
                    obj.GetComponent<EnemyHealth>().TakeDamage(damage * Time.fixedDeltaTime);
                }
            }
        }

        void RunTimer()
        {
            timer += Time.deltaTime;
            if (timer >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Destroy(this.gameObject);
            }
        }
    }
}