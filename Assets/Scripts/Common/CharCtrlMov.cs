using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Movement for character controller.
/// </summary>
public class CharCtrlMov : MonoBehaviour{
    [Header("Refs")]
    [SerializeField] CharacterController charCtrl;
    [SerializeField] Pc pc;

    static void ApplyGravityNSlideDownSlopes(ref CapsuleCharData data, float dt){
        if (data.isGrounded)
            data.vel_Ver = -data.groundSnapVerDownSpd * dt;
        // Freefalling and slope down sliding.
        else {
            data.vel_Ver = data.lastCharCtrlVel.y;
            // Ground cast gave a result but the ground was too steep to be considered
            // "isGrounded" so slide down the slope instead.
            if (data.groundCastHitSomething) {
                // TODO: Create float3 ProjectOnPlane math util.
                //math.down() - math.dot(math.down(), data.groundCastNrm) * data.groundCastNrm
                // TODO: We project last velocity onto the slope normalized direction (we divide by newAcc
                // TODO C: squared length to compensate for it's length, instead of just doing dir * dor(v, dir).
                // TODO C: Create math util.
                //float3 newVel = newAcc * (math.dot(data.lastCharCtrlVel, newAcc) / math.lengthsq(newAcc));


                // Find the gravitational acceleration component along the slope.
                // TODO: Create float3 ProjectOnPlane math util.
                float3 newAcc =
                    (math.down() - math.dot(math.down(), data.groundCastNrm) * data.groundCastNrm)
                    * data.gravitationalAcc;
                float3 slideDir = math.normalize(newAcc);
                // We use the last velocitys component along the slope as last speed, though we
                // clamp it to disallow uphill sliding.
                float slideSpd = math.max(0, math.dot(data.lastCharCtrlVel, slideDir));
                float3 newVel = slideDir * slideSpd;
                newVel += newAcc * dt;
                data.vel_Ver = newVel.y;
                data.vel_Hor = new float2(newVel.x, newVel.z);
                //Debug.Log($"ground normal: {data.groundCastNrm}");
                //float ang = math.degrees(math.acos(
                //        math.clamp(math.dot(data.groundCastNrm, math.up()), -1, 1)
                //    ));
                //Debug.Log($"angle deg: {ang}");
                //Debug.Log($"last char ctrl vel: {data.lastCharCtrlVel}");
                //Debug.Log($"New hor vel to apply: {data.vel_Hor}"
                //    + $"\n New ver vel to apply: {data.vel_Ver}");
                // No slope to slide down so free fall.
            } else {
                // NOTE: Character controller has a "step offset" functionality which can
                // NOTE C: cause the character to quickly snap upwards. If it enter falling
                // NOTE C: state right after this, it will gain huge upwards velocity. So
                // NOTE C: we clamp the vertical vel to min 0.
                data.vel_Ver = Mathf.Min(data.vel_Ver, 0);
                data.vel_Ver -= data.gravitationalAcc * dt;
                data.vel_Ver = Mathf.Clamp(data.vel_Ver, -data.maxFallSpd, 0);
                //Debug.Log("In free fall.");
            }
        }
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check. Returns true if cast hit something.
    /// </summary>
    // TODO: Maybe just do a sphere cast from capusle
    // TODO C: bottom to avoid hits with walls/ceilings?
    public static bool CastForGround(CharacterController charCtrl, out RaycastHit groundHit) {
        float castDist = GlobalData.inst.data.isGroundedChkDist;
        float r = charCtrl.radius;
        float height = Mathf.Max(charCtrl.height, r * 2f);
        Vector3 center = charCtrl.transform.position + charCtrl.center;
        Vector3 bottom = center + Vector3.down * (height / 2f - r);
        Vector3 top = center + Vector3.up * (height / 2f - r);
        // TODO: I'm not 100% sure if SkinWidth should be used in here but it is very small so what ever.
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

    /// <summary>
    /// NOTE: Character controller has its own isGrounded method.
    /// We use this to have custom custom distance for the ground cast, and also to have
    /// custom slope angle, since we likely want the character controller to more aggressively
    /// limit slope upwards movement when compared to when the character enters
    /// the falling state.
    /// </summary>
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
        data.vel_Hor = Vector2.MoveTowards(
            data.vel_Hor,
            horMov * maxLinSpd,
            linAcc * dt
        );
        data.vel_Yaw = yawSpd;
        TrfMathUtils.RotateFwdToTgt(transform, data.vel_Yaw, horMov);
        if (data.isAffectedByGravity)
            ApplyGravityNSlideDownSlopes(ref data, dt);
        else
            data.vel_Ver = 0;
        Vector3 totalMov = animRootMot;
        totalMov.x += data.vel_Hor.x * dt;
        totalMov.y += data.vel_Ver * dt;
        totalMov.z += data.vel_Hor.y * dt;
        pc.Data = data;
        charCtrl.Move(totalMov);
    }
}
