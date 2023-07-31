using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class PlayerAnimator : PossessableMonoBehaviour
    {
        [SerializeField]
        Animator animator;

        PlayerMovement playerMovement;
        PlayerHealth playerHealth;

        float isPossessedBlend = 0;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (playerMovement == null)
            {
                playerMovement = GetComponent<PlayerMovement>();
            }

            if (playerHealth == null)
            {
                playerHealth = GetComponent<PlayerHealth>();
            }
            playerHealth.TookDamage += HandleDamaged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (playerMovement == null)
            {
                playerMovement = GetComponent<PlayerMovement>();
            }

            if (playerHealth == null)
            {
                playerHealth = GetComponent<PlayerHealth>();
            }
            playerHealth.TookDamage -= HandleDamaged;
        }

        protected override void OnPossessed(bool isPossessed)
        {
            if(isPossessed)
            {
                HandleCaught();
            }
            else
            {
                HandleFreed();
            }
        }

        void Update()
        {
            animator.SetFloat("IsMoving", playerMovement.IsMoving() ? 1 : 0);
            animator.SetFloat("IsPossessed", isPossessedBlend);
            Vector2 facing = playerMovement.GetFacing();
            animator.SetFloat("FacingX", facing.x);
            animator.SetFloat("FacingY", facing.y);
        }

        void HandleDamaged()
        {
            animator.SetTrigger("Hurt");
        }

        void HandleCaught()
        {
            animator.SetTrigger("Caught");
            isPossessedBlend = 1;
        }
        void HandleFreed()
        {
            animator.SetTrigger("Freed");
            isPossessedBlend = 0;
        }
    }
}