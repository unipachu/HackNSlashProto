using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Movement for character controller.
/// </summary>
public static class CcMov {
    /// <summary>
    /// Moves the character controller downwards, simulating gravity. If grounded check fails but there's
    /// still ground beneath the capsule (because floor is too steep to walk on) this slides the CC downhill.
    /// </summary>
    public static void ApplyGravityNSlideDownSlopes(int capsuleCharId, float dt){
        if (CpMgr.GetAos(capsuleCharId).isGrounded)
            CpMgr.GetAos(capsuleCharId).vel_Ver = -CpMgr.GetAos(capsuleCharId).groundSnapVerDownSpd * dt;
        // Freefalling and slope down sliding.
        else {
            CpMgr.GetAos(capsuleCharId).vel_Ver = CpMgr.GetAos(capsuleCharId).lastCcVel.y;
            // Ground cast gave a result but the ground was too steep to be considered
            // "isGrounded" so slide down the slope instead.
            if (CpMgr.GetAos(capsuleCharId).groundCastHitSomething) {
                // Find the gravitational acceleration component along the slope.
                float3 newAcc = math.down().ProjectOnPlane(CpMgr.GetAos(capsuleCharId).groundCastNrm)
                    * GlobalData.inst.gravitationalAcc;
                float3 slideDir;
                // Normalization will give NaN if acceleration is zero unless we do this.
                if (math.lengthsq(newAcc) > 0.0001f)
                    slideDir = math.normalize(newAcc);
                else
                    slideDir = math.down();
                // We use the last velocitys component along the slope as last speed, though we
                // clamp it to disallow uphill sliding.
                float slideSpd = math.max(0, math.dot(CpMgr.GetAos(capsuleCharId).lastCcVel, slideDir));
                float3 newVel = slideDir * slideSpd;
                newVel += newAcc * dt;
                CpMgr.GetAos(capsuleCharId).vel_Ver = newVel.y;
                CpMgr.GetAos(capsuleCharId).vel_Hor = new float2(newVel.x, newVel.z);
                //Debug.Log($"ground normal: {data.groundCastNrm}");
                //float ang = math.degrees(math.acos(
                //        math.clamp(math.dot(data.groundCastNrm, math.up()), -1, 1)
                //    ));
                //Debug.Log($"angle deg: {ang}");
                //Debug.Log($"last char ctrl vel: {data.lastCharCtrlVel}");
                //Debug.Log($"New hor vel to apply: {soaData.vel_Hor}\nNew ver vel to apply: {soaData.vel_Ver}");
                // No slope to slide down so free fall.
            } else {
                // NOTE: Character controller has a "step offset" functionality which can
                // NOTE C: cause the character to quickly snap upwards. If it enter falling
                // NOTE C: state right after this, it will gain huge upwards velocity. So
                // NOTE C: we clamp the vertical vel to min 0. I'm pretty sure it's like this.
                CpMgr.GetAos(capsuleCharId).vel_Ver = Mathf.Min(CpMgr.GetAos(capsuleCharId).vel_Ver, 0);
                CpMgr.GetAos(capsuleCharId).vel_Ver -= GlobalData.inst.gravitationalAcc * dt;
                CpMgr.GetAos(capsuleCharId).vel_Ver = Mathf.Clamp(
                    CpMgr.GetAos(capsuleCharId).vel_Ver,
                    -GlobalData.inst.maxFallSpd,
                    0
                );
                //Debug.Log("In free fall.");
            }
        }
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check. Returns true if cast hit something.
    /// </summary>
    public static bool CastForGround(CharacterController cc, out RaycastHit groundHit) {
        float castDist = GlobalData.inst.isGroundedChkDist;
        float r = cc.radius;
        float height = Mathf.Max(cc.height, r * 2f);
        Vector3 center = cc.transform.position + cc.center;
        Vector3 bottom = center + Vector3.down * (height / 2f - r);
        return Physics.SphereCast(
            bottom,
            r,
            Vector3.down,
            out groundHit,
            castDist,
            GlobalData.inst.groundMask,
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
        CharacterController cc,
        out bool groundCastHitSomething,
        out RaycastHit groundHit
    ) {
        groundCastHitSomething = CastForGround(cc, out groundHit);
        //Debug.Log($"Ground cast hit something: {groundCastHitSomething}");
        if (groundCastHitSomething) {
            float slopeAng = Vector3.Angle(groundHit.normal, Vector3.up);
            if( slopeAng <= cc.slopeLimit)
                return true;
        }
        return false;
    }
}
