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
    [SerializeField] LayerMask groundMask;

    [Header("Refs")]
    [SerializeField] private CharacterController characterController;

    private Vector2 velocity = Vector2.zero;

    public Vector2 Velocity => velocity;
    public CharacterController CharacterController => characterController;

    private void Update()
    {
        UpdateIsGrounded();
    }

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

    private bool UpdateIsGrounded()
    {
        float extraDistance = 0.05f;
        float radius = characterController.radius;
        float height = Mathf.Max(characterController.height, radius * 2f);

        Vector3 center = characterController.transform.position + characterController.center;

        Vector3 bottom = center + Vector3.down * (height / 2f - radius);
        Vector3 top = center + Vector3.up * (height / 2f - radius);

        float castDistance = extraDistance + characterController.skinWidth;

        RaycastHit groundHit;
        bool hitGround = Physics.CapsuleCast(
            top,
            bottom,
            radius,
            Vector3.down,
            out groundHit,
            castDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (hitGround)
        {
            float slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            if( slopeAngle <= characterController.slopeLimit)
            {
                return true;
            }
        }
        return false;
    }
}
