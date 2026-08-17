using UnityEngine;

public class CapsuleCharHitRecieveHandler : MonoBehaviour, IHitReceiverOwner {
    [SerializeField] Pc pc;
    [SerializeField] HitReceiver bodyHitReciever;

    private void OnEnable() {
        bodyHitReciever.owner = this;
    }

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        CapsuleCharacterData data = pc.Data;
        data.curHp -= hitData.dmg;
        pc.Data = data;
        Debug.Log($"New HP: {pc.Data.curHp}", this);
        //if (pc.fsm.CurSt.CanSwitchStTo(hitStunState)) {
        //    pc.fsm.SwitchSt(hitStunSt);
        //}
        return new(false);
    }
}
