namespace IndustackNS.Cards;

public class ReferableCard : CardData
{
    [ExtraData("card_guid")]
    public string ReferenceId = Guid.NewGuid().ToString();
}
