using System;
using Unity.Mathematics;
using UnityEngine;

public class FsmSt_Cc_Atk_ShootHomingProj : MonoBehaviour, IFsmSt {
    event Action<CapsuleCharAnimEvent> animEvent;

    [SerializeField] Pc cc;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState) {
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref cc.animEventPlr,
            cc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_GunShoot_Windup,
            animEvent
        );
    }

    public void Exit() {
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        // TODO: Add to So
        cc.charCtrlMov.UpdateMov(cc.Data.input_mov_LastNonZero, cc.AnimationDeltaMovement, 0, 180);
    }

    public void LateTick() {
        cc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt)
        => true;

    // ----------------------
    // Animation Event
    // ----------------------

    void OnAnimEvent(CapsuleCharAnimEvent id) {
        CapsuleCharData data = cc.Data;
        switch (id) {
            case CapsuleCharAnimEvent.Atk_GunShoot_Recovery_Finished:
                data.isAffectedByGravity = false;
                cc.Data = data;
                if (!cc.Data.input_mov.Equals(float2.zero))
                    cc.fsm.SwitchSt(cc.fsmSts.walk);
                else
                    cc.fsm.SwitchSt(cc.fsmSts.idle);
                break;
            case CapsuleCharAnimEvent.Atk_GunShoot_Windup_Finished:
                // TODO: Make into So data
                HomingProjMovData projData;
                AtkData atkData = new(10, KnockbackT.Weak, 0.5f);
                float spd = 5;
                float maxLifetime = 10;
                float homingStr = 2;
                projData = new(spd, maxLifetime, homingStr);
                HomingProjMgr.inst.ShootProj(
                    projData,
                    atkData,
                    cc.rHandEquippable.projSpawnPose.position,
                    cc.rHandEquippable.projSpawnPose.forward,
                    // TODO: Set homing target.
                    null
                );
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref cc.animEventPlr,
                    cc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_GunShoot_Recovery,
                    animEvent
                );
                break;
        }
    }
}
