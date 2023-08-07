namespace IndustackNS.Cards;

public class PortalIn : Building
{
    [ExtraData("portal_reference")]
    public string? Reference;

    private GameCard? outPortal;

    protected override bool CanHaveCard(CardData otherCard) =>
        otherCard is not BaseVillager && (!otherCard.IsBuilding || otherCard is PortalOut);

    public override void UpdateCard()
    {
        var successful = false;
        if (MyGameCard.HasChild)
        {
            if (MyGameCard.Child.CardData is PortalOut referable)
            {
                outPortal = MyGameCard.Child;
                Reference = referable.ReferenceId;
            }
            else if (MyGameCard.Child.HasChild && MyGameCard.Child.CardData is not BaseVillager)
            {
                MyGameCard.StartTimer(
                    1f,
                    new TimerAction(Teleport),
                    SokLoc.Translate(
                        "industack.portal.status.teleport",
                        LocParam.Create("item", MyGameCard.Child.CardData.Name)
                    ),
                    GetActionId(nameof(Teleport))
                );
                successful = true;
            }
        }
        if (!successful)
        {
            MyGameCard.CancelTimer(GetActionId(nameof(Teleport)));
        }
        base.UpdateCard();
        if (!string.IsNullOrEmpty(Reference) && outPortal == null)
        {
            var result = WorldManager.instance.AllCards.FirstOrDefault(
                x =>
                    x.MyBoard?.IsCurrent == true
                    && x.CardData is ReferableCard reference
                    && reference.ReferenceId == Reference
            );
            if (result != null)
            {
                outPortal = result;
            }
            else
            {
                Reference = null;
            }
        }
    }

    [TimedAction("teleport_resource")]
    public void Teleport()
    {
        var child = MyGameCard.Child;
        var card = child.Child;
        child.RemoveFromStack();
        var leaf = outPortal!.GetLeafCard();
        leaf.SetChild(child);
        MyGameCard.SetChild(card);
        WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
    }
}
