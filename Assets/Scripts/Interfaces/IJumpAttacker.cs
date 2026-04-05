using System;


/// <summary>
/// Character that can jump attack.
/// </summary>
[Obsolete]
public interface IJumpAttacker
{
    NodeState RequestJumpAttack();
}
