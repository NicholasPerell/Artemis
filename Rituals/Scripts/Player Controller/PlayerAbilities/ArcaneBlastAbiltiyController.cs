using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class ArcaneBlastAbiltiyController : PlayerAbilityController
    {
        [SerializeField]
        Rigidbody rb;
        [SerializeField]
        float speed;
        [SerializeField]
        int damage;
        [SerializeField]
        float lifeSpan;

        protected override void OnInitialized()
        {
            SetPosition();
        }

        private void SetPosition()
        {
            transform.position = playerController.PlayerPosition;
            transform.forward = playerController.MousePosition - playerController.PlayerPosition;
            rb.velocity = transform.forward * speed;
        }

        private void Update()
        {
            RunTimer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                other.GetComponent<EnemyHealth>().TakeDamage(damage);
                Destroy(this.gameObject);
            }
            else if(other.tag == "Walling")
            {
                Destroy(this.gameObject);
            }
        }

        void RunTimer()
        {
            lifeSpan -= Time.deltaTime;
            if (lifeSpan <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}