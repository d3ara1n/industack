namespace IndustackNS.Cards;

public class SolidFuelGenerator : CardData
{
    static readonly IDictionary<string, int> vanillaOverride = new Dictionary<string, int>
    {
        { "plank", 3 },
        { "charcoal", 10 },
        { "wood", 1 },
        { "stick", 1 },
    };

    protected override bool CanHaveCard(CardData otherCard) =>
        otherCard is Fuel || vanillaOverride.ContainsKey(otherCard.Id);

    public override void UpdateCard()
    {
        var energy = MyGameCard.HasChild ? GetCardEnergy(MyGameCard.Child.CardData) : -1;
        if (energy >= 0)
            MyGameCard.StartTimer(
                5f * (energy + 1),
                new TimerAction(Burn),
                SokLoc.Translate("industack.solid_fuel_generator.status.burn"),
                GetActionId(nameof(Burn))
            );
        else
            MyGameCard.CancelTimer(GetActionId(nameof(Burn)));
        base.UpdateCard();
    }

    [TimedAction("generate_electricity")]
    public void Burn()
    {
        var child = MyGameCard.Child;
        var card = child.Child;
        var energy = GetCardEnergy(child.CardData);
        child.RemoveFromStack();
        WorldManager.instance.DestroyStack(child);
        WorldManager.instance.CreateSmoke(transform.position);
        if (energy > 0)
        {
            var stack = WorldManager.instance.CreateCardStack(
                transform.position,
                energy,
                "industack.electricity",
                false
            );
            if (stack != null)
                WorldManager.instance.StackSend(stack.GetRootCard(), MyGameCard);
        }
        if (card != null)
            MyGameCard.SetChild(card);
    }

    private int GetCardEnergy(CardData card)
    {
        if (card is Fuel fuel)
            return fuel.EnergyValue;
        if (vanillaOverride.ContainsKey(card.Id))
            return vanillaOverride[card.Id];
        return -1;
    }
}
