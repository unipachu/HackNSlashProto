using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Movement for character controller.
/// </summary>
public class CharCtrlMov : MonoBehaviour{
    [Header("Refs")]
    [SerializeField] CharacterController charCtrl;
    [SerializeField] Pc pc;

    void ApplyGravityAndVerSlide(ref CapsuleCharData data, float dt){
        if (pc.Data.isGrounded)
            // TODO: Make this grounded velocity a scriptable object field. It is used
            // TODO C: to "snap" player to the ground if they are close enough to it.
            data.vel_ver = -1000 * dt;
        else {
            // TODO: Didn't you already have a gravity data field? Also you should not use this when sliding.
            data.vel_ver -= 9.81f * dt;
            float2 horDownSlopeDir = float2.zero;
            if (pc.Data.groundCastHitSomething) {
                // TODO: Use float3 so that you don't need to cast here.
                float slopeAng = Vector3.Angle(pc.groundCastResult.normal, Vector3.up);
                if (slopeAng > pc.charCtrl.slopeLimit) {
                    Vector3 downSlopeDir = Vector3.ProjectOnPlane(
                        Vector3.down,
                        pc.groundCastResult.normal
                    ).normalized;
                    horDownSlopeDir = new Vector2(downSlopeDir.x, downSlopeDir.z);
                }
            }
            data.vel_hor += horDownSlopeDir * 100 * dt;
        }
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check. Returns true if cast hit something.
    /// </summary>
    // TODO: Idk if this is good. Maybe just do a sphere cast from capusle
    // TODO C: bottom to avoid hits with walls/ceilings?
    public static bool CastForGround(CharacterController charCtrl, out RaycastHit groundHit) {
        float castDist = GlobalData.inst.data.groundChkDist;
        float r = charCtrl.radius;
        float height = Mathf.Max(charCtrl.height, r * 2f);
        Vector3 center = charCtrl.transform.position + charCtrl.center;
        Vector3 bottom = center + Vector3.down * (height / 2f - r);
        Vector3 top = center + Vector3.up * (height / 2f - r);
        castDist = castDist + charCtrl.skinWidth;
        return Physics.CapsuleCast(
            top,
            bottom,
            r,
            Vector3.down,
            out groundHit,
            castDist,
            GlobalData.inst.data.groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public static bool IsGrounded(
        CharacterController charCtrl,
        out bool groundCastHitSomething,
        out RaycastHit groundHit
    ) {
        groundCastHitSomething = CastForGround(charCtrl, out groundHit);
        if (groundCastHitSomething) {
            float slopeAng = Vector3.Angle(groundHit.normal, Vector3.up);
            if( slopeAng <= charCtrl.slopeLimit)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Ticks character controller movement.
    /// NOTE: Will snap to max linear speed if linear acceleration param is not not set!
    /// </summary>
    public void UpdateMov(
        Vector2 horMov,
        Vector3 animRootMot,
        float maxLinSpd,
        float yawSpd,
        // Should this be called hor acc instead?
        float linAcc = float.PositiveInfinity
    ) {
        float dt = Time.deltaTime;
        CapsuleCharData data = pc.Data;
        data.vel_hor = Vector2.MoveTowards(
            data.vel_hor,
            horMov * maxLinSpd,
            linAcc * dt
        );
        data.vel_yaw = yawSpd;
        TrfMathUtils.RotateFwdToTgt(transform, data.vel_yaw, horMov);
        if (data.isAffectedByGravity)
            ApplyGravityAndVerSlide(ref data, dt);
        else
            data.vel_ver = 0;
        Vector3 totalMov = animRootMot;
        totalMov.x += data.vel_hor.x * dt;
        totalMov.y += data.vel_ver * dt;
        totalMov.z += data.vel_hor.y * dt;
        pc.Data = data;
        charCtrl.Move(totalMov);
    }
}
