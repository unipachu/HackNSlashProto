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
                // TODO: sliding ver vel calculations are the same than freefall. Make a util function.
                data.vel_Ver = Mathf.Min(data.vel_Ver, 0);
                data.vel_Ver -= data.gravitationalAcc * dt;
                data.vel_Ver = Mathf.Clamp(data.vel_Ver, -data.maxFallSpd, 0);
                float3 downSlopeDir = math.normalize(
                    // We project ground normal to down vector plane.
                    // TODO: Create float3 ProjectOnPlane math util.
                    math.down() - math.dot(math.down(), data.groundCastNrm) * data.groundCastNrm
                );
                float slopeDirScl = data.vel_Ver / downSlopeDir.y;
                float3 slopeVel = downSlopeDir * slopeDirScl;
                float2 horVel = new float2(slopeVel.x, slopeVel.z);




                // TODO: Idk if this is a good idea but I'm trying to basically stop horizontal
                // TODO C: velocity from suddenly stopping when the character reaches an edge of
                // TODO C: a slope. This projects current horizontal velocity vector to the new
                // TODO C: velocity vector and chooses the one that is longer. Think about making
                // TODO C: a more realistic velocity when falling.
                float2 horVeldir = math.normalize(horVel);
                float2 velAlongSlope = horVeldir * math.dot(data.vel_Hor, horVeldir);
                data.vel_Hor = math.lengthsq(velAlongSlope) > math.lengthsq(horVel)
                    ? velAlongSlope
                    : horVel;

                // make sure vel hor isn't very small:
                if(math.lengthsq(data.vel_Hor) < 0.0001f)
                    data.vel_Hor = math.normalize(data.vel_Hor) * 0.0001f;

                Debug.Log($"ground normal: {data.groundCastNrm}");
                Debug.Log($"New hor vel to apply: {data.vel_Hor}"
                    + $"\n New ver vel to apply: {data.vel_Ver}");
                    
            // No slope to slide down so free fall.
            } else {
                // NOTE: Character controller has a "step offset" functionality which can
                // NOTE C: cause the character to quickly snap upwards. If it enter falling
                // NOTE C: state right after this, it will gain huge upwards velocity. So
                // NOTE C: we clamp the vertical vel to min 0.
                data.vel_Ver = Mathf.Min(data.vel_Ver, 0);
                data.vel_Ver -= data.gravitationalAcc * dt;
                data.vel_Ver = Mathf.Clamp(data.vel_Ver, -data.maxFallSpd, 0);
                Debug.Log("In free fall.");
            }
        }
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check. Returns true if cast hit something.
    /// </summary>
    // TODO: Idk if this is good. Maybe just do a sphere cast from capusle
    // TODO C: bottom to avoid hits with walls/ceilings?
    public static bool CastForGround(CharacterController charCtrl, out RaycastHit groundHit) {
        float castDist = GlobalData.inst.data.isGroundedChkDist;
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
