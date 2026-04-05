using UnityEngine;
using UnityEngine.AI;

// TODO:
public class Enemy_Controller : MonoBehaviour, IPawn
{
    [Header("Refs")]
    [SerializeField] CharacterLocomotion _movement;
    public CharacterLocomotion Movement => _movement;
    [SerializeField] HealthSystem _health;
    [SerializeField] EquipmentController _equipment;
    public EquipmentController Equipment => _equipment;
    [SerializeField] Animator _animator;
    [SerializeField] CapsuleCharacterVisualsController _visualsController;
    public Transform PlayerTransform; // TODO: A better player/target sensing system should be created.
    public NavMeshAgent agent;

    ACS_FullBody_Idle _aCS_FullBody_Idle;
    public ACS_FullBody_Idle ACS_FullBody_Idle => _aCS_FullBody_Idle;
    ACS_FullBody_Attack_JumpVerticalSlam _aCS_FullBody_Attack_JumpVerticalSlam;
    public ACS_FullBody_Attack_JumpVerticalSlam ACS_FullBody_Attack_JumpVerticalSlam => _aCS_FullBody_Attack_JumpVerticalSlam;
    ACS_FullBody_Walk _aCS_FullBody_Walk;
    public ACS_FullBody_Walk ACS_FullBody_Walk => _aCS_FullBody_Walk;



    public AN_CharacterVisuals CustomAnimator => throw new System.NotImplementedException();

    public float InputBufferTime => throw new System.NotImplementedException();

    public bool AttackInput => throw new System.NotImplementedException();

    public Vector2 MoveInput => throw new System.NotImplementedException();

    public GameObject ThisObject => throw new System.NotImplementedException();

    public Vector3 AnimationDeltaMovement => throw new System.NotImplementedException();

    ActionController _fullBodyActionController = new();

    private void Update()
    {
        UpdateActionControllers();
    }

    public ActionStateRequestResult RequestFullBodyAction(ACS_FullBody newAction)
    {
        return _fullBodyActionController.RequestAction(newAction);
    }

    public void UpdateActionControllers()
    {
        _fullBodyActionController.UpdateActionController(Time.deltaTime);
    }

    public NodeState RequestAttack()
    {
        return NodeState.Success;
    }

    public NodeState RequestChainAttack()
    {
        return NodeState.Success;
    }

    public NodeState RequestAttackFinisher()
    {
        return NodeState.Success;
    }
}
