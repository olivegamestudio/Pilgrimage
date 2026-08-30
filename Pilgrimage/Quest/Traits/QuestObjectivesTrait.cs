using Forma;

namespace Pilgrimage;

public sealed class QuestObjectivesTrait : IFormTrait
{
    public IReadOnlyCollection<Objective> Objectives { get; }

    public QuestObjectivesTrait(IEnumerable<Objective> objectives)
    {
        ArgumentNullException.ThrowIfNull(objectives);
        Objectives = objectives.ToArray();
    }
}
