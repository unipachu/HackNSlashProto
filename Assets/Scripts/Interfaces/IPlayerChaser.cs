using System;
using UnityEngine.AI;

/// <summary>
/// Used for objects that can chase the player with nav mesh agent.
/// </summary>
[Obsolete]
public interface IPlayerChaser
{
    NavMeshAgent Agent { get; }

    NodeState RequestChasePlayer();

    NodeState RequestIdle();
}
