using UnityEngine;

// TODO: Remove this class. In Unity you can just have direct references to child sciprts (unlike in Godot).
// TODO C: Move these component references to the higher class.
public class CapsuleCharacterVisualsComponents : MonoBehaviour
{
    public Animator animator;
    public AnimRootMvmtBroadcaster rootMvmtBroadcaster;
    public CapsuleCharacterVisualsAnimEvents animEvents;
    public CapsuleCharacterAnims anims;
}
