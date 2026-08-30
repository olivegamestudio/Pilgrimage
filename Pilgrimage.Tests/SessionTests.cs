namespace Pilgrimage.Tests;

public sealed class SessionTests
{
    public sealed class StartQuest
    {
        [Fact]
        public void StartQuest_WhenQuestNotInSet_ReturnsFalseAndStartsNothing()
        {
            Player player = Players.CreatePlayer();
            QuestSet quests = new(new Quest[] { });
            ISession session = new Session(player, quests);

            Quest quest = Quests.Define().Create();

            StartOutcome started = session.StartQuest(quest.QuestId);

            Assert.IsType<StartOutcome.NoSuchQuest>(started);
            Assert.Equal(QuestStatus.Unknown, session.GetStatus(quest.QuestId));
        }

        [Fact]
        public void StartQuest_WhenPrerequisiteIsCompleted_StartsQuest()
        {
            Quest firstQuest = Quests.Define().Create();
            Quest secondQuest = Quests.Define().WithPrerequisite(firstQuest).Create();

            Player player = Players.CreatePlayer();
            QuestSet quests = new([firstQuest, secondQuest]);
            ISession session = new Session(player, quests);

            // start first quest
            StartOutcome startedFirst = session.StartQuest(firstQuest.QuestId);
            Assert.IsType<StartOutcome.StartedOutcome>(startedFirst);

            // complete first quest to unlock the second quest
            CompleteOutcome completedFirst = session.CompleteQuest(firstQuest.QuestId);
            Assert.IsType<CompleteOutcome.CompletedOutcome>(completedFirst);

            // try to start second quest, which should start
            StartOutcome startedSecond = session.StartQuest(secondQuest.QuestId);
            Assert.IsType<StartOutcome.StartedOutcome>(startedSecond);
            Assert.Equal(QuestStatus.InProgress, session.GetStatus(secondQuest.QuestId));
        }

        [Fact]
        public void StartQuest_WhenPrerequisiteIsNotCompleted_DoesNotStartQuest()
        {
            Quest firstQuest = Quests.Define().Create();
            Quest secondQuest = Quests.Define().WithPrerequisite(firstQuest).Create();

            Player player = Players.CreatePlayer();
            QuestSet quests = new([firstQuest, secondQuest]);
            ISession session = new Session(player, quests);

            // try to start second quest, which should fail
            StartOutcome startedSecond = session.StartQuest(secondQuest.QuestId);
            Assert.IsType<StartOutcome.RequirementsUnmet>(startedSecond);
            Assert.Equal(QuestStatus.Available, session.GetStatus(secondQuest.QuestId));
        }

        [Fact]
        public void StartQuest_WhenQuestInSet_ReturnsTrueAndRecordsProgress()
        {
            Player player = Players.CreatePlayer();

            Quest quest = Quests.Define().Create();
            QuestSet quests = new([quest]);
            ISession session = new Session(player, quests);

            StartOutcome started = session.StartQuest(quest.QuestId);

            Assert.IsType<StartOutcome.StartedOutcome>(started);
            Assert.Equal(QuestStatus.InProgress, session.GetStatus(quest.QuestId));
        }
    }

    public sealed class CompleteQuest
    {
        [Fact]
        public void CompleteQuest_WhenQuestHasNotStarted_ReturnsNotStarted()
        {
            Quest quest = Quests.Define().Create();
            ISession session = new Session(Players.CreatePlayer(), new QuestSet([quest]));

            CompleteOutcome outcome = session.CompleteQuest(quest.QuestId);

            Assert.IsType<CompleteOutcome.NotStartedOutcome>(outcome);
            Assert.Equal(QuestStatus.Available, session.GetStatus(quest.QuestId));
        }

        [Fact]
        public void CompleteQuest_WhenQuestIsCompleted_UpdatesStatus()
        {
            Quest quest = Quests.Define().Create();
            ISession session = new Session(Players.CreatePlayer(), new QuestSet([quest]));
            session.StartQuest(quest.QuestId);

            CompleteOutcome outcome = session.CompleteQuest(quest.QuestId);

            Assert.IsType<CompleteOutcome.CompletedOutcome>(outcome);
            Assert.Equal(QuestStatus.Completed, session.GetStatus(quest.QuestId));
        }

        [Fact]
        public void CompleteQuest_WhenQuestIsAlreadyCompleted_ReturnsAlreadyCompleted()
        {
            Quest quest = Quests.Define().Create();
            ISession session = new Session(Players.CreatePlayer(), new QuestSet([quest]));
            session.StartQuest(quest.QuestId);
            session.CompleteQuest(quest.QuestId);

            CompleteOutcome outcome = session.CompleteQuest(quest.QuestId);

            Assert.IsType<CompleteOutcome.AlreadyCompletedOutcome>(outcome);
        }
    }
}
