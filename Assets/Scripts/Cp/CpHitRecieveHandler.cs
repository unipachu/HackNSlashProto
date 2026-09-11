using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
// TODO: This whole owner system is a little sketchy. Though hit reciever is a monobehavior and thus doesn't
// TODO C: support constructor dependency injection. You could make a custom script that allows you to pass
// TODO C: a hit reciever owner as a serialize field interface to the hit reciever.
// TODO C: Also does this class need to be a monobehavior at all?
public class CpHitRecieveHandler : MonoBehaviour, IHitReceiverOwner {
    [SerializeField] CpRegisterer pc;
    [SerializeField] HitReceiver bodyHitReciever;

    private void OnEnable() {
        bodyHitReciever.owner = this;
    }

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        int cpId = pc.Id;
        var data = CpMgr.inst.soaData;
        ref Cp_AosData aosData = ref CpMgr.inst.aosData[cpId];
        var classRefs = CpMgr.inst.classRefs[cpId];
        if (!data.invul[cpId]) {
            data.hp_Cur[cpId] -= hitData.atkData.dmg;
            //Debug.Log($"New HP: {pc.Data.curHp}", this);
            data.lastRecievedHitDir[cpId] = hitData.hitWldDir;
            data.lastKnockbackStr[cpId] = hitData.atkData.knockbackStr;
            //Debug.Log($"knockback str: {data.lastKnockbackStr[cpId]}.");
            switch (hitData.atkData.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    Vector3 horHitDir = new Vector3(
                        data.lastRecievedHitDir[cpId].x,
                        0,
                        data.lastRecievedHitDir[cpId].z
                    );
                    // If you, for some reason, set the hit direction to Vector3.zero.
                    if (horHitDir.sqrMagnitude < 0.0001f)
                        horHitDir = Vector3.down;
                    else
                        horHitDir.Normalize();
                    AnimInfo knockbackAnim;
                    if (Vector3.Dot(horHitDir, CpMgr.inst.unityComps[cpId].rootTrf.forward) > 0)
                        knockbackAnim = CpAnimInfo.Get(CpAnimInfoT.knockback_Weak_Fwd);
                    else
                        knockbackAnim = CpAnimInfo.Get(CpAnimInfoT.knockback_Weak_Bwd);
                    CpMgr.inst.TrySwitchActSt(
                        () => classRefs.actSts.knockback.Enter(knockbackAnim),
                        cpId
                    );
                    break;
                case KnockbackT.Strong:
                    Debug.LogError("Strong knockback not implemented!", this);
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(data.invul[cpId], false);
    }
}
