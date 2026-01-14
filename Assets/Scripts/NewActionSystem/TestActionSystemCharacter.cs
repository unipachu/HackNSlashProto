using UnityEngine;

/// <summary>
/// TODO: Test script for a character using the new action system.
/// </summary>
public class TestActionSystemCharacter : MonoBehaviour
{
    int _health;
    bool _isDead = false;
    ActionController _actionController;

    public bool IsDead => _isDead;

    public bool CanReceiveHit(NewHitData hit)
    {
        if (_isDead)
            return false;

        if (_actionController.IsInvulnerable)
            return false;

        return true;
    }

    ActionDefinition ResolveReaction(NewHitData hit)
    {
        // TODO:
        //if (health <= 0)
        //    return deathAction;

        //if (hit.sourceAction.priority >= ActionPriority.Knockdown)
        //    return knockdownAction;

        //return hitReactionAction;
        return null;
    }
}
