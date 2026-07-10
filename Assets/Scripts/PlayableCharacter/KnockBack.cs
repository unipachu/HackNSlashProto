using UnityEngine;

/// <summary>
/// Handles knockback applied to the character controller.
/// </summary>
public class KnockBack : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    private bool isInKnockBack = false;

    public bool IsInKnockBack { get => isInKnockBack;}

    private float knockBackTimer = 0;
    private Vector3 knockBackDir = Vector3.zero;
    private float knockBackDuration = 0;
    private float knockBackStrength = 0;

    // Update is called once per frame
    void Update()
    {
        if(isInKnockBack)
        {
            //Debug.Log("knockBackTimer: " + knockBackTimer + " . knockBackDuration: " + knockBackDuration);
            if(knockBackTimer > knockBackDuration)
            {
                // TODO: Both the knockback animation and the knockback movements should be finished before transitioning to walking state etc.
                isInKnockBack = false;
                return;
            }

            Vector3 knockBackMovement = knockBackDir * (knockBackDuration - knockBackTimer) * knockBackStrength;

            //Debug.Log(knockBackMovement);

            _characterController.SimpleMove(knockBackMovement);

            knockBackTimer += Time.deltaTime;
        }
    }

    // TODO: Parameter for knockback distance instead of "strength". Make sure the knockback animation and knockback locomotion match.
    public void StartKnockBack(Vector3 dir, float duration, float strength)
    {
        isInKnockBack = true;
        knockBackTimer = 0;
        knockBackDir = dir.normalized;
        knockBackDuration = duration;
        knockBackStrength = strength;
    }
}
