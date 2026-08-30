namespace Pilgrimage;

public sealed record QuestReaction(
    Quest Form,
    bool Changed,
    IReadOnlyCollection<QuestCompletedInterest> RemovedInterests)
{
    internal static QuestReaction Unchanged(Quest form) => new(form, false, []);
}
