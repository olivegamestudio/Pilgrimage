namespace Pilgrimage;

public interface ISessionFactory
{
    ISession Create(Player player, QuestSet quests);
}
