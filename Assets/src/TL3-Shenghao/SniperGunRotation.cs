// Created by ChatGPT for the Shrimp Game project.
using UnityEngine;

public class SniperGunRotation : MonoBehaviour
{
    [Header("Gun Rotation Settings")]
    [SerializeField] private float minAngle = -30f;
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Start Settings")]
    [SerializeField] private bool startMovingUp = true;

    private float currentAngle;
    private int direction;

    private void Start()
    {
        // Ensure valid angle range
        if (minAngle > maxAngle)
        {
            float temp = minAngle;
            minAngle = maxAngle;
            maxAngle = temp;
        }

        currentAngle = startMovingUp ? minAngle : maxAngle;
        direction = startMovingUp ? 1 : -1;

        transform.localRotation =
            Quaternion.Euler(0f, 0f, currentAngle);
    }

    private void Update()
    {
        float speed = Mathf.Max(0f, rotationSpeed);

        float targetAngle =
            direction == 1 ? maxAngle : minAngle;

        currentAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            speed * Time.deltaTime
        );

        transform.localRotation =
            Quaternion.Euler(0f, 0f, currentAngle);

        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            direction *= -1;
        }
    }
}
