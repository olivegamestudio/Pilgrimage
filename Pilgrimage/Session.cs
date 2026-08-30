namespace Pilgrimage;

public class Session : ISession
{
    Player _player;
    readonly QuestSet _quests;

    public Session(Player player, QuestSet quests)
    {
        _player = player;
        _quests = quests;
    }

    public CompleteOutcome CompleteQuest(QuestId questId)
    {
        Quest? quest = _quests.Get(questId);
        if (quest is null)
        {
            return CompleteOutcome.NoSuchQuest;
        }

        QuestTransition<CompleteOutcome> transition = quest.Complete(_player);
        _quests.Replace(transition.Form);

        foreach (Observation<QuestCompleted> observation in transition.Emissions)
        {
            _quests.Route(observation);
        }

        return transition.Outcome;
    }

    public StartOutcome StartQuest(QuestId questId)
    {
        Quest? quest = _quests.Get(questId);
        if (quest is null)
        {
            return new StartOutcome.NoSuchQuest();
        }

        QuestTransition<StartOutcome> transition = quest.Start(_player);
        _quests.Replace(transition.Form);
        return transition.Outcome;
    }

    public QuestStatus GetStatus(QuestId questId)
    {
        if (_quests.Get(questId) is null)
        {
            return QuestStatus.Unknown;
        }

        return _quests.Get(questId)!.State.Status;
    }
}
