using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed;

    bool canMove;
    Vector2 movementInput;
    Vector2 facingInput;
    Rigidbody rb;
    Vector2 facing;

    Vector3 mouseInWorldSpace;
    public Vector3 MousePosition { get { return mouseInWorldSpace; } }

    Dictionary<System.Type, float> speedMultipliers;

    private void Awake()
    {
        speedMultipliers = new Dictionary<System.Type, float>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        canMove = true;
    }

    void Update()
    {
        CheckForInput();
    }

    private void FixedUpdate()
    {
        AttemptMove();
    }

    void CheckForInput()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        if(movementInput.sqrMagnitude > 1)
        {
            movementInput.Normalize();
        }

        mouseInWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        facingInput = new Vector2(mouseInWorldSpace.x - transform.position.x, mouseInWorldSpace.z - transform.position.z);
    }

    void AttemptMove()
    {
        if(canMove)
        {
            Vector3 velocity = new Vector3(movementInput.x, 0, movementInput.y) * speed;
            foreach(var e in speedMultipliers)
            {
                velocity *= e.Value;
            }
            rb.velocity = velocity;
            facing = facingInput;
        }
        else if(rb.velocity.sqrMagnitude > 0)
        {
            facing = rb.velocity;
        }
    }

    public Vector2 GetFacing()
    {
        return facing;
    }

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > 0;
    }

    public void AddSpeedMultiplier(System.Type type, float amount)
    {
        speedMultipliers.TryAdd(type, amount);
    }

    public void RemoveSpeedMultiplier(System.Type type)
    {
        speedMultipliers.Remove(type);
    }
}
