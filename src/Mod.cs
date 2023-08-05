using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace IndustackNS
{
    public class Industack : Mod
    {
        public override void Ready()
        {
            InitializeCardColorRules(ColorManager.instance);
            AddCardsToCardBag(WorldManager.instance.GameDataLoader);
            Logger.Log("Ready!");
        }

        private void InitializeCardColorRules(ColorManager manager)
        {
            // before Resources rule works
            manager.InsertCardColorRule(0, manager.Gold, x => x.Id == "industack.electricity");
        }

        private void AddCardsToCardBag(GameDataLoader loader)
        {
            // do not use boosterpack, use this!
            List<(SetCardBagType, string, int)> cardSets =
                new()
                {
                    (SetCardBagType.AdvancedResources, "industack.copper_ore", 1),
                    (SetCardBagType.BasicResources, "industack.coal", 1),
                    (SetCardBagType.BasicIdea, "industack.blueprint.copper_wire", 1),
                    (SetCardBagType.BasicBuildingIdea, "industack.blueprint.solid_fuel_generator", 1),
                    (SetCardBagType.BasicIdea, "industack.blueprint.battery", 1)
                };
            foreach ((var set, var id, var chance) in cardSets)
            {
                loader.AddCardToSetCardBag(set, id, chance);
            }
        }
    }
}
