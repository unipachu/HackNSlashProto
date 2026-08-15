using UnityEngine;

// TODO BEFORE BUILD: If you comment out this class, you can easily find 
public class Dbg : Singleton<Dbg>{
    [SerializeField] bool showDebugMessages = true;
    [SerializeField] bool showDebugWarnings = true;
    [SerializeField] bool showDebugErrors = true;
    [SerializeField] bool useAsserts = true;

    /// <summary>
    /// Uses Debug.Assert(condition, errMsg);
    /// </summary>
    public void Assert(bool condition, string errMsg) {
        if (useAsserts)
            Debug.Assert(condition, errMsg);
    }

    /// <summary>
    /// Uses Debug.Assert(condition, errMsg, ctx);
    /// </summary>
    public void Assert(bool condition, string errMsg, Object ctx) {
        if(useAsserts)
            Debug.Assert(condition, errMsg, ctx);
    }

    /// <summary>
    /// Logs a message useing Debug.Log(msg);
    /// </summary>
    public void Log(string msg){
        if(showDebugMessages)
            Debug.Log(msg);
    }

    /// <summary>
    /// Logs a message useing Debug.Log(msg, ctx);
    /// </summary>
    public void Log(string msg, Object ctx){
        if(showDebugMessages)
            Debug.Log(msg, ctx);
    }

    /// <summary>
    /// Logs an error message useing Debug.LogError(msg);
    /// </summary>
    public void LogErr(string msg){
        if (showDebugErrors)
            Debug.LogError(msg);
    }

    /// <summary>
    /// Logs an error message useing Debug.LogError(msg, ctx);
    /// </summary>
    public void LogErr(string msg, Object ctx){
        if(showDebugErrors)
            Debug.LogError(msg, ctx);
    }

    /// <summary>
    /// Logs a warning message using Debug.LogWarning(msg);
    /// </summary>
    public void LogWrn(string msg){
        if (showDebugWarnings)
            Debug.LogWarning(msg);
    }

    /// <summary>
    /// Logs a warning message using Debug.LogWarning(msg, ctx);
    /// </summary>
    public void LogWrn(string msg, Object ctx){
        if(showDebugWarnings)
            Debug.LogWarning(msg, ctx);
    }

}
