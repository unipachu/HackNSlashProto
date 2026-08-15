using UnityEngine;

/// <summary>
/// Movement for character controller.
/// </summary>
// TODO: Refactor
public class CharCtrlMov : MonoBehaviour{
    [Header("Movement Settings")]
    [SerializeField] LayerMask groundMask;

    [Header("Refs")]
    [SerializeField] CharacterController charCtrl;
    [HideInInspector] public bool IsAffectedByGravity = true;
    [HideInInspector] public Vector2 horVel = Vector2.zero;
    [HideInInspector] public float verVel = 0;

    /// <summary>
    /// Call once per frame to move character.
    /// NOTE: Calling this multiple times a frame can cause problems because character controller is not recommended to be called
    /// two times a frame.
    /// </summary>
    // TODO: This class now takes care of velocity based movement as well as direct movement by e.g. animation delta movement.
    // TODO C: Is this a good way to do this?
    public void UpdateMov(
        Vector2 horMovementInput,
        Vector3 animRootMotion,
        float maxLinSpd,
        float LinAcc,
        float maxAngSpd
    ){
        Vector2 xzMovementInput = new Vector2(horMovementInput.x, horMovementInput.y);
        horVel = Vector2.MoveTowards(
            horVel,
            horMovementInput * maxLinSpd,
            LinAcc * Time.deltaTime
        );
        RotateFwd(maxAngSpd, xzMovementInput);
        if (IsAffectedByGravity)
            ApplyGravity();
        else
            verVel = 0;
        animRootMotion.x += horVel.x * Time.deltaTime;
        animRootMotion.y += verVel * Time.deltaTime;
        animRootMotion.z += horVel.y * Time.deltaTime;
        charCtrl.Move(animRootMotion);
    }

    /// <summary>
    /// Snaps to linear and angular velocity.
    /// </summary>
    public void UpdateMov(
        Vector2 horMovInput,
        Vector3 animRootMot,
        float LinSpd,
        float AngSpd
    ){
        Vector2 xzMovementInput = new Vector2(horMovInput.x, horMovInput.y);
        horVel = horMovInput * LinSpd;
        RotateFwd(AngSpd, xzMovementInput);
        if (IsAffectedByGravity)
            ApplyGravity();
        else
            verVel = 0;
        animRootMot.x += horVel.x * Time.deltaTime;
        animRootMot.y += verVel * Time.deltaTime;
        animRootMot.z += horVel.y * Time.deltaTime;
        charCtrl.Move(animRootMot);
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check.
    /// </summary>
    // TODO: Make local variables into fields and reveal to inspector.
    // TODO: Idk if this is good. Maybe just do a sphere cast from capusle
    // TODO C: bottom to avoid hits with walls/ceilings?
    public bool IsGrounded(){
        float extraDist = 0.05f;
        float r = charCtrl.radius;
        float height = Mathf.Max(charCtrl.height, r * 2f);
        Vector3 center = charCtrl.transform.position + charCtrl.center;
        Vector3 bottom = center + Vector3.down * (height / 2f - r);
        Vector3 top = center + Vector3.up * (height / 2f - r);
        float castDist = extraDist + charCtrl.skinWidth;
        RaycastHit groundHit;
        bool hitGround = Physics.CapsuleCast(
            top,
            bottom,
            r,
            Vector3.down,
            out groundHit,
            castDist,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
        if (hitGround){
            float slopeAng = Vector3.Angle(groundHit.normal, Vector3.up);
            if( slopeAng <= charCtrl.slopeLimit)
                return true;
        }
        return false;
    }

    void ApplyGravity(){
        if (IsGrounded())
            verVel = 0;
        else
            verVel -= 9.81f * Time.deltaTime;
    }

    /// <summary>
    /// Rotates character towards the forward vector in xz-plane.
    /// </summary>
    /// <param name="fwd">In XZ-plane.</param>
    void RotateFwd(float maxAngSpd, Vector2 fwd){
        if (fwd == Vector2.zero)
            return;
        Vector3 dir3D = new Vector3(fwd.x, 0, fwd.y);
        Quaternion targetRotation = Quaternion.LookRotation(dir3D, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            maxAngSpd * Time.deltaTime
        );
    }
}
