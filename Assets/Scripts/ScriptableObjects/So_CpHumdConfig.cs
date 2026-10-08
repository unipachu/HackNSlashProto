using UnityEngine;

[CreateAssetMenu(fileName = "CpHumdConfig_", menuName = "Scriptable Object Data/CpHumdConfig")]
public class So_CpHumdConfig : ScriptableObject{
    [Header("Cooldown Durations")]
    public float cooldownDur_Dodge = 0.2f;

    [Header("Equipment Settings")]
    public HandItemT rHandItem;

    [Header("St_AtkFlying")]
    public float act_AtkFlying_TgtHorSpeed = 2;

    [Header("St_AtkJump")]
    [Tooltip("In m/s. Should be positive.")]
    public float act_AtkJump_DownSpeedAfterJumpFinished = 10;

    [Header("St_Falling")]
    [Tooltip("The distance the pawn needs to fall to enter landing animation when "
    + "hitting the ground.")]
    public float act_Falling_LandingStFallDistThreshold = 2;

    [Header("St_Dodge")]
    public float act_Dodge_YawAngSpd = 400;
    [Tooltip("Reach of the dodge move. Multiplies root motion.")]
    public float act_Dodge_HorMovSpdMult = 1.5f;
}
