namespace IndustackNS.Cards;

public sealed class KineticTurret : Turret
{
    protected override bool CanHaveCard(CardData otherCard) => otherCard is ICartridge;

    public override void UpdateCard()
    {
        if (MyGameCard.HasChild && MyGameCard.Child.CardData is ICartridge)
        {
            CartridgeTypeId = MyGameCard.Child.CardData.Id;
        }
        base.UpdateCard();
    }
}
