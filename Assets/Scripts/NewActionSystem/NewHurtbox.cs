using UnityEngine;

/// <summary>
/// Logic for recieving hits.
/// </summary>
// TODO: Interface might be a better fit.
public class NewHurtbox : MonoBehaviour
{
    //public Character owner;

    public void ReceiveHit(NewHitData hit)
    {
        Debug.Log("Took hit from: " + hit.attacker
            + ", from action: " + hit.sourceAction.name);

        //if (owner.IsInvulnerable)
        //    return;
        //if (!owner.CanReceiveHit(hit))
        //    return;

        //owner.ApplyHit(hit);
    }
}