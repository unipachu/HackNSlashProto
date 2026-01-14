using UnityEngine;

/// <summary>
/// Debug view for ActionController
/// </summary>
public class ActionControllerInfoDebugView : MonoBehaviour
{
    public bool DebugViewEnabled = true;

    public ActionController actionController;

    void OnGUI()
    {
        if (!DebugViewEnabled)
            return;

        var action = actionController.CurrentAction;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200), GUI.skin.box);

        if (action == null)
        {
            GUILayout.Label("Action: NONE");
        }
        else
        {
            GUILayout.Label($"Action: {action.name}");
            GUILayout.Label($"Priority: {action.Priority}");
            GUILayout.Label($"Time: {actionController.NormalizedTime:F2}");
            GUILayout.Label($"HyperArmor: {actionController.HasHyperArmor()}");
            GUILayout.Label($"Invulnerable: {actionController.IsInvulnerable}");
        }

        GUILayout.EndArea();
    }
}