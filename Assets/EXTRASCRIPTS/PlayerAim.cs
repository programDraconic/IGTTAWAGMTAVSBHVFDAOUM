using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private float aimOffsetDegrees = 0f;

    private void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        float cameraDistance =
            Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, cameraDistance)
        );

        Vector2 direction = (Vector2)mouseWorld - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + aimOffsetDegrees);
    }
}