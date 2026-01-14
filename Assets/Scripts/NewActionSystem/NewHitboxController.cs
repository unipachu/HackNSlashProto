using UnityEngine;

public class NewHitboxController : MonoBehaviour
{
    public WeaponHitbox weaponHitbox;
    public ActionController actionController;

    int currentWindow = -1;

    void Update()
    {
        var action = actionController.CurrentAction;
        if (action == null || action._hitWindows.Length == 0)
        {
            DisableHitbox();
            return;
        }

        float t = actionController.NormalizedTime;

        for (int i = 0; i < action._hitWindows.Length; i++)
        {
            var w = action._hitWindows[i];

            if (t >= w.WindowStart && t <= w.WindowEnd)
            {
                if (currentWindow != i)
                {
                    weaponHitbox.Activate();
                    currentWindow = i;
                }
                return;
            }
        }

        DisableHitbox();
    }

    void DisableHitbox()
    {
        if (currentWindow != -1)
        {
            weaponHitbox.Deactivate();
            currentWindow = -1;
        }
    }
}
