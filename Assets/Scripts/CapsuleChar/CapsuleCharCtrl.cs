using UnityEngine;

public class CapsuleCharCtrl : MonoBehaviour{
    [SerializeField] PlrCtrl plrCtrl;
    [SerializeField] Bt_BasicEnemy bt_BasicEnemy;

    ICapsuleCharCtrlInputter inputter;

    private void Awake() {
        if (plrCtrl != null)
            inputter = plrCtrl;
        else if (bt_BasicEnemy != null)
            inputter = bt_BasicEnemy;
        else
            Debug.LogError("No inputter set!", this);
    }

    public Vector2 Input_Look_Gamepad => inputter.Input_Look_Gamepad;
    /// <summary>
    /// Gives mouse delta.
    /// </summary>
    public Vector2 Input_Look_Pointer => inputter.Input_Look_Pointer;
    public Vector2 Input_Mov => inputter.Input_Mov;

    public bool TryConsume_Atk_Light() => inputter.TryConsume_Atk_Light();

    public bool TryConsume_Atk_Heavy() => inputter.TryConsume_Atk_Heavy();

    public bool TryConsume_Atk_Ult() => inputter.TryConsume_Atk_Ult();

    public bool TryConsume_Dodge() => inputter.TryConsume_Dodge();
}
