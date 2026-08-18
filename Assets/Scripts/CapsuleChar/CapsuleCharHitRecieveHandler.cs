using UnityEngine;

public class CapsuleCharHitRecieveHandler : MonoBehaviour, IHitReceiverOwner {
    [SerializeField] Pc pc;
    [SerializeField] HitReceiver bodyHitReciever;

    private void OnEnable() {
        bodyHitReciever.owner = this;
    }

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        if (!pc.Data.invul) {
            CapsuleCharData data = pc.Data;
            data.hp_Cur -= hitData.atkData.dmg;
            //Debug.Log($"New HP: {pc.Data.curHp}", this);
            data.lastRecievedHitDir = hitData.hitWldDir;
            data.lastKnockbackStr = hitData.atkData.knockbackStr;
            pc.Data = data;
            switch (hitData.atkData.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    if (pc.fsm.CurSt.CanSwitchStTo(pc.fsmSts.knockback_Weak))
                        pc.fsm.SwitchSt(pc.fsmSts.knockback_Weak);
                    break;
                case KnockbackT.Strong:
                    // TODO: Try enter strong knockback state.
                    break;
                default:
                    Debug.LogError("Switch defaulted", this);
                    break;
            }
        }
        return new(pc.Data.invul, false);
    }
}
