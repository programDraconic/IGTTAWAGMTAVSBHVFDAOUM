using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        var keyboard = Keyboard.current;
        moveInput = Vector2.zero;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            moveInput.y += 1f;

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            moveInput.y -= 1f;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            moveInput.x += 1f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            moveInput.x -= 1f;

        moveInput = moveInput.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}