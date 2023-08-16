namespace Industack.Cards;

public class Electricity : CardData
{
    protected override bool CanHaveCard(CardData otherCard) => otherCard.MyCardType == CardType.Resources || otherCard.MyCardType == CardType.Humans;
}