using System;
using UnityEngine;

/// <summary>
/// Combo-specific animation durations.<br/>
/// NOTE: <see cref="AnimEventPlr"/> uses default animation durations if these fields are set to -1.
/// </summary>
[Serializable]
public class DbRow_ComboAnimDurConfig : ISheetRowWithId {
    [SheetColumnRequired]
    [SheetColumn("Id")]
    [SerializeField] string id;
    public float laserAimNShoot_WindupDur;
    public float laserAimNShoot_AimDur;
    public float laserAimNShoot_RecoveryDur;
    public float melee3HorHit_Windup0Dur;
    public float melee3HorHit_Impact0Dur;
    public float melee3HorHit_Recovery0Dur;
    public float melee3HorHit_Impact1Dur;
    public float melee3HorHit_Recovery1Dur;
    public float melee3HorHit_Impact2Dur;
    public float melee3HorHit_Recovery2Dur;
    public float meleeSingleHorHit_Windup;
    public float meleeSingleHorHit_Impact;
    public float meleeSingleHorHit_Recovery;
    public float meleeSingleVerHit_Windup;
    public float meleeSingleVerHit_Impact;
    public float meleeSingleVerHit_Recovery;
    public float shootProj_Windup;
    public float shootProj_Recovery;

    public string Id => id;
}
