namespace Pilgrimage;

public interface ISession
{
    QuestStatus GetStatus(QuestId questId);

    StartOutcome StartQuest(QuestId questId);

    CompleteOutcome CompleteQuest(QuestId questId);
}