using Forma;

namespace Pilgrimage;

public sealed class QuestRewardsTrait : IFormTrait
{
    public IReadOnlyCollection<Reward> Rewards { get; }

    public QuestRewardsTrait(IEnumerable<Reward> rewards)
    {
        ArgumentNullException.ThrowIfNull(rewards);
        Rewards = rewards.ToArray();
    }
}
