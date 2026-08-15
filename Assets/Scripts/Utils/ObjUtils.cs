using UnityEngine;

/// <summary>
/// GameObject and general Unity Object related utility and extension methods.
/// </summary>
public static class ObjUtils {
    public static void ActivateNSetPose(this GameObject go, Vector3 wldPos, Quaternion wldRot) {
        go.SetActive(true);
        go.transform.position = wldPos;
        go.transform.rotation = wldRot;
    }
}
