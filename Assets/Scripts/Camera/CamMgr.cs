using Unity.Cinemachine;
using UnityEngine;

public class CamMgr : MonoBehaviour{
    [Header("Settings")]
    [SerializeField] float horSpd = 360;
    [SerializeField] float verSpd = 360;
    [SerializeField] bool invY = false;

    [Header("Refs")]
    [SerializeField] PlrCtrl plrCtrl;
    [SerializeField] CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] CinemachineBrain brain;

    Vector3 camFwdDir = Vector3.zero;

    public Vector3 CamFwdDir => camFwdDir;

    void Start() {
        SaveCamFwdDir();
    }

    void LateUpdate() {
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
        SaveCamFwdDir();
    }

    void SaveCamFwdDir() {
        camFwdDir = brain.transform.forward;
    }
}
