using Forma;

namespace Pilgrimage.Tests;

public sealed class QuestFormTests
{
    [Fact]
    public void Quest_IsAFormaFormWithQuestCompositionAndRetainedState()
    {
        Quest quest = Quests.Define().Create();

        Form form = quest;

        Assert.Equal($"pilgrimage:quest:{quest.QuestId.Value}", form.Id.Value);
        Assert.Collection(
            form.Composition.Traits,
            trait => Assert.IsType<QuestTrait>(trait),
            trait => Assert.IsType<QuestRequirementsTrait>(trait),
            trait => Assert.IsType<QuestObjectivesTrait>(trait),
            trait => Assert.IsType<QuestRewardsTrait>(trait),
            trait => Assert.IsType<QuestRepeatTrait>(trait));
        Assert.Equal(QuestStatus.Available, Assert.IsType<QuestInstanceState>(form.State).Status);
    }

    [Fact]
    public void QuestSet_RoutesCompletionOnlyToInterestedQuestForms()
    {
        Quest prerequisite = Quests.Define().Create();
        Quest interested = Quests.Define().WithPrerequisite(prerequisite).Create();
        Quest uninterested = Quests.Define().Create();
        QuestSet quests = new([prerequisite, interested, uninterested]);
        ISession session = new Session(Players.CreatePlayer(), quests);

        session.StartQuest(prerequisite.QuestId);
        session.CompleteQuest(prerequisite.QuestId);

        Quest updated = quests.Get(interested.QuestId)!;
        Assert.Contains(prerequisite.QuestId, updated.State.CompletedPrerequisites);
        Assert.Empty(updated.Interests);
        Assert.Same(uninterested, quests.Get(uninterested.QuestId));
    }

    [Fact]
    public void QuestObservation_IsIdempotent()
    {
        Quest prerequisite = Quests.Define().Create();
        Quest quest = Quests.Define().WithPrerequisite(prerequisite).Create();
        Observation<QuestCompleted> observation =
            Observation<QuestCompleted>.Create(new(prerequisite.QuestId));

        QuestReaction first = quest.Observe(observation);
        QuestReaction duplicate = first.Form.Observe(observation);

        Assert.True(first.Changed);
        Assert.False(duplicate.Changed);
        Assert.Same(first.Form, duplicate.Form);
    }
}
