using UnityEngine;

[CreateAssetMenu(fileName = "CpCommonConfig_", menuName = "Scriptable Object Data/CpCommonConfig")]
public class So_CpCommonConfig : ScriptableObject {
    [Header("Ui")]
    public string displayName = "Test Name";
    
    [Header("General Movement Settings")]
    // TODO MINOR: This should be just general falling movement values.
    public float falling_HorAcc = 10;
    [Tooltip("Gives horizontal air control.")]
    public float falling_TgtHorSpd = 1;
    [Tooltip("If pawn succeeds isGrounded check, this is the vertical velocity "
        + "used to snap slightly hovering pawn to the ground.")]
    public float groundSnapVerDownSpd = 1000;
    public float impact_YawSpd = 1200;
    // TODO MAYBE: Call this neutralMovHorAcc because some characters walk, some fly.
    public float walkHorAcc = 100;
    // TODO MAYBE: Call this neutralMovHorAcc because some characters walk, some fly.
    public float walkTgtHorSpd = 5;
    // TODO MAYBE: Call this neutralMovHorAcc because some characters walk, some fly.
    public float walkYawSpd = 1000;
    public float windup_YawSpd = 1000;

    [Header("Input Settings")]
    [Tooltip("How long should inputs stay in the buffer (in sec)?")]
    public float inputBufferDuration = 0.3f;

    [Header("Health")]
    public int hp_Max = 100;

    [Header("Hit Recieving Settings")]
    [Tooltip("Does not apply knockback this strength and below.\n" 
        + "NOTE: Cp entities have a hyperarmor bool field that overrides this when true.")]
    public KnockbackT ignoredKnockback = KnockbackT.None;
    public float knockbackStrMult = 1;

    [Header("Team Settings")]
    public Team team = Team.Team0;

    [Header("Debug")]
    public bool enableDbgMsgs = false;
}
