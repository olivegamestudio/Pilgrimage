using System.Collections.Immutable;

namespace Pilgrimage;

public sealed class QuestLedger
{
    readonly ImmutableDictionary<QuestId, QuestProgress> _entries;

    private QuestLedger(ImmutableDictionary<QuestId, QuestProgress> entries) => _entries = entries;

    public static QuestLedger Empty { get; } = new(ImmutableDictionary<QuestId, QuestProgress>.Empty);

    public QuestProgress? Find(QuestId id) =>
        _entries.TryGetValue(id, out QuestProgress? progress) ? progress : null;

    public QuestLedger Accept(QuestProgress progress) =>
        _entries.ContainsKey(progress.QuestId)
            ? throw new InvalidOperationException($"Quest {progress.QuestId} already accepted.")
            : new QuestLedger(_entries.Add(progress.QuestId, progress));

    public QuestLedger Advance(QuestId id, Func<QuestProgress, QuestProgress> transition) =>
        _entries.TryGetValue(id, out QuestProgress? current)
            ? new QuestLedger(_entries.SetItem(id, transition(current)))
            : throw new InvalidOperationException($"Quest {id} not accepted.");
}
