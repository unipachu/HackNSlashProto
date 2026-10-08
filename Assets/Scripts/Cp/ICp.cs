using UnityEngine;
using UnityEngine.AI;

public interface ICp : IFollowTgt{
    Animator Anim { get; }
    CharacterController Cc {get;}
    So_CpCommonConfig So_CpCommonConfig { get; }
    ref Cp_CommonData CommonData { get;}
    GameObject Go { get; }
    NavMeshAgent NavMeshAgent { get; }
    Transform WldHpBarPos { get; }

    bool TrySetupNEnterKnockbackSt();
    bool TryEnterDeathSt();
}
