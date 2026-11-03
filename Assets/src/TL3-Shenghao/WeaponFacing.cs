using UnityEngine;

public class WeaponFacing : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerBody;

    private Vector3 rightPosition;
    private Vector3 rightScale;
    private bool facingLeft;
    private float previousPlayerX;

    private void Awake()
    {
        rightPosition = transform.localPosition;
        rightScale = transform.localScale;
        if (playerBody != null)
            previousPlayerX = playerBody.transform.position.x;
    }

    private void LateUpdate()
    {
        if (playerBody == null)
            return;

        float currentPlayerX = playerBody.transform.position.x;
        float horizontalMovement = currentPlayerX - previousPlayerX;
        previousPlayerX = currentPlayerX;

        if (horizontalMovement < -0.0001f)
            facingLeft = true;
        else if (horizontalMovement > 0.0001f)
            facingLeft = false;

        float direction = facingLeft ? -1f : 1f;

        transform.localPosition = new Vector3(
            rightPosition.x * direction, rightPosition.y, rightPosition.z);

        transform.localScale = new Vector3(
            rightScale.x * direction, rightScale.y, rightScale.z);
    }
}