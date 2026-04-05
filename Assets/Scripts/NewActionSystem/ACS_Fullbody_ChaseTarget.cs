using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.AI;

public class ACS_Fullbody_ChaseTarget : ACS_FullBody
{
    private NavMeshAgent _agent;
    //private Enemy_Controller _enemyController;
    private Transform _target;

    public ACS_Fullbody_ChaseTarget(IPawn pawn, Transform target, NavMeshAgent agent) : base(pawn)
    {
        //_enemyController = enemyController;
        _target = target;
        _agent = agent;
    }

    public override void BufferAction(ACS_ActionState newAction)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanBuffer(ACS_ActionState newAction)
    {
        return false;
    }

    public override bool CanInstantlyTransitionTo(ACS_ActionState newAction)
    {
        return true;
    }

    public override void EnterState()
    {
        //Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.RequestCrossfadeTo(Pawn.CustomAnimator.CharacterVisualsLayer_FullBody.Walk);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState(float deltaTime)
    {
        //Debug.Log("Went here");
        //Pawn.Movement.UpdateMovement(LocomotionType.VelocityByDirectionalInput, Pawn.MoveInput);
        //if (Pawn.AttackInput)
        //    Pawn.RequestFullBodyAction(new ACS_FullBody_Attack_JumpVerticalSlam(Pawn));
        //else if (Pawn.MoveInput == Vector2.zero)
        //    Pawn.RequestFullBodyAction(new ACS_FullBody_Idle(Pawn));

        // TODO: Think when to return failure.





        // NOTE: The knockback state is expected to be in the layer 0.
        //if (IsStunned() || IsAttacking())
        //{
        //    return NodeState.Failure;
        //}

        if (_target == null)
        {
            // TODO: Idle when target vanishes.
        }

        // NOTE: If the navmeshagent was stopped, it is set to not stopped in here.
        _agent.isStopped = false;
        _agent.SetDestination(_target.position);
    }
}
