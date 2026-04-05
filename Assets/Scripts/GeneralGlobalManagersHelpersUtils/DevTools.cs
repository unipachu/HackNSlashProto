using UnityEngine;

public class DevTools : Singleton<DevTools>
{
    [SerializeField] private bool _showDebugMessages = true;
    [SerializeField] private bool _showDebugWarnings = true;
    [SerializeField] private bool _showDebugErrors = true;

    public void Log(string message)
    {
        if(_showDebugMessages)
            Debug.Log(message);
    }

    public void Log(string message, Object context)
    {
        if(_showDebugMessages)
            Debug.Log(message, context);
    }

    public void LogWarning(string message)
    {
        if (_showDebugWarnings)
            Debug.LogWarning(message);
    }

    public void LogWarning(string message, Object context)
    {
        if(_showDebugWarnings)
            Debug.LogWarning(message, context);
    }

    public void LogError(string message)
    {
        if (_showDebugErrors)
            Debug.LogError(message);
    }

    public void LogError(string message, Object context)
    {
        if(_showDebugErrors)
            Debug.LogError(message, context);
    }
}
