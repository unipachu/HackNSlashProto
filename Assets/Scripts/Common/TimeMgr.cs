using UnityEngine;

/// <summary>
/// NOTE: This should be the only place where time scale is set.
/// </summary>
public class TimeMgr : Singleton<TimeMgr>{
    [Tooltip("Base time scale used for all other calculations. Should be 1 unless debugging.")]
    [SerializeField] float baseTimeScl = 1;
    [Tooltip("Time scale for slow motion effects during gameplay.")]
    [SerializeField] float slowMotionTimeScl = 0.25f;

    bool paused;
    bool slowMotion;
    bool turboMode;

    public static void SetPaused(bool paused) {
        inst.paused = paused;
        UpdateTimeScl();
    }

    public static void SetSlowMotion(bool slowMotion) {
        inst.slowMotion = slowMotion;
        UpdateTimeScl();
    }

    public static void SetTurboMode(bool turboMode) {
        inst.turboMode = turboMode;
        UpdateTimeScl();
    }

    /// <summary>
    /// Updates <see cref="Time.timeScale"/> based on <see cref="TimeMgr"/> fields.
    /// </summary>
    public static void UpdateTimeScl() {
        if (inst.paused) {
            Time.timeScale = 0;
            return;
        }
        float timeScl = inst.baseTimeScl;
        if (inst.turboMode)
            timeScl *= 1.2f;   
        if (inst.slowMotion) {
            Time.timeScale = inst.slowMotionTimeScl * timeScl;
            return;
        }
        Time.timeScale = timeScl;
    }
}
