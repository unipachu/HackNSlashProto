using System;
using UnityEngine;

public class FsmSt_Pc_Atk_HorSlash3 : MonoBehaviour, IFsmSt {
    event Action<string> animEvent;

    [SerializeField] Pc pc;

    AtkPhase attackPhase = AtkPhase.Impact;
    bool comboAllowed = false;
    bool dodgeAllowed = false;
    bool impactInputRotationAllowed = false;
    float recoveryMotionInterpTimer = 0;

    // Animation event ids
    const string Impact_ComboAllowed = "Impact_ComboAllowed";
    const string Impact_ComboDisallowed = "Impact_ComboDisallowed";
    const string Impact_Finished = "Impact_Finished";
    const string Impact_HitDealerActivated = "Impact_HitDealerActivated";
    const string Impact_HitDealerDeactivated = "Impact_HitDealerDeactivated";
    const string Impact_RotationAllowed = "Impact_RotationAllowed";
    const string Impact_RotationDisallowed = "Impact_RotationDisallowed";
    const string Recovery_DodgeAllowed = "Recovery_DodgeAllowed";
    const string Recovery_Finished = "Recovery_Finished";

    ActAnimEvent[] animEvents_Impact = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_HorSlash3_Impact,
        (0, Impact_RotationAllowed),
        (4, Impact_RotationDisallowed),
        (8, Impact_HitDealerActivated),
        (18, Impact_HitDealerDeactivated),
        (22, Impact_ComboAllowed),
        (25, Impact_ComboDisallowed),
        (26, Impact_Finished)
    );
    // TODO: Set the correct event frames.
    ActAnimEvent[] animEvents_Recovery = VisUtils.CreateAnimEvents(
        CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
        (6, Recovery_DodgeAllowed),
        (18, Recovery_Finished)
    );

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
            CapsuleCharAnimInfo.atk_HorSlash3_Impact,
            animEvents_Impact,
            animEvent
        );
    }

    public void Exit() {
        // TODO: Deactivate HitDealers.
    }

    public void PhysicsTick() {
    }

    public void Tick() {
        switch (attackPhase) {
            case AtkPhase.Impact:
                float angSpd = 0;
                if (impactInputRotationAllowed)
                    angSpd = pc.Data.st_AtkHorSlash_Impact_AngSpd;
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    pc.AnimationDeltaMovement,
                    0,
                    angSpd);
                if (comboAllowed) {
                    //if (pc.inputBuffer.ConsumeInput("atk1"))
                    //{
                    //    pc.fSM.SwitchState(pc.fSMStates.atk_HorSlash2);
                    //}
                }
                return;
            case AtkPhase.Recovery:
                // interpolate to walking speed.
                recoveryMotionInterpTimer += Time.deltaTime;
                float interpValue = Mathf.Clamp01(recoveryMotionInterpTimer / 0.2f);
                pc.charCtrlMov.UpdateMov(
                    pc.MoveInput,
                    Vector3.zero,
                    pc.Data.st_Walk_MaxLinSpd * interpValue,
                    pc.Data.st_Walk_LinAcc,
                    pc.Data.st_Walk_MaxAngSpd * interpValue);
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

    // ----------------------
    // Animation Event
    // ----------------------

    private void OnAnimEvent(string id) {
        switch (id) {
            case Impact_ComboAllowed:
                comboAllowed = true;
                break;
            case Impact_ComboDisallowed:
                comboAllowed = false;
                break;
            case Impact_Finished:
                VisUtils.CrossfadeNInitAnimEventPlr(
                    ref pc.animEventPlr,
                    pc.capsuleCharAnim,
                    CapsuleCharAnimInfo.atk_HorSlash1_Recovery,
                    animEvents_Recovery,
                    animEvent
                );
                attackPhase = AtkPhase.Recovery;
                break;
            case Impact_HitDealerActivated:
                // TODO: Activate HitDealer.
                break;
            case Impact_HitDealerDeactivated:
                // TODO: Deactivate HitDealer.
                break;
            case Impact_RotationAllowed:
                impactInputRotationAllowed = true;
                break;
            case Impact_RotationDisallowed:
                impactInputRotationAllowed = false;
                break;
            case Recovery_DodgeAllowed:
                dodgeAllowed = true;
                break;
            case Recovery_Finished:
                pc.fsm.SwitchSt(pc.fsmSts.idle);
                break;
        }
    }
}
