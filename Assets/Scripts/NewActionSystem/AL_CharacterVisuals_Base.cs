using UnityEngine;

public class AL_CharacterVisuals_Base : AL_NewCustomAnimatorLayer
{
    public AN_CharacterVisuals_Idle Idle { get; }

    public AL_CharacterVisuals_Base(int layerIndex, Animator animator) : base(layerIndex, animator, new AN_CharacterVisuals_Idle(animator, layerIndex))
    {
        // TODO: Is this the only way to initialize the initial state separately from the initial state?
        Idle = InitialState as AN_CharacterVisuals_Idle;
    }
}
