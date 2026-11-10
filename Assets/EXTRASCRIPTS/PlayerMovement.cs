using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check if a keyboard is available
        if (Keyboard.current == null) return;

        Vector2 movement = Vector2.zero;

        // Check Horizontal Input (A/D or Left/Right Arrows)
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            movement.x += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            movement.x -= 1f;

        // Check Vertical Input (W/S or Up/Down Arrows)
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            movement.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            movement.y -= 1f;

        // Normalize vector so diagonal movement isn't faster
        movement = movement.normalized;

        bool isMoving = movement.sqrMagnitude > 0.01f;

        // Send parameters to Animator

        animator.SetFloat("MoveX", movement.x, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveY", movement.y, 0.1f, Time.deltaTime);
        
        // animator.SetBool("IsMoving", isMoving);

        movementInput = movement;
    }

    private void FixedUpdate()
    {
        // Move the player using physics
        rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }
}