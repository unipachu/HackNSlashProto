using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Camera manager. Handles camera movement by player input.
/// </summary>
public class CamMgr : MonoBehaviour{
    [Header("Cam Rotation Settings")]
    [SerializeField] bool camMovByInputAllowed = true;
    [SerializeField] float horSpd = 360;
    [SerializeField] float verSpd = 360;
    [Tooltip("Smaller the number the more the camera tilts up.")]
    [SerializeField] float tiltOfs = 0.2f;
    [Tooltip("Increases the tilt range of the camera.")]
    [SerializeField] float tiltMult = 0.2f;
    [SerializeField] bool invY = false;

    [Header("Refs")]
    [SerializeField] PlrCtrl plrCtrl;
    [SerializeField] CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] CinemachinePanTilt panTilt;
    [SerializeField] CinemachineBrain brain;

    Vector3 camFwdDir = Vector3.zero;

    public Vector3 CamFwdDir => camFwdDir;

    void Start() {
        SaveCamFwdDir();
    }

    void LateUpdate() {
        if (camMovByInputAllowed) {
            MoveCamPosOnOrbitsByInput();
            RotateCam();
        }
        SaveCamFwdDir();
    }

    void MoveCamPosOnOrbitsByInput() {
        // Mouse delta rot
        Vector2 look = plrCtrl.Input_Look_Pointer;
        orbitalFollow.HorizontalAxis.Value
            += look.x * horSpd;
        orbitalFollow.VerticalAxis.Value
            += look.y * verSpd * (invY ? -1 : 1);
        // Gamepad vel based rot
        look = plrCtrl.Input_Look_Gamepad;
        orbitalFollow.HorizontalAxis.Value
            += look.x * horSpd * Time.deltaTime;
        orbitalFollow.VerticalAxis.Value
            += look.y * verSpd * Time.deltaTime * (invY ? -1 : 1);
        orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
            orbitalFollow.VerticalAxis.Value,
            orbitalFollow.VerticalAxis.Range.x,
            orbitalFollow.VerticalAxis.Range.y
        );
    }

    void SaveCamFwdDir() {
        camFwdDir = brain.transform.forward;
    }

    void RotateCam() {
        panTilt.PanAxis.Value = orbitalFollow.HorizontalAxis.Value;
        panTilt.TiltAxis.Value = orbitalFollow.VerticalAxis.Value * tiltMult + tiltOfs;
    }
}
