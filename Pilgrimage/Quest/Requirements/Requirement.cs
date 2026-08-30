namespace Pilgrimage;

public abstract record Requirement
{
    /// <summary>
    /// Evaluates whether the requirement is satisfied by the given player.
    /// </summary>
    /// <param name="player">The player to evaluate the requirement against.</param>
    /// <returns>
    /// Returns true if the requirement is met by the specified player;
    /// otherwise, returns false.
    /// </returns>
    public abstract bool IsMetBy(Player player);
}
