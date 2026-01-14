using UnityEngine;

public class NewAnimationDriver : MonoBehaviour
{
    Animator animator;

    static readonly int ActionID = Animator.StringToHash("ActionID");
    static readonly int InAction = Animator.StringToHash("InAction");
    static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // TODO: Do not use animator parameters.
    public void PlayAction(ActionDefinition action)
    {
        animator.applyRootMotion = action.UseRootMotion;
        animator.SetInteger(ActionID, action.ActionID);
        animator.SetBool(InAction, true);
    }

    public void EndAction()
    {
        animator.SetBool(InAction, false);
    }

    public float GetNormalizedTime()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
    }
}
