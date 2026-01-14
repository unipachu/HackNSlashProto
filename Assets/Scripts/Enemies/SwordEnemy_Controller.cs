using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the functionality of the sword enemy. It represent the bodily functions and states of the enemy, where
/// as the ai or the "brains" of the enemy are controller by a separate class.
/// </summary>
[RequireComponent(typeof(NavMeshAgent), typeof(CharacterController), typeof(KnockBack))]
public class SwordEnemy_Controller : MonoBehaviour, IPlayerChaser, IHittable, IJumpAttacker
{
    [Header("Settings")]
    [SerializeField] private int _maxHealth = 3;

    [Header("Refs")]
    [SerializeField] private CustomAnimator_CharacterVisuals _customAnimator;
    [SerializeField] private CharacterController _characterController;
    
    private int _currentHealth;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private KnockBack _knockBack;

    public NavMeshAgent Agent => _agent;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    // TODO: Might make more sense to use enum/bool to check this to make sure that the "dying state" has been properly initialized.
    public bool IsDead => _currentHealth <= 0;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _knockBack = GetComponent<KnockBack>();
        _currentHealth = _maxHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerTransform = FindFirstObjectByType<PlayerController>().transform;
        Debug.Assert(_playerTransform != null, "_playerTransform of " + gameObject.name + "was null!", this);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDead)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _customAnimator.OnRootXZMotion += OnRootXZMotion;
    }

    private void UnsubscribeToEvents()
    {
        _customAnimator.OnRootXZMotion -= OnRootXZMotion;
    }

    // TODO: Root motion rotation through code doesn't seem to work well with the CharacterVisuals.
    // TODO C: Figure out a way to apply only linear xz root motion through code while animator handles y motion and rotation.
    private void OnRootXZMotion(Vector2 deltaXZMotion)
    {
        //Vector3 horizontalMotion = new Vector3(
        //    deltaXZMotion.x,
        //    0f,
        //    deltaXZMotion.y
        //);
        //_characterController.Move(horizontalMotion);
    }

    /// <summary>
    /// Used by the behavior tree to ask the sword enemy to follow the player.
    /// </summary>
    /// <returns>
    /// Returns Running if was able to start chasing the player or if already chasing the player, otherwise Failure.
    /// </returns>
    // TODO: Maybe this should be something like "Try chase player until in range, or something.
    public NodeState RequestChasePlayer()
    {
        // TODO: Think when to return failure.

        // NOTE: The knockback state is expected to be in the layer 0.
        if (IsStunned() || IsAttacking())
        {
            return NodeState.Failure;
        }

        // NOTE: If the navmeshagent was stopped, it is set to not stopped in here.
        Agent.isStopped = false;
        Agent.SetDestination(_playerTransform.position);
        if (!_customAnimator.IsActiveState(_customAnimator.WalkState))
            _customAnimator.RequestFixedTimeCrossfadeTo(_customAnimator.WalkState);
        return NodeState.Running;
    }

    public NodeState RequestIdle()
    {
        if (IsStunned() || IsAttacking())
        {
            return NodeState.Failure;
        }

        Agent.isStopped = true;
        if(!_customAnimator.IsActiveState(_customAnimator.IdleState))
            _customAnimator.RequestFixedTimeCrossfadeTo(_customAnimator.IdleState);
        return NodeState.Success;
    }

    public NodeState RequestJumpAttack()
    {
        if (IsStunned())
        {
            return NodeState.Failure;
        }

        Agent.isStopped = true;
        if (!_customAnimator.IsActiveState(_customAnimator.AttackJumpState))
            _customAnimator.RequestFixedTimeCrossfadeTo(_customAnimator.AttackJumpState);
        return NodeState.Success;
    }

    public void GetHit(int dmgAmount, Vector3 attackerPos)
    {
        Agent.isStopped = true;
        _customAnimator.RequestFixedTimeCrossfadeTo(_customAnimator.KnockBackBackwardState);
        _currentHealth -= dmgAmount;
        Vector3 knockBackDir = (transform.position - attackerPos).normalized;
        _knockBack.StartKnockBack(knockBackDir, 0.2f, 50);
    }

    /// <returns>
    /// If the character is in a state where it cannot move
    /// </returns>
    // TODO: Add death states and such.
    private bool IsStunned()
    {
        return _customAnimator.IsActiveState(_customAnimator.KnockBackBackwardState)
            || _knockBack.IsInKnockBack;
    }

    private bool IsAttacking()
    {
        return _customAnimator.IsActiveState(_customAnimator.AttackJumpState);
    }


}
