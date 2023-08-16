namespace Industack.Cards;

public class ProteinBar : Food
{
    protected override bool CanHaveCard(CardData otherCard) => otherCard is Food;

    public override bool DetermineCanHaveCardsWhenIsRoot => true;

    public override bool CanHaveCardsWhileHasStatus() => true;

    public override void UpdateCard()
    {
        MyGameCard.SpecialValue = FoodValue;
        MyGameCard.SpecialIcon.sprite = SpriteManager.instance.FoodIcon;
        if (MyGameCard.HasChild)
        {
            Merge();
        }
        base.UpdateCard();
    }

    public void Merge()
    {
        var child = MyGameCard.Child;
        var value = (child.SpecialValue.GetValueOrDefault() + 1) / 2;
        FoodValue += value;
        var card = child.Child;
        child.RemoveFromStack();
        child.DestroyCard(true);
        if (card != null)
        {
            MyGameCard.SetChild(card);
        }
    }
}
