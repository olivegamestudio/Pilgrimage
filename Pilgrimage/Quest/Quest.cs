using Forma;

namespace Pilgrimage;

public sealed class Quest : Form
{
    readonly QuestRequirementsTrait _requirements;
    readonly QuestObjectivesTrait _objectives;
    readonly QuestRewardsTrait _rewards;
    readonly QuestRepeatTrait _repeat;

    public QuestId QuestId { get; }

    public new QuestInstanceState State => (QuestInstanceState)base.State;

    public IReadOnlyCollection<Requirement> Requirements => _requirements.Requirements;

    public IReadOnlyCollection<Objective> Objectives => _objectives.Objectives;

    public IReadOnlyCollection<Reward> Rewards => _rewards.Rewards;

    public QuestRepeat Repeat => _repeat.Repeat;

    public IReadOnlyCollection<QuestCompletedInterest> Interests =>
        _requirements.PrerequisiteIds
            .Except(State.CompletedPrerequisites)
            .Select(id => new QuestCompletedInterest(id))
            .ToArray();

    public Quest(
        QuestId id,
        IEnumerable<Requirement> requirements,
        IEnumerable<Objective> objectives,
        IEnumerable<Reward> rewards,
        QuestRepeat repeat)
        : this(
            id,
            new QuestRequirementsTrait(requirements),
            new QuestObjectivesTrait(objectives),
            new QuestRewardsTrait(rewards),
            new QuestRepeatTrait(repeat),
            new QuestInstanceState(QuestStatus.Available, []))
    {
    }

    private Quest(
        QuestId id,
        QuestRequirementsTrait requirements,
        QuestObjectivesTrait objectives,
        QuestRewardsTrait rewards,
        QuestRepeatTrait repeat,
        QuestInstanceState state)
        : base(
            new FormId($"pilgrimage:quest:{id.Value}"),
            new FormComposition([new QuestTrait(), requirements, objectives, rewards, repeat]),
            state)
    {
        QuestId = id;
        _requirements = requirements;
        _objectives = objectives;
        _rewards = rewards;
        _repeat = repeat;
    }

    internal QuestTransition<StartOutcome> Start(Player player)
    {
        if (State.Status == QuestStatus.InProgress)
        {
            return new(this, new StartOutcome.AlreadyInProgress());
        }

        if (State.Status == QuestStatus.Completed && Repeat == QuestRepeat.Never)
        {
            return new(this, new StartOutcome.NotRepeatable());
        }

        bool prerequisitesMet = _requirements.PrerequisiteIds.All(State.CompletedPrerequisites.Contains);
        bool otherRequirementsMet = Requirements
            .Where(requirement => requirement is not QuestRequirement)
            .All(requirement => requirement.IsMetBy(player));

        if (!prerequisitesMet || !otherRequirementsMet)
        {
            return new(this, new StartOutcome.RequirementsUnmet());
        }

        return new(WithState(State.WithStatus(QuestStatus.InProgress)), StartOutcome.Started);
    }

    internal QuestTransition<CompleteOutcome> Complete(Player player)
    {
        if (State.Status == QuestStatus.Available)
        {
            return new(this, CompleteOutcome.NotStarted);
        }

        if (State.Status == QuestStatus.Completed)
        {
            return new(this, CompleteOutcome.AlreadyCompleted);
        }

        if (Objectives.Any(objective => !objective.IsMetBy(player)))
        {
            return new(this, CompleteOutcome.ObjectivesUnmet);
        }

        Quest completed = WithState(State.WithStatus(QuestStatus.Completed));
        return new(
            completed,
            CompleteOutcome.Completed(Rewards),
            [Observation<QuestCompleted>.Create(new(QuestId))]);
    }

    public QuestReaction Observe(Observation<QuestCompleted> observation)
    {
        ArgumentNullException.ThrowIfNull(observation);

        QuestId completed = observation.Value.QuestId;
        if (!_requirements.PrerequisiteIds.Contains(completed) ||
            State.CompletedPrerequisites.Contains(completed))
        {
            return QuestReaction.Unchanged(this);
        }

        Quest updated = WithState(State.WithCompletedPrerequisite(completed));
        return new QuestReaction(
            updated,
            true,
            [new QuestCompletedInterest(completed)]);
    }

    private Quest WithState(QuestInstanceState state) =>
        new(QuestId, _requirements, _objectives, _rewards, _repeat, state);
}
