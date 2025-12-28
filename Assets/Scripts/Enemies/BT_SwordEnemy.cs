using System.Collections.Generic;
using UnityEngine;

public class BT_SwordEnemy : BT_Tree
{
    // TODO: Possibly set member variables in constructor and create this in the SwordEnemy controller script.
    [SerializeField] private float _chaseMinDist;
    [SerializeField] private float _chaseMaxDist;
    [SerializeField] private Transform _playerTransform;

    protected override BT_Node SetupTree()
    {
        // TODO: If the enemy is dead, do nothing, i.e. use sequence where the IsDeadNode is first.
        BT_Node root = new BT_Selector(new List<BT_Node>
        {
            new BT_IsDead(GetComponent<IHittable>()),
            new BT_Selector(new List<BT_Node> {
                new BT_Sequence(new List<BT_Node>
                {
                    new BT_IsInRange(_chaseMinDist, _chaseMaxDist, transform, _playerTransform),
                    new BT_ChasePlayer(GetComponent<IPlayerChaser>())
                }),
                new BT_Idle(GetComponent<IPlayerChaser>())
            })
            // TODO: check if enemy is in range, if so, then do melee attack. Otherwise move onwards.
        });
        return root;
    }
}
