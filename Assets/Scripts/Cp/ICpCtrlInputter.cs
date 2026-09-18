using UnityEngine;

public interface ICpCtrlInputter{
    public Vector2 Input_Look_Gamepad { get; }
    public Vector2 Input_Look_Pointer { get; }
    public Vector2 Input_Mov { get; }

    public bool TryConsume_Atk_Light();

    public bool TryConsume_Atk_Heavy();

    public bool TryConsume_Atk_Ult();

    public bool TryConsume_Dodge();

    /// <summary>
    /// Should be called when a pawn stops listening to the inputs of the controller.
    /// </summary>
    void Unpossess() {
        // Nop.
    }
}
