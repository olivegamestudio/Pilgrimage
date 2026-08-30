namespace Pilgrimage;

internal sealed class SessionFactory : ISessionFactory
{
    public ISession Create(Player player, QuestSet quests) => new Session(player, quests);
}
