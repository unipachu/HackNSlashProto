using UnityEngine;

public interface ICpCtrlInputter{
    public Vector2 Input_Look_Gamepad { get; }
    public Vector2 Input_Look_Pointer { get; }
    public Vector2 Input_Mov { get; }

    public bool TryConsume_Atk_Light();

    public bool TryConsume_Atk_Heavy();

    public bool TryConsume_Atk_Ult();

    public bool TryConsume_Dodge();
}
