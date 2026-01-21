using UnityEngine;

public class AN_CharacterVisuals : AN_Animator
{
    public ANL_CapsuleCharacter_FullBody CharacterVisualsLayer_FullBody { get; }

    public AN_CharacterVisuals(Animator animator)
    {
        CharacterVisualsLayer_FullBody = new(0, animator);
    }

    public override void AnimatorUpdate()
    {
        CharacterVisualsLayer_FullBody.UpdateLayer();
    }
}
