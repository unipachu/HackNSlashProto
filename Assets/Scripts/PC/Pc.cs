using UnityEngine;

/// <summary>
/// Capsule character.
/// </summary>
// TODO: Rename to Cc.
public class Pc : MonoBehaviour{
    [Header("Data Refs")]
    [SerializeField] CapsuleCharRegisterer capsuleCharRegisterer;

    [Header("Component Refs")]
    public CapsuleCharCtrl ctrl;
    public CharCtrlMov charCtrlMov;
    public Fsm fsm;
    public Fsm_PcSts fsmSts;
    public PcInputBuffer inputBuffer;
    public AnimRootMovBroadcaster capsuleCharRootMvmtBroadcaster;
    public Animator capsuleCharAnim;
    public CapsuleCharWeapon weapon;
    public CapsuleCharHitRecieveHandler hitRecieverHandler;
    public CharacterController charCtrl;
    
    const float movInputDeadzone = 0.2f;

    /// <summary>
    /// Used to create animation events decoupled from the Animator.
    /// </summary>
    [HideInInspector] public AnimEventPlr animEventPlr;

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
        var data = Data;
        data.trf_pos = transform.position;
        data.trf_rot = transform.rotation;
        data.trf_lossyScl = transform.lossyScale;
        data.lastCharCtrlVel = charCtrl.velocity;
        data.curStDur += Time.deltaTime;
        UpdateInput(ref data, ctrl, inputBuffer);
        Data = data;
        fsm.CurSt.Tick();
    }

    void LateUpdate() {
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
            out RaycastHit groundCastResult
        );
        data.groundCastNrm = groundCastResult.normal;
        pc.Data = data;
    }

    public static void UpdateInput(ref CapsuleCharData data, CapsuleCharCtrl ctrl, PcInputBuffer inputBuffer){
        data.input_atk_Light = ctrl.TryConsume_Atk_Light();
        data.input_atk_Heavy = ctrl.TryConsume_Atk_Heavy();
        data.input_atk_Ult = ctrl.TryConsume_Atk_Ult();
        data.input_dodge = ctrl.TryConsume_Dodge();
        if (ctrl.Input_Mov.sqrMagnitude > movInputDeadzone) {
            data.input_mov = ctrl.Input_Mov;
            data.input_mov_LastNonZero = data.input_mov;
        } else {
            data.input_mov = Vector2.zero;
        }
        // Buffer certain inputs
        if (data.input_atk_Light)
            inputBuffer.BufferInput(BufferableInput.Atk_Light);
        else if (data.input_atk_Heavy)
            inputBuffer.BufferInput(BufferableInput.Atk_Heavy);
        else if (data.input_atk_Ult)
            inputBuffer.BufferInput(BufferableInput.Atk_Ult);
        else if (data.input_dodge)
            inputBuffer.BufferInput(BufferableInput.Dodge);
        //Debug.Log("mov input mag: " + data.mov.magnitude);
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
        CapsuleCharData data = Data;
        data.curStDur = 0;
        if (ctrl.Input_Mov.sqrMagnitude > movInputDeadzone)
            data.input_mov_WhenLastSwitchedSt = data.input_mov;
        else
            data.input_mov_WhenLastSwitchedSt = Vector2.zero;
        Data = data;
    }
}
