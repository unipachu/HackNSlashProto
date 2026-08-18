using UnityEngine;

/// <summary>
/// Applies spring movement to the Rigidbody or the Transform of this game object.
/// </summary>
public class SpringMov : MonoBehaviour {
    [Header("Linear Movement Settings")]
    public float linSpring = 5;
    public float maxLinSpringAcc = 99999;
    [Tooltip("Damps linear velocity based on relative linear velocity between the spring " +
        "object and the target.")]
    public float linVelMatchDamper = 5;
    public float maxLinVelMatchDamperAcc = 99999;
    [Tooltip("Damps linear velocity based on spring object linear world velocity.")]
    public float linDragDamper = 1;
    public float maxLinDragDamperAcc = 99999;
    [Tooltip("Clamps total linear acceleration caused by spring, velocity match damper, and drag damper.")]
    public float maxTotalLinAcc = 99999;

    [Header("Angular Movement Settings")]
    public float angSpring = 5;
    public float maxAngSpringAcc = 99999;
    [Tooltip("Damps anuglar velocity based on relative angular velocity between the spring " +
        "object and the target.")]
    public float angVelMatchDamper = 5;
    public float maxAngVelMatchDamperAcc = 99999;
    [Tooltip("Damps angular velocity based on spring object angular world velocity.")]
    public float angDragDamper = 1;
    public float maxAngDragDamperAcc = 99999;
    [Tooltip("Clamps total angular acceleration caused by spring, velocity match damper, and drag damper.")]
    public float maxTotalAngAcc = 99999;

    [Header("Other Settings")]
    [Tooltip("Should move the spring object to target when game starts?")]
    public bool startAtTgt = true;
    public Vector3 tgtPosWldOfs = Vector3.zero;
    public Vector3 tgtPosOfsInTgtSpc = Vector3.zero;

    [Header("Refs")]
    [Tooltip("(Recommended but optional) Rigidbody to be moved with Rigidbody.Move in FixedUpdate." +
        "Should be KINEMATIC for stable spring calculations and interpolation. " +
        "If empty, moves Transform directly in Update which can cause unstable movement " +
        "calculations if framerate is not stable!")]
    [SerializeField] Rigidbody rb;
    [Tooltip("Target for the spring.")]
    public Transform tgt;

    Vector3 linVel;
    Vector3 angVel;
    Vector3 tgtLinVel;
    Vector3 tgtAngVel;
    Pose tgtPrevPose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        tgtPrevPose = new(TgtPos(), tgt.rotation);
        if (startAtTgt) {
            transform.position = TgtPos();
            transform.rotation = tgt.rotation;
            if (rb != null) {
                rb.position = TgtPos();
                rb.rotation = tgt.rotation;
            }
        }
    }

    void Update() {
        if (rb != null)
            return;
        UpdateTgtMotSt(Time.deltaTime);
        Vector3 trfPos = transform.position;
        Quaternion trfRot = transform.rotation;
        MathUtils.UpdateSpringTrf(
            ref trfPos,
            ref trfRot,
            ref linVel,
            ref angVel,
            tgtLinVel,
            tgtAngVel,
            TgtPos(),
            tgt.rotation,
            Time.deltaTime,
            linSpring,
            maxLinSpringAcc,
            linVelMatchDamper,
            maxLinVelMatchDamperAcc,
            linDragDamper,
            maxLinDragDamperAcc,
            maxTotalLinAcc,
            angSpring,
            maxAngSpringAcc,
            angVelMatchDamper,
            maxAngVelMatchDamperAcc,
            angDragDamper,
            maxAngDragDamperAcc,
            maxTotalAngAcc
        );
        transform.SetPositionAndRotation(trfPos, trfRot);
        // Update tgt prev pose.
        tgtPrevPose = new(TgtPos(), tgt.rotation);
    }

    void FixedUpdate() {
        if (rb == null)
            return;
        UpdateTgtMotSt(Time.fixedDeltaTime);
        Vector3 rbPos = rb.position;
        Quaternion rbRot = rb.rotation;
        MathUtils.UpdateSpringTrf(
            ref rbPos,
            ref rbRot,
            ref linVel,
            ref angVel,
            tgtLinVel,
            tgtAngVel,
            TgtPos(),
            tgt.rotation,
            Time.fixedDeltaTime,
            linSpring,
            maxLinSpringAcc,
            linVelMatchDamper,
            maxLinVelMatchDamperAcc,
            linDragDamper,
            maxLinDragDamperAcc,
            maxTotalLinAcc,
            angSpring,
            maxAngSpringAcc,
            angVelMatchDamper,
            maxAngVelMatchDamperAcc,
            angDragDamper,
            maxAngDragDamperAcc,
            maxTotalAngAcc
        );
        rb.Move(rbPos, rbRot);
        // Update tgt prev pose.
        tgtPrevPose = new(TgtPos(), tgt.rotation);
    }

    Vector3 TgtPos()
        => tgt.position
        + tgtPosWldOfs
        + TrfMathUtils.TrfPtUnscaled(tgt, tgtPosOfsInTgtSpc) - tgt.position;

    /// <summary>
    /// Saves target linear and angular velocities.
    /// </summary>
    void UpdateTgtMotSt(float dt) {
        // Make sure we do not divide by a zero delta time.
        if (dt > 0) {
            // Linear velocity.
            tgtLinVel = (TgtPos() - tgtPrevPose.position) / dt;
            // Angular velocity.
            tgtAngVel = MathUtils.AngVel(tgtPrevPose.rotation, tgt.rotation, dt);
        }
        else {
            tgtLinVel = Vector3.zero;
            tgtAngVel = Vector3.zero;
        }
    }
}
