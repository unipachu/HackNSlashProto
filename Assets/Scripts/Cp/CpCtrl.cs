using UnityEngine;


// TODO: Maybe this is not needed.
public class CpCtrl : MonoBehaviour{
    [SerializeField] PlrCtrl plrCtrl;

    ICpCtrlInputter inputter;

    private void Awake() {
        inputter = plrCtrl;
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
