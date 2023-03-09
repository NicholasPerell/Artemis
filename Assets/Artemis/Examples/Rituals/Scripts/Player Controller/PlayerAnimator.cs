using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    PlayerMovement playerMovement;
    PlayerHealth playerHealth;

    float isPossessed = 0;

    private void OnEnable()
    {
        if(playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if(playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
        playerHealth.tookDamage += HandleDamaged;
    }

    private void OnDisable()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
        playerHealth.tookDamage -= HandleDamaged;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("IsMoving", playerMovement.IsMoving() ? 1 : 0);
        animator.SetFloat("IsPossessed", isPossessed);
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
        isPossessed = 1;
    }
    void HandleFreed()
    {
        animator.SetTrigger("Freed");
        isPossessed = 0;
    }
}
