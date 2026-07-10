using UnityEngine;

/// <summary>
/// Playable character.
/// </summary>
public class PC : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time window (in seconds) before the end of an action/animation when new actions can be buffered." +
        " " +
        "\nNOTE: Is set to work with certain animation sample rates/speed/time scales.")]
    // TODO: Rename to action buffer time.
    [SerializeField] float _inputBufferTime = 0.25f;

    [Header("Refs")]
    public PC_BaseData baseData;
    public CharacterLocomotion Movement;
    public HealthSystem _health;
    public EquipmentController Equipment;
    public CapsuleCharacterVisualsComponents VisComponents;
    //public AN_CharacterVisuals _customAnimator;
    //[SerializeField] Animator _animator;
    //[SerializeField] AnimRootMvmtBroadcaster _rootMoveBroadcaster;

    // TODO: Separeate action controllers for hands and body corresponding to animator layers.
    //ActionController _handsActionController = new();
    //ActionController _bodyActionController = new();
    //public ActionController _fullBodyActionController = new();
    public FSM fSM;
    public FSM_PCStates fSMStates;

    // TODO: Rename to CustomAnimator.
    //public AN_CharacterVisuals CustomAnimator => _customAnimator;

    [HideInInspector] public Vector2 MoveInput = Vector2.zero;
    [HideInInspector] public bool AttackInput = false;
    [HideInInspector] public Vector3 AnimationDeltaMovement = Vector3.zero;

    //public P

    //CapsuleCharacterVisualsComponents IActionCharacter.CCVisComponents => VisComponents;

    //CapsuleCharacterVisualsComponents IActionCharacter.CCVisComponents => CCVisComponents;

    private void OnEnable()
    {
        VisComponents.rootMvmtBroadcaster.OnRootMove += OnAnimatorRootMove;
    }

    private void Start()
    {
        //_customAnimator = new(VisComponents.animator);

        // Enter initial state:
        //RequestFullBodyAction(new ACS_FullBody_Idle(this));
        fSM.SwitchState(fSMStates.idle);
    }

    private void FixedUpdate()
    {
        fSM.CurrentState.PhysicsTick();
    }

    private void Update()
    {
        fSM.CurrentState.Tick();
    }

    private void OnDisable()
    {
        VisComponents.rootMvmtBroadcaster.OnRootMove -= OnAnimatorRootMove;
    }

    // TODO: Create a PC_ControllerInput class with IPawn which can consume input from Controllers.
    public void UpdateInput(Vector2 moveInput, bool attackInput)
    {
        MoveInput = moveInput;
        AttackInput = attackInput;
    }

    //public void UpdateActionControllers()
    //{
    //    _fullBodyActionController.UpdateActionController(Time.deltaTime);
    //}

    //public ActionStateRequestResult RequestFullBodyAction(ACS_FullBody newAction)
    //{
    //    // TODO: Stop hand and body actions/blend them to "inactive" animations/actions.
    //    return _fullBodyActionController.RequestAction(newAction);
    //}

    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="deltaLinearMovement">
    /// Delta movement of animation root.
    /// </param>
    private void OnAnimatorRootMove(Vector3 deltaLinearMovement)
    {
        AnimationDeltaMovement = deltaLinearMovement;
    }

    //public bool CanReceiveHit(NewHitData hit)
    //{
    //    if (_isDead)
    //        return false;

    //    //if (_actionController.IsInvulnerable)
    //    //    return false;

    //    return true;
    //}

    //ActionDefinition ResolveReaction(NewHitData hit)
    //{
    //    // TODO:
    //    //if (health <= 0)
    //    //    return deathAction;

    //    //if (hit.sourceAction.priority >= ActionPriority.Knockdown)
    //    //    return knockdownAction;

    //    //return hitReactionAction;
    //    return null;
    //}

    // TODO: Move hyperarmor elsewhere
    //public bool HasHyperArmor()
    //{
    //    float t = ActionState.NormalizedTime;
    //    return t >= _currentAction.HyperArmorFrom &&
    //           t <= _currentAction.HyperArmorTo;
    //}
}
