namespace Pilgrimage;

public sealed record QuestRequirement(QuestId QuestId) : Requirement
{
    /// <summary>
    /// Determines whether the quest requirement is met by the specified player.
    /// </summary>
    /// <param name="player">The player to evaluate the quest requirement against.</param>
    /// <returns>
    /// Returns true if the quest associated with the requirement has been completed by the player;
    /// otherwise, returns false.
    /// </returns>
    public override bool IsMetBy(Player player) =>
        player.Ledger.Find(QuestId)?.State == QuestState.Completed;
}
