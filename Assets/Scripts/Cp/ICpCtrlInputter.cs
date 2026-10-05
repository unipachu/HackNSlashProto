using UnityEngine;

public interface ICpCtrlInputter{
    public Vector2 Input_RStick { get; }
    public Vector2 Input_PointerDelta { get; }
    public Vector2 Input_LStick { get; }

    bool TryConsume_Atk_Light();

    public bool TryConsume_Atk_Heavy();

    public bool TryConsume_Atk_Ult();

    public bool TryConsume_Dodge();
}
