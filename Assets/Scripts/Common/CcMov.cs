using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Movement for character controller.
/// </summary>
public static class CcMov {
    public static void ApplyGravityNSlideDownSlopes(int capsuleCharId, float dt){
        // TODO: This is cheating. Either use ref keywords, or take in all the arrays.
        CpMgr cpMgr = CpMgr.inst;
        if (cpMgr.data.isGrounded[capsuleCharId])
            cpMgr.data.vel_Ver[capsuleCharId] = -cpMgr.data.groundSnapVerDownSpd[capsuleCharId] * dt;
        // Freefalling and slope down sliding.
        else {
            cpMgr.data.vel_Ver[capsuleCharId] = cpMgr.data.lastCharCtrlVel[capsuleCharId].y;
            // Ground cast gave a result but the ground was too steep to be considered
            // "isGrounded" so slide down the slope instead.
            if (cpMgr.data.groundCastHitSomething[capsuleCharId]) {
                // TODO: Create float3 ProjectOnPlane math util.
                //math.down() - math.dot(math.down(), data.groundCastNrm) * data.groundCastNrm
                // TODO: We project last velocity onto the slope normalized direction (we divide by newAcc
                // TODO C: squared length to compensate for it's length, instead of just doing dir * dor(v, dir).
                // TODO C: Create math util.
                //float3 newVel = newAcc * (math.dot(data.lastCharCtrlVel, newAcc) / math.lengthsq(newAcc));


                // Find the gravitational acceleration component along the slope.
                // TODO: Create float3 ProjectOnPlane math util.
                float3 newAcc =
                    (math.down() - math.dot(
                        math.down(),
                        cpMgr.data.groundCastNrm[capsuleCharId]) * cpMgr.data.groundCastNrm[capsuleCharId]
                    )
                    * cpMgr.data.gravitationalAcc[capsuleCharId];
                float3 slideDir;
                // Normalization will give NaN if acceleration is zero unless we do this.
                if (math.lengthsq(newAcc) > 0.0001f)
                    slideDir = math.normalize(newAcc);
                else
                    slideDir = math.down();
                // We use the last velocitys component along the slope as last speed, though we
                // clamp it to disallow uphill sliding.
                float slideSpd = math.max(0, math.dot(cpMgr.data.lastCharCtrlVel[capsuleCharId], slideDir));
                float3 newVel = slideDir * slideSpd;
                newVel += newAcc * dt;
                cpMgr.data.vel_Ver[capsuleCharId] = newVel.y;
                cpMgr.data.vel_Hor[capsuleCharId] = new float2(newVel.x, newVel.z);
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
                cpMgr.data.vel_Ver[capsuleCharId] = Mathf.Min(cpMgr.data.vel_Ver[capsuleCharId], 0);
                cpMgr.data.vel_Ver[capsuleCharId] -= cpMgr.data.gravitationalAcc[capsuleCharId] * dt;
                cpMgr.data.vel_Ver[capsuleCharId] = Mathf.Clamp(
                    cpMgr.data.vel_Ver[capsuleCharId],
                    -cpMgr.data.maxFallSpd[capsuleCharId],
                    0
                );
                //Debug.Log("In free fall.");
            }
        }
    }

    /// <summary>
    /// Uses Physics.CapsuelCast to do a ground check. Returns true if cast hit something.
    /// </summary>
    // TODO: Maybe just do a sphere cast from capusle
    // TODO C: bottom to avoid hits with walls/ceilings?
    public static bool CastForGround(CharacterController cc, out RaycastHit groundHit) {
        float castDist = GlobalData.inst.data.isGroundedChkDist;
        float r = cc.radius;
        float height = Mathf.Max(cc.height, r * 2f);
        Vector3 center = cc.transform.position + cc.center;
        Vector3 bottom = center + Vector3.down * (height / 2f - r);
        Vector3 top = center + Vector3.up * (height / 2f - r);
        // TODO: I'm not 100% sure if SkinWidth should be used in here but it is very small so what ever.
        castDist = castDist + cc.skinWidth;
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
