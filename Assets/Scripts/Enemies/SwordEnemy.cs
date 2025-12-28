using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CharacterController), typeof(KnockBack))]
public class SwordEnemy : MonoBehaviour, IPlayerChaser, IHittable
{
    [Header("Settings")]
    [SerializeField] private int _maxHealth = 3;

    [Header("Refs")]
    [SerializeField] private CharacterVisualsAnimationController _characterVisualsaAnimationController;
    
    private int _currentHealth;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private KnockBack _knockBack;

    public NavMeshAgent Agent => _agent;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    // TODO: Might make more sense to use enum/bool to check this to make sure that the "dying state" has been properly initialized.
    public bool IsDead => _currentHealth <= 0;

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
        Debug.Assert(_playerTransform != null, "_playerTransform of " + gameObject.name + "was null!");
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
        if (_characterVisualsaAnimationController.IsPlaying_KnockBackBackward()) return NodeState.Failure;

        // NOTE: If the navmeshagent was stopped, it is set to not stopped in here.
        Agent.isStopped = false;
        Agent.SetDestination(_playerTransform.position);
        //_characterVisualsaAnimationController.Play_Walk();
        return NodeState.Running;
    }

    public NodeState RequestIdle()
    {
        if (_characterVisualsaAnimationController.IsPlaying_KnockBackBackward()) return NodeState.Failure;

        Agent.isStopped = true;
        //_characterVisualsaAnimationController.Play_Idle();
        return NodeState.Success;
    }

    public void GetHit(int dmgAmount, Vector3 attackerPos)
    {
        Agent.isStopped = true;
        _characterVisualsaAnimationController.Play_KnockBackBackward();
        _currentHealth -= dmgAmount;
        Vector3 knockBackDir = (transform.position - attackerPos).normalized;
        _knockBack.StartKnockBack(knockBackDir, 0.2f, 50);
    }
}
