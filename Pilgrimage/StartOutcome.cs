namespace Pilgrimage;

public abstract record StartOutcome
{
    private StartOutcome() { }

    public static StartOutcome Started { get; } = new StartedOutcome();

    public sealed record StartedOutcome : StartOutcome;
    public sealed record NoSuchQuest : StartOutcome;
    public sealed record AlreadyInProgress : StartOutcome;
    public sealed record RequirementsUnmet : StartOutcome;
    public sealed record NotRepeatable : StartOutcome;
}
