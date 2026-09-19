using UnityEngine;

/// <summary>
/// Resolves recieved hits foe a capsule pawn.
/// </summary>
public class CpHitReciever : MonoBehaviour, IHitReceiver {
    [SerializeField] CpRegisterer cp;

    public HitResult ReceiveHit(HitDealer hitDealer, HitData hitData) {
        //Debug.Log(
        //    $"HitData:\n" +
        //    $"  atkData.dmg: {hitData.atkData.dmg}\n" +
        //    $"  atkData.knockbackT: {hitData.atkData.knockbackT}\n" +
        //    $"  atkData.knockbackStr: {hitData.atkData.knockbackStr}\n" +
        //    $"  hitWldDir: {hitData.hitWldDir}"
        //);
        int cpId = this.cp.Id;
        var classRefs = CpMgr.inst.classRefs[cpId];
        var cp = CpMgr.inst.cp[cpId];
        if (!CpMgr.GetAos(cpId).invul) {
            CpMgr.GetAos(cpId).hp_Cur -= hitData.atkData.dmg;
            //Debug.Log($"New HP: {pc.Data.curHp}", this);
            CpMgr.GetAos(cpId).lastRecievedHitDir = hitData.hitWldDir;
            CpMgr.GetAos(cpId).lastKnockbackStr = hitData.atkData.knockbackStr;
            //Debug.Log($"knockback str: {data.lastKnockbackStr[cpId]}.");
            switch (hitData.atkData.knockbackT) {
                case KnockbackT.None:
                    break;
                case KnockbackT.Weak:
                    Vector3 horHitDir = new Vector3(
                        CpMgr.GetAos(cpId).lastRecievedHitDir.x,
                        0,
                        CpMgr.GetAos(cpId).lastRecievedHitDir.z
                    );
                    // If you, for some reason, set the hit direction to Vector3.zero.
                    if (horHitDir.sqrMagnitude < 0.0001f)
                        horHitDir = Vector3.down;
                    else
                        horHitDir.Normalize();
                    AnimInfo knockbackAnim;
                    if (Vector3.Dot(horHitDir, cp.transform.forward) > 0)
                        knockbackAnim = CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Fwd);
                    else
                        knockbackAnim = CpAnimInfoFactory.Construct(CpAnimInfoT.knockback_Weak_Bwd);
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
        return new(CpMgr.GetAos(cpId).invul, false);
    }
}
