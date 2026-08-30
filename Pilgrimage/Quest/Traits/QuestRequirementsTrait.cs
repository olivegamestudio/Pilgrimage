using Forma;

namespace Pilgrimage;

public sealed class QuestRequirementsTrait : IFormTrait
{
    public IReadOnlyCollection<Requirement> Requirements { get; }

    public IReadOnlySet<QuestId> PrerequisiteIds { get; }

    public QuestRequirementsTrait(IEnumerable<Requirement> requirements)
    {
        ArgumentNullException.ThrowIfNull(requirements);

        Requirements = requirements.ToArray();
        PrerequisiteIds = Requirements
            .OfType<QuestRequirement>()
            .Select(requirement => requirement.QuestId)
            .ToHashSet();
    }
}
