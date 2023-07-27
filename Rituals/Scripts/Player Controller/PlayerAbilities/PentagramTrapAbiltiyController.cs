using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class PentagramTrapAbiltiyController : PlayerAbilityController
    {
        [Header("Thrown")]
        [SerializeField]
        float timeThrown;
        [SerializeField]
        float speedThrown;

        [Space]
        [Header("Once Landed")]
        [SerializeField]
        float radiusOfTrigger;
        [SerializeField]
        float radiusOfDamage;
        [SerializeField]
        float damageDealt;

        [Space]
        [Header("Components")]
        [SerializeField]
        Animator animator;
        [SerializeField]
        Rigidbody rb;

        bool isArmed { get { return timeThrown <= 0; } }
        bool hasExploded;
        float timeSinceExplosion;

        protected override void OnInitialized()
        {
            SetForward();
            rb.velocity = transform.forward * speedThrown;
            hasExploded = false;
            timeSinceExplosion = 0;
        }

        private void Update()
        {
            if(!isArmed)
            {
                CheckForWalls();
                CountDownThrowing();
            }
            else if(!hasExploded)
            {
                CheckForEnemy();
            }
            else
            {
                CountDownToEnd();
            }
        }

        private void SetForward()
        {
            transform.forward = playerController.MousePosition - playerController.PlayerPosition;
        }

        private void CheckForWalls()
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position + Vector3.down * .5f, radiusOfTrigger))
            {
                if (obj.tag == "Walling")
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }

        private void CountDownThrowing()
        {
            timeThrown -= Time.deltaTime;
            if(isArmed)
            {
                rb.velocity = Vector3.zero;
                animator.SetTrigger("Armed");
            }
        }

        private void CheckForEnemy()
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position + Vector3.down * .5f, radiusOfTrigger))
            {
                if (obj.tag == "Enemy")
                {
                    Explode();
                }
            }
        }

        private void Explode()
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position + Vector3.down * .5f, radiusOfDamage))
            {
                if (obj.tag == "Enemy")
                {
                    obj.GetComponent<EnemyHealth>().TakeDamage(damageDealt);
                }
            }
            animator.SetTrigger("Explode");
            hasExploded = true;
        }

        private void CountDownToEnd()
        {
            timeSinceExplosion += Time.deltaTime;
            if(timeSinceExplosion >= animator.GetCurrentAnimatorStateInfo(0).length)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * .5f, radiusOfTrigger);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * .5f, radiusOfDamage);
        }
    }
}