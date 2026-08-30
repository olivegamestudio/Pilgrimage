using Forma;

namespace Pilgrimage;

public sealed class QuestInstanceState : FormState
{
    public QuestStatus Status { get; }

    public IReadOnlySet<QuestId> CompletedPrerequisites { get; }

    internal QuestInstanceState(QuestStatus status, IEnumerable<QuestId> completedPrerequisites)
    {
        ArgumentNullException.ThrowIfNull(completedPrerequisites);

        Status = status;
        CompletedPrerequisites = completedPrerequisites.ToHashSet();
    }

    internal QuestInstanceState WithStatus(QuestStatus status) =>
        new(status, CompletedPrerequisites);

    internal QuestInstanceState WithCompletedPrerequisite(QuestId questId) =>
        new(Status, CompletedPrerequisites.Append(questId));
}
