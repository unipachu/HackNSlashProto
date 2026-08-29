using UnityEngine;

// TODO BEFORE BUILD: If you comment out this class/methods, you can easily find the methods in other files
// TODO BEFORE BUILD C: and comment them out as well. However you should check asserts and error
// TODO BEFORE BUILD C: logs carefully since you never want those asserts to fail in a published game!

/// <summary>
/// A bit useless class, but can be used to easily turn off debug messages, and have debug
/// messages be dependent on conditions.
/// </summary>
public class Dbg : Singleton<Dbg>{
    [SerializeField] bool showDebugMessages = true;
    [SerializeField] bool showDebugWarnings = true;
    [SerializeField] bool showDebugErrors = true;
    [SerializeField] bool useAsserts = true;

    /// <summary>
    /// Uses Debug.Assert(condition, errMsg);
    /// </summary>
    public static void Assert(bool condition, string errMsg) {
        if (inst.useAsserts)
            Debug.Assert(condition, errMsg);
    }

    /// <summary>
    /// Uses Debug.Assert(condition, errMsg, ctx);
    /// </summary>
    public static void Assert(bool condition, string errMsg, Object ctx) {
        if(inst.useAsserts)
            Debug.Assert(condition, errMsg, ctx);
    }

    /// <summary>
    /// Logs a message useing Debug.Log(msg);
    /// </summary>
    public static void Log(string msg, bool condition = true){
        if(inst.showDebugMessages && condition)
            Debug.Log(msg);
    }

    /// <summary>
    /// Logs a message useing Debug.Log(msg, ctx);
    /// </summary>
    public static void Log(string msg, Object ctx, bool condition = true){
        if(inst.showDebugMessages && condition)
            Debug.Log(msg, ctx);
    }

    /// <summary>
    /// Logs an error message useing Debug.LogError(msg);
    /// </summary>
    public static void LogErr(string msg, bool condition = true){
        if (inst.showDebugErrors && condition)
            Debug.LogError(msg);
    }

    /// <summary>
    /// Logs an error message useing Debug.LogError(msg, ctx);
    /// </summary>
    public static void LogErr(string msg, Object ctx, bool condition = true){
        if(inst.showDebugErrors && condition)
            Debug.LogError(msg, ctx);
    }

    /// <summary>
    /// Logs a warning message using Debug.LogWarning(msg);
    /// </summary>
    public static void LogWrn(string msg, bool condition = true){
        if (inst.showDebugWarnings && condition)
            Debug.LogWarning(msg);
    }

    /// <summary>
    /// Logs a warning message using Debug.LogWarning(msg, ctx);
    /// </summary>
    public static void LogWrn(string msg, Object ctx, bool condition = true){
        if(inst.showDebugWarnings && condition)
            Debug.LogWarning(msg, ctx);
    }

}
