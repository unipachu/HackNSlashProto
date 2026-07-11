using UnityEngine;

// TODO: Instead of setting references in the inspector, you could make these non-Monobehavior and just create new.
public class FSM_PCStates : MonoBehaviour
{
    public FSMSt_PC_Atk_HorSlash1 atk_HorSlash1;
    public FSMSt_PC_Atk_HorSlash2 atk_HorSlash2;
    public FSMSt_PC_Atk_HorSlash3 atk_HorSlash3;
    public FSMSt_PC_Atk_Jump atk_Jump;
    public FSMSt_PC_Idle idle;
    public FSMSt_PC_Walk walk;
}
