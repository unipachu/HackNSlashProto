using UnityEngine;

/// <summary>
/// Moves a kinematic rb, e.g. a platforming platform.
/// </summary>
public class KinematicRbMover : MonoBehaviour{
    [Header("Ping Pong Movement")]
    [SerializeField] bool pingPong = true;
    [SerializeField] bool smoothMov = true;
    [SerializeField] Vector3 pingPongPt0 = Vector3.zero;
    [SerializeField] Vector3 pingPongPt1 = new Vector3(0, 10, 0);
    [SerializeField] float pingPongDur = 1;

    [Header("Rotational Movement")]
    [SerializeField] bool rotate = true;
    [SerializeField] Vector3 axis = Vector3.up;
    [SerializeField] float rotSpd = 1;

    [Header("Refs")]
    [SerializeField] Rigidbody rb;

    // TODO: Use this to choose which target to go towards with ping ponging
    bool movingToPt0;
    float pingPongTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Debug.Assert(rb != null && rb.isKinematic, "Rb missing or was not kinematic!", this);
    }

    private void FixedUpdate() {
        if (pingPong) {
            pingPongTimer += Time.fixedDeltaTime;
            if (pingPongDur > 0) {
                float t = Mathf.Clamp01(pingPongTimer / pingPongDur);
                if (smoothMov)
                    t = Mathf.SmoothStep(0, 1, t);
                Vector3 tgt = movingToPt0 ? pingPongPt0 : pingPongPt1;
                Vector3 start = movingToPt0 ? pingPongPt1 : pingPongPt0;
                rb.MovePosition(Vector3.Lerp(start, tgt, t));
                if(pingPongTimer >= pingPongDur) {
                    pingPongTimer -= pingPongDur;
                    movingToPt0 = !movingToPt0;
                }
            }
        }
        if (rotate) {
            rb.MoveRotation(Quaternion.AngleAxis(rotSpd * Time.fixedDeltaTime, axis) * rb.rotation);
        }
    }
}
