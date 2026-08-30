namespace Pilgrimage;

public abstract record CompleteOutcome
{
    private CompleteOutcome() { }

    public static CompleteOutcome Completed(IReadOnlyCollection<Reward> granted) =>
        new CompletedOutcome(granted);

    public static CompleteOutcome NoSuchQuest { get; } = new NoSuchQuestOutcome();
    public static CompleteOutcome NotStarted { get; } = new NotStartedOutcome();
    public static CompleteOutcome AlreadyCompleted { get; } = new AlreadyCompletedOutcome();
    public static CompleteOutcome ObjectivesUnmet { get; } = new ObjectivesUnmetOutcome();

    public sealed record CompletedOutcome(IReadOnlyCollection<Reward> Granted) : CompleteOutcome;
    public sealed record NoSuchQuestOutcome : CompleteOutcome;
    public sealed record NotStartedOutcome : CompleteOutcome;
    public sealed record AlreadyCompletedOutcome : CompleteOutcome;
    public sealed record ObjectivesUnmetOutcome : CompleteOutcome;
}
