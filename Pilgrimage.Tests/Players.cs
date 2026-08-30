namespace Pilgrimage.Tests;

static class Players
{
    public static Player CreatePlayer() => new(new Inventory([]), QuestLedger.Empty);
}
