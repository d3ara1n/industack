namespace IndustackNS.Cards;

public class Furnace : Building
{
    public override bool DetermineCanHaveCardsWhenIsRoot => true;

    public override bool CanHaveCardsWhileHasStatus() => true;

    protected override bool CanHaveCard(CardData otherCard) =>
        otherCard is Fuel || otherCard is Ore;
}
