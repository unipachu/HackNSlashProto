/// <summary>
/// A behavior tree node. Generally a node should be able to only write "input controller data"/fire input
/// events (like a real player would), and write pathfinding, and sensing data. Movement, action states and
/// such functionality should be handled by separate objects/systems.
/// </summary>
public interface IBtNode {
    /// <summary>
    /// Name used for debugging.
    /// </summary>
    string DbgName { get; }

    BtResult Eval();

    /// <summary>
    /// Used to reset the child index.
    /// </summary>
    void Reset() {
        // Nop.
    }
}
