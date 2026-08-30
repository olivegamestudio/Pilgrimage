namespace Pilgrimage;

internal sealed record QuestTransition<TOutcome>(
    Quest Form,
    TOutcome Outcome,
    IReadOnlyCollection<Observation<QuestCompleted>> Emissions)
{
    public QuestTransition(Quest form, TOutcome outcome)
        : this(form, outcome, [])
    {
    }
}
