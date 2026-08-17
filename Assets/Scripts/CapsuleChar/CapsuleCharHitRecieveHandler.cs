using UnityEngine;

public class CapsuleCharHitRecieveHandler : MonoBehaviour, IHitReceiverOwner {
    [SerializeField] Pc pc;
    [SerializeField] HitReceiver bodyHitReciever;



    private void OnEnable() {
        bodyHitReciever.owner = this;
    }

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        CapsuleCharacterData data = pc.Data;
        data.curHp -= hitData.atkData.dmg;
        data.lastRecievedHitDir = hitData.hitWldDir;
        data.lastKnockbackStr = hitData.atkData.knockbackStr;
        pc.Data = data;
        //Debug.Log($"New HP: {pc.Data.curHp}", this);
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
        return new(false);
    }
}
