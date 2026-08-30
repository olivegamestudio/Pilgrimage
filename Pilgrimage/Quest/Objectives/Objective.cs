namespace Pilgrimage;

public abstract record Objective
{
    /// <summary>
    /// Determines whether the specified player meets the criteria defined by the objective.
    /// </summary>
    /// <param name="player">The player whose progress or state is evaluated against the objective.</param>
    /// <returns>True if the player meets the objective, otherwise false.</returns>
    public abstract bool IsMetBy(Player player);
}
