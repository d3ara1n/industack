namespace Industack.Cards;

public class Turret : Building
{
    [ExtraData("industack.turret.bullet_type")]
    public string? CartridgeTypeId;

    // set at card.json or derived
    public bool ConsumeCartridge = true;

    public float DamageMultiplier = 1.0f;

    public float ReloadTime = 2.0f;

    private Combatable? target;

    private ICartridge? liveCartridge;

    protected override bool CanHaveCard(CardData otherCard) =>
        otherCard is ICartridge
        && !string.IsNullOrEmpty(CartridgeTypeId)
        && otherCard.Id == CartridgeTypeId;

    public override void UpdateCard()
    {
        if (liveCartridge != null)
        {
            if (target != null)
            {
                (var cartridge, liveCartridge) = (liveCartridge, null);
                Shoot(target, cartridge);
            }
            else
            {
                target = FindTarget();
            }
        }
        else
        {
            if (
                MyGameCard.HasChild
                && MyGameCard.Child.CardData is ICartridge
                && MyGameCard.Child.CardData.Id == CartridgeTypeId
            )
            {
                MyGameCard.StartTimer(
                    ReloadTime,
                    new TimerAction(TimedReload),
                    SokLoc.Translate(
                        "industack.turret.status.reload",
                        LocParam.Create("type", CartridgeTypeId)
                    ),
                    GetActionId(nameof(TimedReload))
                );
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId(nameof(TimedReload)));
            }
        }
        base.UpdateCard();
    }

    [TimedAction("industack.turret.actions.reload")]
    public void TimedReload()
    {
        // if LiveDamage present, discard
        var child = MyGameCard.Child;
        var bullet = child.CardData as ICartridge;
        liveCartridge = bullet;
        if (ConsumeCartridge)
        {
            var card = child.Child;
            child.RemoveFromStack();
            if (card != null)
                MyGameCard.SetChild(card);
            child.DestroyCard(false, true);
        }
    }

    protected virtual Combatable? FindTarget() => WorldManager.instance.GetCard<Enemy>();

    protected virtual void Shoot(Combatable target, ICartridge bullet)
    {
        var damage = bullet.Damage * DamageMultiplier;
        // may apply status
        target.Damage((int)damage);
    }
}
