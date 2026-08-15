using UnityEngine;

/// <summary>
/// Interpolates Transform pos/rot based on rb movement in LateUpdate.
/// </summary>
// TODO: Test this.
public class PhysInterp : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] bool interpPos = true;
    [SerializeField] bool interpRot = true;

    [Header("Refs")]
    [SerializeField] Rigidbody srcRb;
    [SerializeField] Transform tgtTrf;

    // Position
    Vector3 curPos;
    Vector3 prevPos;
    // Rotation
    Quaternion curRot;
    Quaternion prevRot;

    void Start(){
        prevPos = curPos = srcRb.position;
        prevRot = curRot = srcRb.rotation;
    }

    void LateUpdate(){
        prevPos = curPos;
        prevRot = curRot;
        curPos = srcRb.position;
        curRot = srcRb.rotation;
        // Normalized time since last physics update.
        float t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        t = Mathf.Clamp01(t);
        if (interpPos){
            tgtTrf.position = Vector3.Lerp(
                prevPos,
                curPos,
                t
            );
        }
        if (interpRot){
            tgtTrf.rotation = Quaternion.Slerp(
                prevRot,
                curRot,
                t
            );
        }
    }
}
