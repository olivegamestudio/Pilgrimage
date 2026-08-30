namespace Pilgrimage;

public abstract record QuestRepeat
{
    QuestRepeat() { }

    public static QuestRepeat Never { get; } = new NeverRepeat();
    public static QuestRepeat Always { get; } = new AlwaysRepeat();
    public static QuestRepeat AfterCooldown(TimeSpan duration) => new CooldownRepeat(duration);

    public sealed record NeverRepeat : QuestRepeat;

    public sealed record CooldownRepeat(TimeSpan Duration) : QuestRepeat;

    public sealed record AlwaysRepeat : QuestRepeat;
}