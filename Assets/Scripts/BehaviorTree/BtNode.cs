// TODO: Rename to IBtNode
public interface BtNode {
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
