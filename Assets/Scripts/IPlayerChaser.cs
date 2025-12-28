using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Used for objects that can be moved with nav mesh agent.
/// </summary>
public interface IPlayerChaser
{
    NavMeshAgent Agent { get; }

    NodeState RequestChasePlayer();

    NodeState RequestIdle();
}
