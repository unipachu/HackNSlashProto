public abstract class ACS_FullBody : ACS_ActionState
{
    protected NewPlayerController PC { get; }

    public ACS_FullBody(NewPlayerController playerController)
    {
        PC = playerController;
    }
}
