using UnityEngine;

/// <summary>
/// Debug view for ActionController.
/// </summary>
public class ActionControllerTimelineDebugView : MonoBehaviour
{
    public ActionController actionController;
    public bool DebugViewEnabled = true;

    void OnGUI()
    {
        if (!DebugViewEnabled)
            return;

        var action = actionController.CurrentAction;
        if (action == null)
            return;

        float t = actionController.NormalizedTime;

        Rect bar = new Rect(10, 220, 300, 20);
        GUI.Box(bar, "");

        GUI.Box(new Rect(bar.x, bar.y,
            bar.width * t, bar.height), "");

        DrawMarker(action.CanChainFrom, bar, Color.green);
        DrawMarker(action.CanCancelFrom, bar, Color.yellow);
        DrawMarker(action.IFrameFrom, bar, Color.cyan);
        DrawMarker(action.IFrameTo, bar, Color.cyan);

        foreach (var w in action._hitWindows)
        {
            DrawWindow(w.WindowStart, w.WindowEnd, bar, Color.red);
        }
    }

    void DrawMarker(float t, Rect bar, Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(
            new Rect(bar.x + bar.width * t, bar.y, 2, bar.height),
            Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void DrawWindow(float from, float to, Rect bar, Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(
            new Rect(bar.x + bar.width * from,
                     bar.y,
                     bar.width * (to - from),
                     bar.height),
            Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
