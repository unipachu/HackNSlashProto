public abstract class ACS_FullBody : ACS_ActionState
{
    protected IActionCharacter Pawn { get; }

    public ACS_FullBody(IActionCharacter pawn)
    {
        Pawn = pawn;
    }
}
