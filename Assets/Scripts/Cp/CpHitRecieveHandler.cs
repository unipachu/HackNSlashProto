using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
public class CpHitRecieveHandler : MonoBehaviour, IHitReceiverOwner {
    [SerializeField] CpRegisterer pc;
    [SerializeField] HitReceiver bodyHitReciever;

    private void OnEnable() {
        bodyHitReciever.owner = this;
    }

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        CpMgr ccMgr = CpMgr.inst;
        int id = pc.Id;
        if (!ccMgr.data.invul[id]) {
            ccMgr.data.hp_Cur[id] -= hitData.atkData.dmg;
            //Debug.Log($"New HP: {pc.Data.curHp}", this);
            ccMgr.data.lastRecievedHitDir[id] = hitData.hitWldDir;
            ccMgr.data.lastKnockbackStr[id] = hitData.atkData.knockbackStr;
            switch (hitData.atkData.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    if (ccMgr.ActSt_CanSwitchTo(CpActSt.Knockback_Weak))
                        ccMgr.ActSt_SwitchState(pc.Id, CpActSt.Knockback_Weak);
                    break;
                case KnockbackT.Strong:
                    // TODO: Try enter strong knockback state.
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(ccMgr.data.invul[id], false);
    }
}
