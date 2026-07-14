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

    [SerializeField] LayerMask groundMask;

    [Header("Refs")]
    [SerializeField] private CharacterController characterController;

    [HideInInspector] public bool IsAffectedByGravity = true;
    //[HideInInspector] public float currentMaxLinearSpeed = 5;
    //[HideInInspector] public float currentMaxLinearAcc = 100;
    //[HideInInspector] public float currentMaxAngularSpeed = 800;


    
    [HideInInspector] public Vector2 _horizontalVelocity = Vector2.zero;
    [HideInInspector] public float _verticalVelocity = 0;


    /// <summary>
    /// Call once per frame to move character.
    /// NOTE: Calling this multiple times a frame can cause problems because character controller is not recommended to be called
    /// two times a frame.
    /// </summary>
    // TODO: This class now takes care of velocity based movement as well as direct movement by e.g. animation delta movement.
    // TODO C: Is this a good way to do this?
    public void UpdateMovement(Vector2 horMovementInput, Vector3 animRootMotion, float maxLinSpd, float LinAcc, float maxAngSpd)
    {
        Vector2 xzMovementInput = new Vector2(horMovementInput.x, horMovementInput.y);

        _horizontalVelocity = Vector2.MoveTowards(_horizontalVelocity, horMovementInput * maxLinSpd, LinAcc * Time.deltaTime);

        RotateForward(maxAngSpd, xzMovementInput);
;

        if (IsAffectedByGravity)
        {
            ApplyGravity();
        }
        else
        {
            _verticalVelocity = 0;
        }
        animRootMotion.x += _horizontalVelocity.x * Time.deltaTime;
        animRootMotion.y += _verticalVelocity * Time.deltaTime;
        animRootMotion.z += _horizontalVelocity.y * Time.deltaTime;
        characterController.Move(animRootMotion);
    }

    /// <summary>
    /// Snaps to linear and angular velocity.
    /// </summary>
    public void UpdateMovement(Vector2 horMovementInput, Vector3 animRootMotion, float LinSpd, float AngSpd)
    {
        Vector2 xzMovementInput = new Vector2(horMovementInput.x, horMovementInput.y);
        _horizontalVelocity = horMovementInput * LinSpd;

        RotateForward(AngSpd, xzMovementInput);

        if (IsAffectedByGravity)
        {
            ApplyGravity();
        }
        else
        {
            _verticalVelocity = 0;
        }
        animRootMotion.x += _horizontalVelocity.x * Time.deltaTime;
        animRootMotion.y += _verticalVelocity * Time.deltaTime;
        animRootMotion.z += _horizontalVelocity.y * Time.deltaTime;
        characterController.Move(animRootMotion);
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check.
    /// </summary>
    // TODO: Make local variables into fields and reveal to inspector.
    // TODO: Idk if this is good. Maybe just do a sphere cast from capusle bottom to avoid hits with walls/ceilings?
    public bool IsGrounded()
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

    void ApplyGravity()
    {
        if (IsGrounded())
        {
            _verticalVelocity = 0;
        }
        else
        {
            _verticalVelocity -= 9.81f * Time.deltaTime;
        }
    }

    /// <summary>
    /// Rotates character towards the forward vector in xz-plane.
    /// </summary>
    /// <param name="fWD">In XZ-plane.</param>
    void RotateForward(float maxAngSpd, Vector2 fWD)
    {
        if (fWD == Vector2.zero) return;

        Vector3 dir3D = new Vector3(fWD.x, 0, fWD.y);

        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngSpd * Time.deltaTime);
    }
}
