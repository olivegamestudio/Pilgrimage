namespace Pilgrimage;

public sealed record Player(Inventory Inventory, QuestLedger Ledger)
{
    internal Player Accept(QuestProgress progress) => this with { Ledger = Ledger.Accept(progress) };

    internal Player Advance(QuestId id, Func<QuestProgress, QuestProgress> transition) =>
        this with { Ledger = Ledger.Advance(id, transition) };
}
