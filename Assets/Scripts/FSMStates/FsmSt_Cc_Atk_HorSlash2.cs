using System;
using Unity.Mathematics;
using UnityEngine;

public class FsmSt_Cc_Atk_HorSlash2 : MonoBehaviour, IFsmSt {
    event Action<CapsuleCharAnimEvent> animEvent;

    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Impact;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    void OnEnable() {
        animEvent += OnAnimEvent;
    }

    void OnDisable() {
        animEvent -= OnAnimEvent;
    }

    public void Enter(IFsmSt previousState) {
        attackPhase = AtkPhase.Impact;
        comboAllowed = false;
        dodgeAllowed = false;
        impactInputRotationAllowed = false;
        recoveryMotionInterpTimer = 0;
        pc.inputBuffer.Clear();
        VisUtils.CrossfadeNInitAnimEventPlr(
            ref pc.animEventPlr,
            pc.capsuleCharAnim,
            CapsuleCharAnimInfo.atk_HorSlash2_Impact,
            animEvent
        );
    }

    public void Exit() {
        pc.weapon.hitDealer.Deactivate();
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        if (CapsuleCharFsmUtils.SwitchToFallingStIfNotGrounded(pc))
            return;
        switch (attackPhase) {
            case AtkPhase.Impact:
                float angSpd = 0;
                if (impactInputRotationAllowed) angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.Data.input_mov,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed) {
                    if (pc.inputBuffer.TryConsumeInput(BufferableInput.Atk_Light)) {
                        pc.fsm.SwitchSt(pc.fsmSts.atk_HorSlash3);
                    }
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.Data.input_mov,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_YawSpd * interpValue,
                    pc.Data.st_Walk_LinAcc
                );
                if (dodgeAllowed) {
                    // If input buffer has dodge, then transition to dodge state.
                }
                return;
            default:
                Debug.LogError("Switch defaulted.", this);
                return;
        }
    }

    public void LateTick() {
        pc.animEventPlr.Tick();
    }

    public bool CanSwitchStTo(IFsmSt newSt) => true;

    // ----------------------
    // Animation Event
    // ----------------------

    private void OnAnimEvent(CapsuleCharAnimEvent id) {
        switch (id) {
            case CapsuleCharAnimEvent.HorSlash2_Impact_RotationAllowed:
                impactInputRotationAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash2_Impact_RotationDisallowed:
                impactInputRotationAllowed = false;
                break;
            case CapsuleCharAnimEvent.HorSlash2_Impact_HitDealerActivated:
                pc.weapon.hitDealer.atkData = new(1, KnockbackT.Weak, 1);
                pc.weapon.hitDealer.hitWldDir = pc.transform.forward;
                pc.weapon.hitDealer.Activate();
                break;
            case CapsuleCharAnimEvent.HorSlash2_Impact_HitDealerDeactivated:
                pc.weapon.hitDealer.Deactivate();
                break;
            case CapsuleCharAnimEvent.HorSlash2_Impact_ComboAllowed:
                comboAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash2_Impact_ComboDisallowed:
                comboAllowed = false;
                break;
            case CapsuleCharAnimEvent.HorSlash2_Impact_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash2_Recovery,
                    animEvent
                );
                attackPhase = AtkPhase.Recovery;
                break;
            case CapsuleCharAnimEvent.HorSlash2_Recovery_DodgeAllowed:
                dodgeAllowed = true;
                break;
            case CapsuleCharAnimEvent.HorSlash2_Recovery_Finished:
                if (!pc.Data.input_mov.Equals(float2.zero))
                    pc.fsm.SwitchSt(pc.fsmSts.walk);
                else
                    pc.fsm.SwitchSt(pc.fsmSts.idle);
                return;
        }
    }
}
