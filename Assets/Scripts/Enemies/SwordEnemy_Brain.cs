using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behavior tree of the sword enemy.
/// </summary>
public class SwordEnemy_Brain : BT_Tree
{
    // TODO: Possibly set member variables in constructor and create this in the SwordEnemy controller script.
    [SerializeField] private float _chaseMinDist;
    [SerializeField] private float _chaseMaxDist;
    // TODO: Change to "_targetTransform"
    [SerializeField] private Transform _playerTransform;

    protected override BT_Node SetupTree()
    {
        BT_Node root = new BT_Selector(new List<BT_Node>
        {
            new BT_IsDead(GetComponent<IHittable>()),
            new BT_Selector(new List<BT_Node> {
                // TODO: check if in attack range, if so then attack and exit selector.
                // TODO: Cooldown node?
                // TODO: Random node to check whether to do one or two hit combo?
                new BT_Sequence(new List<BT_Node>
                {
                    new BT_IsInRange(_chaseMinDist, _chaseMaxDist, transform, _playerTransform),
                    new BT_ChasePlayer(GetComponent<IPlayerChaser>())
                }),
                new BT_Sequence(new List<BT_Node>
                {
                    new BT_IsInRange(0, _chaseMinDist, transform, _playerTransform),
                    new BT_JumpAttack(GetComponent<IJumpAttacker>())
                }),
                new BT_Idle(GetComponent<IPlayerChaser>())
            })
        });
        return root;
    }
}
