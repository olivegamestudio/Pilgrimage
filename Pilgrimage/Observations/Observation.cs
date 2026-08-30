namespace Pilgrimage;

public sealed record Observation<T>(Guid Id, T Value)
{
    public static Observation<T> Create(T value) => new(Guid.NewGuid(), value);
}
