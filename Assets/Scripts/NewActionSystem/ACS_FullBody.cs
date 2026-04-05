public abstract class ACS_FullBody : ACS_ActionState
{
    protected IPawn Pawn { get; }

    public ACS_FullBody(IPawn pawn)
    {
        Pawn = pawn;
    }
}
