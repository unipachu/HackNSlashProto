using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TODO: Test script for a character using the new action system.
/// </summary>
// TODO: Rename to character visuals controller. Or maybe CapsuleCharacterController
public class NewPlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time window (in seconds) before the end of an action/animation when new actions can be buffered." +
        " " +
        "\nNOTE: Is set to work with certain animation sample rates/speed/time scales.")]
    [SerializeField] float _inputBufferTime = 0.25f;
    public float InputBufferTime => _inputBufferTime;

    [Header("Refs")]
    [SerializeField] PlayerMovement _movement;
    public PlayerMovement Movement => _movement;
    [SerializeField] HealthSystem _health;
    [SerializeField] EquipmentController _equipment;
    public EquipmentController Equipment => _equipment;
    [SerializeField] Animator _animator;
    [SerializeField] CapsuleCharacterVisualsController _visualsController;

    [Header("Input Related Refs")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] InputActionProperty _moveInputAction;
    [SerializeField] InputActionProperty _attackInputAction;

    // TODO: Separeate action controllers for hands and body corresponding to animator layers.
    //ActionController _handsActionController = new();
    //ActionController _bodyActionController = new();
    ActionController _fullBodyActionController = new();

    // TODO: Rename to CustomAnimator.
    AN_CharacterVisuals _customAnimator;
    public AN_CharacterVisuals CustomAnimator => _customAnimator;

    // TODO: Do I need to create instances of all the actions in here, or could I just create them as a new state is requested?
    ACS_FullBody_Idle _aCS_FullBody_Idle;
    public ACS_FullBody_Idle ACS_FullBody_Idle => _aCS_FullBody_Idle;
    ACS_FullBody_Attack_JumpVerticalSlam _aCS_FullBody_Attack_JumpVerticalSlam;
    public ACS_FullBody_Attack_JumpVerticalSlam ACS_FullBody_Attack_JumpVerticalSlam => _aCS_FullBody_Attack_JumpVerticalSlam;
    ACS_FullBody_Walk _aCS_FullBody_Walk;
    public ACS_FullBody_Walk ACS_FullBody_Walk => _aCS_FullBody_Walk;

    Vector2 moveInput = Vector2.zero;
    public Vector2 MoveInput => moveInput;
    bool attackInput = false;
    public bool AttackInput => attackInput;

    Vector3 _animationDeltaXZMovement = Vector2.zero;
    public Vector3 AnimationDeltaXZMovement => _animationDeltaXZMovement;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
        _visualsController.OnRootMove += OnAnimatorXZMove;
    }

    private void Start()
    {
        _customAnimator = new(_animator);

        _aCS_FullBody_Idle = new(this);
        _aCS_FullBody_Attack_JumpVerticalSlam = new(this);
        _aCS_FullBody_Walk = new(this);
        
        // Enter initial state:
        RequestFullBodyAction(_aCS_FullBody_Idle);
    }

    private void Update()
    {
        ReadInputs();

        _fullBodyActionController.UpdateActionController(Time.deltaTime);
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
        _visualsController.OnRootMove -= OnAnimatorXZMove;
    }

    private void ReadInputs()
    {
        moveInput = _moveInputAction.action.ReadValue<Vector2>();
        attackInput = _attackInputAction.action.WasPressedThisFrame();
    }

    public bool RequestFullBodyAction(ACS_FullBody newAction)
    {
        // TODO: Stop hand and body actions/blend them to "inactive" animations/actions.
        return _fullBodyActionController.RequestAction(newAction);
    }

    /// <summary>
    /// Used to save latest animation delta movement. Makes y component 0.
    /// </summary>
    /// <param name="deltaLinearMovement">
    /// Delta movement of animation root.
    /// </param>
    private void OnAnimatorXZMove(Vector3 deltaLinearMovement)
    {
        _animationDeltaXZMovement = new Vector3(deltaLinearMovement.x, 0, deltaLinearMovement.z);
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
