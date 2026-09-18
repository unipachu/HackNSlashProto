using UnityEngine;

public static class CtrlUtils {
    public static bool TryConsume(ref bool input) {
        if (!input)
            return false;
        input = false;
        return true;
    }
}
