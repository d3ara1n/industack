using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace Industack
{
    public class IndustackMod : global::Mod
    {
        public override void Ready()
        {
            InitializeCardColorRules(ColorManager.instance);
            //AddCardsToCardBag(WorldManager.instance.GameDataLoader);
            AddBoosterpacks();
            Logger.Log("Industack mod content Ready!");
        }

        private void AddBoosterpacks()
        {
            AddBoosterpackTo(
                "main",
                new[]
                {
                    "industack.boosterpack.tiny_step",
                    "industack.boosterpack.scientific_spirit"
                }
            );
        }

        private void AddBoosterpackTo(string boardId, IEnumerable<string> boosterIds)
        {
            foreach (var id in boosterIds)
            {
                WorldManager.instance.Boards.First(x => x.Id == boardId).BoosterIds.Add(id);
            }
        }

        private void InitializeCardColorRules(ColorManager manager)
        {
            // before Resources rule works
            manager.InsertCardColorRule(0, manager.Gold, x => x.Id == "industack.electricity");
        }

        private void AddCardsToCardBag(GameDataLoader loader)
        {
            // do not use boosterpack, use this!
            // boosterpack definition is buggy
            List<(SetCardBagType, string, int)> cardSets =
                new()
                {
                    (SetCardBagType.AdvancedResources, "industack.copper_ore", 1),
                    (SetCardBagType.AdvancedResources, "industack.copper_deposit", 1),
                    (SetCardBagType.BasicResources, "industack.coal", 1),
                    (SetCardBagType.BasicIdea, "industack.blueprint.copper_wire", 1),
                    (
                        SetCardBagType.BasicBuildingIdea,
                        "industack.blueprint.solid_fuel_generator",
                        1
                    ),
                    (SetCardBagType.BasicIdea, "industack.blueprint.battery", 1)
                };
            foreach ((var set, var id, var chance) in cardSets)
            {
                loader.AddCardToSetCardBag(set, id, chance);
            }
        }
    }
}
