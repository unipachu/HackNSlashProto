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
            switch (hitData.atkData.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    Fsm.TrySwitchState(
                        () => classRefs.actSts.knockback.Enter(
                            CpAnimInfo.knockback_Weak_Fwd,
                            CpAnimInfo.knockback_Weak_Bwd
                        ),
                        ref classRefs.st_cur,
                        ref classRefs.st_prev,
                        ref aosData.isSwitchingSt,
                        aosData.enableDebugMsgs
                    );
                    break;
                case KnockbackT.Strong:
                    // TODO: Try enter strong knockback state.
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(data.invul[cpId], false);
    }
}
