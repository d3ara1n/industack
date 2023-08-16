using UnityEngine;

namespace Industack.Cards;

public class PortalIn : Building
{
    [ExtraData("portal_reference")]
    public string? Reference;

    private GameCard? outPortal;

    protected override bool CanHaveCard(CardData otherCard) =>
        otherCard is not BaseVillager
        && (!otherCard.IsBuilding || otherCard is PortalOut || otherCard is ResourceMagnet);

    public void Start()
    {
        if (!string.IsNullOrEmpty(Reference) && outPortal == null)
        {
            var result = WorldManager.instance.GetCardWithUniqueId(Reference);
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

    public override void UpdateCard()
    {
        if (MyGameCard.HasChild)
        {
            if (MyGameCard.Child.CardData is PortalOut referable)
            {
                outPortal = MyGameCard.Child;
                Reference = referable.UniqueId;
            }
            else
            {
                var resource = GetNextResource();
                if (resource != null)
                    Teleport(resource);
            }
        }
        base.UpdateCard();
        DrawArrow();
    }

    public void Teleport(GameCard child)
    {
        var card = child.Child;
        child.RemoveFromStack();
        outPortal!.GetLeafCard().SetChild(child);
        MyGameCard.GetLeafCard().SetChild(card);
        WorldManager.instance.CreateSmoke(MyGameCard.transform.position);
    }

    private void DrawArrow()
    {
        if (
            MyGameCard.IsHovered
            && outPortal != null
            && !CardsInStackMatchingPredicate(
                    x => x is PortalOut portal && portal.UniqueId == Reference
                )
                .Any()
        )
        {
            var arrow = new ConveyorArrow()
            {
                Start = MyGameCard.transform.position,
                End = outPortal.transform.position
            };
            DrawManager.instance.DrawShape(arrow);
        }
    }

    private GameCard? GetNextResource()
    {
        if (
            MyGameCard.HasChild
            && !MyGameCard.Child.CardData.IsBuilding
            && MyGameCard.Child.HasChild
            && !MyGameCard.Child.Child.CardData.IsBuilding
        )
        {
            return MyGameCard.Child;
        }
        else
        {
            var card = MyGameCard.Child;
            while (card != null)
            {
                if (card.CardData is ResourceMagnet)
                {
                    return card.Child;
                }
                else
                {
                    card = card.Child;
                }
            }
            return null;
        }
    }
}
