namespace Pilgrimage;

public sealed class QuestSet
{
    readonly Dictionary<QuestId, Quest> _quests;

    public IReadOnlyCollection<Quest> Quests => _quests.Values;

    public QuestSet(IEnumerable<Quest> quests)
    {
        ArgumentNullException.ThrowIfNull(quests);

        _quests = quests.ToDictionary(quest => quest.QuestId);
    }

    public Quest? Get(QuestId id) => _quests.GetValueOrDefault(id);

    internal void Replace(Quest quest) => _quests[quest.QuestId] = quest;

    internal void Route(Observation<QuestCompleted> observation)
    {
        Quest[] interested = _quests.Values
            .Where(quest => quest.Interests.Contains(new QuestCompletedInterest(observation.Value.QuestId)))
            .ToArray();

        foreach (Quest quest in interested)
        {
            QuestReaction reaction = quest.Observe(observation);
            if (reaction.Changed)
            {
                Replace(reaction.Form);
            }
        }
    }
}
