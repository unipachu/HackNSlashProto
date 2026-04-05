using UnityEngine;

/// <summary>
/// TODO: Test script for a character using the new action system.
/// </summary>
// TODO: Rename to character visuals controller. Or maybe CapsuleCharacterController
public class PlayerController : MonoBehaviour, IPawn
{
    [Header("Settings")]
    [Tooltip("Time window (in seconds) before the end of an action/animation when new actions can be buffered." +
        " " +
        "\nNOTE: Is set to work with certain animation sample rates/speed/time scales.")]
    // TODO: Rename to action buffer time.
    [SerializeField] float _inputBufferTime = 0.25f;
    public float InputBufferTime => _inputBufferTime;

    [Header("Refs")]
    [SerializeField] CharacterLocomotion _movement;
    public CharacterLocomotion Movement => _movement;
    [SerializeField] HealthSystem _health;
    [SerializeField] EquipmentController _equipment;
    public EquipmentController Equipment => _equipment;
    [SerializeField] Animator _animator;
    [SerializeField] CapsuleCharacterVisualsController _visualsController;

    // TODO: Separeate action controllers for hands and body corresponding to animator layers.
    //ActionController _handsActionController = new();
    //ActionController _bodyActionController = new();
    ActionController _fullBodyActionController = new();

    // TODO: Rename to CustomAnimator.
    AN_CharacterVisuals _customAnimator;
    public AN_CharacterVisuals CustomAnimator => _customAnimator;

    Vector2 _moveInput = Vector2.zero;
    public Vector2 MoveInput => _moveInput;
    bool _attackInput = false;
    public bool AttackInput => _attackInput;

    Vector3 _animationDeltaMovement = Vector3.zero;
    public Vector3 AnimationDeltaMovement => _animationDeltaMovement;

    public GameObject ThisObject => gameObject;

    private void OnEnable()
    {
        _visualsController.OnRootMove += OnAnimatorRootMove;
    }

    private void Start()
    {
        _customAnimator = new(_animator);

        // Enter initial state:
        RequestFullBodyAction(new ACS_FullBody_Idle(this));
    }

    private void OnDisable()
    {
        _visualsController.OnRootMove -= OnAnimatorRootMove;
    }

    public void UpdateInput(Vector2 moveInput, bool attackInput)
    {
        _moveInput = moveInput;
        _attackInput = attackInput;
    }

    public void UpdateActionControllers()
    {
        _fullBodyActionController.UpdateActionController(Time.deltaTime);
    }

    public ActionStateRequestResult RequestFullBodyAction(ACS_FullBody newAction)
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
    private void OnAnimatorRootMove(Vector3 deltaLinearMovement)
    {
        _animationDeltaMovement = deltaLinearMovement;
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
