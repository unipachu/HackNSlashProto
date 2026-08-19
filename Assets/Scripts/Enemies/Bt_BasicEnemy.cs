using UnityEngine;
using UnityEngine.AI;

public class Bt_BasicEnemy : MonoBehaviour, ICapsuleCharCtrlInputter {
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform tgt;

    Vector2 movInput;
    bool atkInput;

    private void Update() {
        if(Vector3.Distance(transform.position, tgt.position) > 6) {
            atkInput = false;
            movInput = Vector2.zero;
        } else if (Vector3.Distance(transform.position, tgt.position) > 3) {
            agent.SetDestination(tgt.position);
            Vector3 desiredVel = agent.desiredVelocity;
            movInput = new Vector2(desiredVel.x, desiredVel.z);
            if(movInput.sqrMagnitude > 0 )
                movInput.Normalize();
            atkInput = false;
        } else if (Vector3.Distance(transform.position, tgt.position) > 1) {
            movInput = Vector2.zero;
            atkInput = true;
        }else {
            movInput = Vector2.zero;
            atkInput = false;
        }

    }

    public Vector2 Input_Look_Gamepad => Vector2.zero;

    public Vector2 Input_Look_Pointer => Vector2.zero;

    public Vector2 Input_Mov => movInput;

    public bool TryConsume_Atk_Heavy() => false;

    public bool TryConsume_Atk_Light() => atkInput;

    public bool TryConsume_Atk_Ult() => false;

    public bool TryConsume_Dodge() => false;
}
