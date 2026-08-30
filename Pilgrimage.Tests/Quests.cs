namespace Pilgrimage.Tests;

static class Quests
{
    static int _nextQuestId = 1;

    public static QuestHelper Define() => new(_nextQuestId++);

    internal class QuestHelper(int questId)
    {
        readonly List<Requirement> _requirements = [];
        readonly List<Objective> _objectives = [];
        readonly List<Reward> _rewards = [];

        QuestRepeat _repeat = QuestRepeat.Never;

        public QuestHelper WithRepeat(QuestRepeat repeat)
        {
            _repeat = repeat;
            return this;
        }

        public Quest Create()
        {
            return new(new QuestId(questId), _requirements, _objectives, _rewards, _repeat);
        }

        public QuestHelper WithObjective(Objective objective)
        {
            _objectives.Add(objective);
            return this;
        }

        public QuestHelper WithReward(Reward reward)
        {
            _rewards.Add(reward);
            return this;
        }

        public QuestHelper WithPrerequisite(Quest prerequisite)
        {
            _requirements.Add(new QuestRequirement(prerequisite.QuestId));
            return this;
        }
    }
}
