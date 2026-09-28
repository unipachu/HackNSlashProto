using UnityEngine;

public class AimLaser : MonoBehaviour {
    [SerializeField] float maxDistance = 100f;
    [SerializeField] LayerMask layerMask = ~0;
    public LineRenderer lineRenderer;

    void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    public void Tick() {
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * maxDistance;

        if (Physics.Raycast(start, transform.forward, out RaycastHit hit, maxDistance, layerMask)) {
            end = hit.point;
        }

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}