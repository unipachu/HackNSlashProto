public abstract class ACS_FullBody : ACS_ActionState
{
    protected PC Pawn { get; }

    public ACS_FullBody(PC pawn)
    {
        Pawn = pawn;
    }
}
