using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Units per second.")]
    [SerializeField] private float maxLinearSpeed = 5;
    [Tooltip("Degrees per second.")]
    [SerializeField] private float maxAngularSpeed = 800;
    [Tooltip("Units per second squared.")]
    [SerializeField] private float acceleration = 100;

    [Header("Refs")]
    [SerializeField] private CharacterController characterController;

    private Vector2 velocity = Vector2.zero;

    private Vector2 Velocity => velocity;

    public void SolveMovement(Vector2 movementInput)
    {
        UpdateVelocity(movementInput);
        Vector3 XYVelocity = new Vector3(velocity.x, 0, velocity.y);
        characterController.SimpleMove(XYVelocity);
        RotateForward();
    }

    private void UpdateVelocity(Vector2 movementInput)
    {
        velocity = Vector2.MoveTowards(velocity, movementInput * maxLinearSpeed, acceleration * Time.deltaTime);
    }

    public void RotateForward()
    {
        if (velocity == Vector2.zero) return;

        Vector3 dir3D = new(velocity.x, 0, velocity.y);

        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngularSpeed * Time.deltaTime);
    }
}
