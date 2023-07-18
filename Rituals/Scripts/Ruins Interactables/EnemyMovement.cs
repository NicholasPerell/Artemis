using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Perell.Artemis.Example.Rituals
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        float attackRange;
        [SerializeField]
        float attackDelay;
        float attackCooldown;
        [SerializeField]
        float attackDamageStartTime;
        [SerializeField]
        float attackDamageEndTime;
        [SerializeField]
        GameObject[] damagingBoxes;
        [Space]

        [SerializeField]
        Transform playerTransform;

        [SerializeField]
        Animator anim;
        [SerializeField]
        NavMeshAgent nav;

        bool inRange;

        private void OnEnable()
        {
            attackCooldown = 0;

            if (playerTransform == null)
            {
                //TODO: Better way to set the playerTransform
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            }

            nav.stoppingDistance = attackRange;
            nav.SetDestination(playerTransform.position);
        }

        private void Update()
        {
            LocatePlayer();
            CheckForAttacking();
            UpdateFacing();
        }

        private void LocatePlayer()
        {
            inRange = (transform.position - playerTransform.position).sqrMagnitude < attackRange * attackRange;
            if ((nav.destination - playerTransform.position).sqrMagnitude > attackRange * attackRange / 4)
            {
                nav.SetDestination(playerTransform.position);
            }
            else if (inRange)
            {
                nav.isStopped = true;
            }
            else if (nav.isStopped)
            {
                nav.isStopped = false;
                nav.SetDestination(playerTransform.position);
            }
        }

        private void CheckForAttacking()
        {
            if(attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }
            else if(inRange)
            {
                Invoke("AttackStart", attackDamageStartTime);
                Invoke("AttackEnd", attackDamageEndTime);

                anim.SetTrigger("Attack");
                attackCooldown = attackDelay;
            }
        }

        private void UpdateFacing()
        {
            Vector3 facing;

            if(inRange || nav.isStopped)
            {
                facing = playerTransform.position - transform.position;
            }
            else
            {
                facing = nav.desiredVelocity;
            }

            anim.SetFloat("FacingX", facing.x);
            anim.SetFloat("FacingY", facing.z);
        }

        private void AttackStart()
        {
            foreach(GameObject obj in damagingBoxes)
            {
                obj.SetActive(true);
            }
        }

        private void AttackEnd()
        {
            foreach (GameObject obj in damagingBoxes)
            {
                obj.SetActive(false);
            }
        }
    }
}
