namespace IndustackNS.Cards;

public class Bullet : Resource, ICartridge
{
    public int AttackDamage;
    public int Damage => AttackDamage;

    public override void UpdateCard()
    {
        MyGameCard.SpecialValue = Damage;
        MyGameCard.SpecialIcon.sprite = SpriteManager.instance.RangedFightIcon;
        base.UpdateCard();
    }
}
