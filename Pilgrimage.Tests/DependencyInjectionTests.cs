using Microsoft.Extensions.DependencyInjection;

namespace Pilgrimage.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddPilgrimage_RegistersSessionFactory()
    {
        ServiceCollection services = new();

        services.AddPilgrimage();

        using ServiceProvider provider = services.BuildServiceProvider();
        ISessionFactory factory = provider.GetRequiredService<ISessionFactory>();

        Assert.NotNull(factory);
    }

    [Fact]
    public void AddPilgrimage_ReturnsServiceCollectionForChaining()
    {
        ServiceCollection services = new();

        IServiceCollection returned = services.AddPilgrimage();

        Assert.Same(services, returned);
    }

    [Fact]
    public void SessionFactory_CreatesSessionForRuntimeDomainState()
    {
        ServiceCollection services = new();
        using ServiceProvider provider = services.AddPilgrimage().BuildServiceProvider();
        ISessionFactory factory = provider.GetRequiredService<ISessionFactory>();
        Quest quest = Quests.Define().Create();

        ISession session = factory.Create(Players.CreatePlayer(), new QuestSet([quest]));

        Assert.Equal(QuestStatus.Available, session.GetStatus(quest.QuestId));
    }
}
