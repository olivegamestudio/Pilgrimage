namespace Pilgrimage;

public record SlottedBag(int MaxSlots, IReadOnlyCollection<BagSlot> Slots) : Bag;