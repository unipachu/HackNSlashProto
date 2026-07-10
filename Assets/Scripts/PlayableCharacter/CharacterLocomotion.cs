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

        RotateForward(maxAngSpd);
;

        if (IsAffectedByGravity)
        {
            if (IsGrounded())
            {
                //Debug.Log("Grounded");
                _verticalVelocity = 0;
            }
            else
            {
                _verticalVelocity -= 9.81f * Time.deltaTime;
            }
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

        RotateForward(AngSpd);

        if (IsAffectedByGravity)
        {
            if (IsGrounded())
            {
                //Debug.Log("Grounded");
                _verticalVelocity = 0;
            }
            else
            {
                _verticalVelocity -= 9.81f * Time.deltaTime;
            }
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
    /// NOTE: Movement input should have a max length of 1 and represents xz-movement!
    /// </summary>
    // TODO: Problem here is that is solve movement isn't called every frame, velocity isn't updated every frame, and
    // TODO C: when SolveMovement is called again, it still uses the velocity from the last time it was called.
    //private void SolveMovement(Vector3 movementInput)
    //{
    //    //Debug.Log("SolveMovement called!");
    //    Vector2 xzMovementInput = new Vector2(movementInput.x, movementInput.y);
    //    UpdateVelocity(xzMovementInput);
    //    Vector3 XYVelocity = new Vector3(_horizontalVelocity.x, 0, _horizontalVelocity.y);
    //    characterController.SimpleMove(XYVelocity);
    //    RotateForward();
    //}

    //private void UpdateVelocity(Vector2 movementInput)
    //{
    //    _horizontalVelocity = Vector2.MoveTowards(_horizontalVelocity, movementInput * currentMaxLinearSpeed, currentMaxLinearAcc * Time.deltaTime);
    //}

    private void RotateForward(float maxAngSpd)
    {
        if (_horizontalVelocity == Vector2.zero) return;

        Vector3 dir3D = new(_horizontalVelocity.x, 0, _horizontalVelocity.y);

        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngSpd * Time.deltaTime);
    }

    // TODO: Delete commented code.
    ///// <summary>
    ///// Moves character controller and then applies gravity. Use with e.g. animations' delta movement.
    ///// </summary>
    //private void MoveCharacterController(Vector2 movementInput, Vector3 animRootMotion)
    //{

    //}

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
