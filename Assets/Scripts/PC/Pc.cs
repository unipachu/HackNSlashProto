using UnityEngine;

/// <summary>
/// Playable character.
/// </summary>
public class Pc : MonoBehaviour{
    [Header("Data Refs")]
    [SerializeField] CapsuleCharRegisterer capsuleCharRegisterer;

    [Header("Component Refs")]
    public CharCtrlMov charCtrlMov;
    public Fsm fsm;
    public Fsm_PcSts fsmSts;
    public PcInputBuffer inputBuffer;
    public AnimRootMovBroadcaster capsuleCharRootMvmtBroadcaster;
    public Animator capsuleCharAnim;
    public CapsuleCharWeapon weapon;
    public CapsuleCharHitRecieveHandler hitRecieverHandler;
    public PlrCtrl plrCtrl;
    public CamMgr camMgr;
    public CharacterController charCtrl;

    const float movInputDeadzone = 0.2f;

    /// <summary>
    /// Used to create animation events decoupled from the Animator.
    /// </summary>
    [HideInInspector] public AnimEventPlr animEventPlr;

    // TODO: Move to native array in capsule char data.
    public CapsuleCharInputData inputData;
    // TODO C: Nonnative types. Maybe these could be moved to a struct as well (but use
    // TODO C: the struct here instead of in the capsule char mgr
    public RaycastHit groundCastResult;

    public CapsuleCharData Data {
        get => capsuleCharRegisterer.Data;
        set => capsuleCharRegisterer.Data = value;
    }
    public Vector3 AnimationDeltaMovement { get; private set; }

    void OnEnable(){
        capsuleCharRootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
        fsm.StSwitched += OnStSwitched;
    }

    void Start(){
        // Enter initial state:
        fsm.SwitchSt(fsmSts.idle);
    }

    void FixedUpdate(){
        UpdateGroundCheck(this);
        fsm.CurSt.PhysicsTick();
    }

    void Update() {
        UpdateInput();
        fsm.CurSt.Tick();
    }

    private void LateUpdate() {
        fsm.CurSt.LateTick();
    }

    void OnDisable(){
        capsuleCharRootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
        fsm.StSwitched -= OnStSwitched;
    }

    public static void UpdateGroundCheck(Pc pc) {
        var data = pc.Data;
        data.isGrounded = CharCtrlMov.IsGrounded(
            pc.charCtrl,
            out data.groundCastHitSomething,
            out pc.groundCastResult
        );
        pc.Data = data;
    }

    void UpdateInput(){
        inputData.atk_Light = plrCtrl.TryConsume_Atk_Light();
        inputData.atk_Heavy = plrCtrl.TryConsume_Atk_Heavy();
        inputData.atk_Ult = plrCtrl.TryConsume_Atk_Ult();
        inputData.dodge = plrCtrl.TryConsume_Dodge();
        if (plrCtrl.Input_Mov.sqrMagnitude > movInputDeadzone) {
            inputData.mov = plrCtrl.Input_Mov;
            inputData.mov_CamRel = MathUtils.TrfInputByBasis(inputData.mov, camMgr.CamFwdDir);
            inputData.mov_LastNonZero = inputData.mov;
            inputData.mov_LastNonZero_CamRel = MathUtils.TrfInputByBasis(
                inputData.mov,
                camMgr.CamFwdDir
            );
        } else {
            inputData.mov = Vector2.zero;
            inputData.mov_CamRel = Vector2.zero;
        }
        // Buffer certain inputs
        if (inputData.atk_Light)
            inputBuffer.BufferInput(BufferableInput.Atk_Light);
        else if (inputData.atk_Heavy)
            inputBuffer.BufferInput(BufferableInput.Atk_Heavy);
        else if (inputData.atk_Ult)
            inputBuffer.BufferInput(BufferableInput.Atk_Ult);
        else if (inputData.dodge)
            inputBuffer.BufferInput(BufferableInput.Dodge);
    }

    // ---------------------------------------------------------------
    // Event Callbacks
    // ---------------------------------------------------------------

    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="deltaLinearMovement">
    /// Delta movement of animation root.
    /// </param>
    void OnAnimatorRootMove(Vector3 deltaLinearMovement){
        AnimationDeltaMovement = deltaLinearMovement;
    }

    void OnStSwitched() {
        //Debug.Log($"Mov input when st switched: {inputData.mov}.");
        if (plrCtrl.Input_Mov.sqrMagnitude > movInputDeadzone) {
            inputData.mov_WhenLastSwitchedSt = inputData.mov;
            inputData.mov_WhenLastSwitchedSt_CamRel = MathUtils.TrfInputByBasis(
                inputData.mov,
                camMgr.CamFwdDir
            );
        } else {
            inputData.mov_WhenLastSwitchedSt = Vector2.zero;
            inputData.mov_WhenLastSwitchedSt_CamRel = Vector2.zero;
        }
    }
}
