using UnityEngine;

public enum LocomotionType
{
    VelocityByDirectionalInput,
    DirectMotion,
}

/// <summary>
/// Movement for characters.
/// </summary>
// TODO: Refactor
public class CharacterLocomotion : MonoBehaviour
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

    private Vector2 _horizontalVelocity = Vector2.zero;

    public Vector2 HorizontalVelocity => _horizontalVelocity;
    public CharacterController CharacterController => characterController;

    /// <summary>
    /// Call once per frame to move character.
    /// NOTE: Calling this multiple times a frame can cause problems because character controller is not recommended to be called
    /// two times a frame.
    /// </summary>
    // TODO: This class now takes care of velocity based movement as well as direct movement by e.g. animation delta movement.
    // TODO C: Is this a good way to do this?
    public void UpdateMovement(LocomotionType locomotionType, Vector3 movement)
    {
        switch (locomotionType)
        {
            case LocomotionType.VelocityByDirectionalInput:
                SolveMovement(movement);
                break;
            case LocomotionType.DirectMotion:
                MoveCharacterController(movement);
                break;
            default:
                Debug.LogError("Switch defaulted.", this);
                break;
        }
    }

    /// <summary>
    /// NOTE: Movement input should have a max length of 1 and represents xz-movement!
    /// </summary>
    // TODO: Problem here is that is solve movement isn't called every frame, velocity isn't updated every frame, and
    // TODO C: when SolveMovement is called again, it still uses the velocity from the last time it was called.
    private void SolveMovement(Vector3 movementInput)
    {
        //Debug.Log("SolveMovement called!");
        Vector2 xzMovementInput = new Vector2(movementInput.x, movementInput.y);
        UpdateVelocity(xzMovementInput);
        Vector3 XYVelocity = new Vector3(_horizontalVelocity.x, 0, _horizontalVelocity.y);
        characterController.SimpleMove(XYVelocity);
        RotateForward();
    }

    private void UpdateVelocity(Vector2 movementInput)
    {
        _horizontalVelocity = Vector2.MoveTowards(_horizontalVelocity, movementInput * maxLinearSpeed, acceleration * Time.deltaTime);
    }

    private void RotateForward()
    {
        if (_horizontalVelocity == Vector2.zero) return;

        Vector3 dir3D = new(_horizontalVelocity.x, 0, _horizontalVelocity.y);

        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngularSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Moves character controller and then applies gravity. Use with e.g. animations' delta movement.
    /// </summary>
    private void MoveCharacterController(Vector3 motion)
    {
        // We also update velocity here even though we don't use it, so that when we use it the next time, the game doesn't use the last
        // velocity when velocity based movement was used.
        _horizontalVelocity = Vector3.zero;
        // NOTE: I tried to use both CharacterController.Move and .SimpleMove at the same time (Move for xz-movement and simple
        // move for gravity), but it caused the character to move even when motion parameter was zero. CharacterController
        // documentation recommends to only use either Move or SimpleMove and that seems to have been the problem (though I do not
        // understand why). When using both, the character seemed to move with the same speed and same direction as it moved
        // the last time SimpleMove was called, even though in this method the SimpleMove was set to move Vector3.zero. Weird.
        CharacterController.SimpleMove(motion/Time.deltaTime);
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check.
    /// </summary>
    // TODO: Make local variables into fields and reveal to inspector.
    private bool IsGrounded()
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
