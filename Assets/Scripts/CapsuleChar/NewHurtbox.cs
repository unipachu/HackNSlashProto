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
        // TODO: Add more info to debug log.
        Debug.Log("Took hit from: " + hit.Attacker);

        //if (owner.IsInvulnerable)
        //    return;
        //if (!owner.CanReceiveHit(hit))
        //    return;

        //owner.ApplyHit(hit);
    }
}