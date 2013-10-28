using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class SlayerTitleDef
    {
        public static void Configure()
        {
            SlayerTitleCore.MaximumTitleAchieved += SlayerTitleCore_MaxSlayerTitleAchieved;
        }

        public static void InitializeConfiguration(SlayerTitleCore core)
        {
            core.Enabled = false;

            bool sheepExists = false;

            foreach (TitleDefinition def in core.TitleDefinitions)
                if (def.DefinitionName == "Sheep Slayer")
                    sheepExists = true;

            if (!sheepExists)
            {
                core.TitleDefinitions.Add(new TitleDefinition("Sheep Slayer", false,
                    new List<Type>()
                    {
                        typeof(Sheep) 
                    },
                    new List<TitleEntry>()
                    {
                        new TitleEntry("Hunter of Mutton", 50),
                        new TitleEntry("Master of the Feast", 100),
                        new TitleEntry("Mammoth of the Wool", 250)
                    }));
            }

            bool dragonExists = false;

            foreach (TitleDefinition def in core.TitleDefinitions)
                if (def.DefinitionName == "Dragon Slayer")
                    dragonExists = true;

            if (!dragonExists)
            {
                core.TitleDefinitions.Add(new TitleDefinition("Dragon Slayer", false,
                    new List<Type>()
                    {
                        typeof(Wyvern),
                        typeof(Drake),
                        typeof(Dragon),
                        typeof(WyvernRenowned),
                        typeof(WhiteWyrm),
                        typeof(ShadowWyrm),
                        typeof(AncientWyrm),
                        typeof(StygianDragon),
                        typeof(CrimsonDragon),
                        typeof(GreaterDragon),
                        typeof(SerpentineDragon),
                        typeof(SkeletalDragon)
                    },
                    new List<TitleEntry>()
                    {
                        new TitleEntry("Apprentice Dragonslayer", 50),
                        new TitleEntry("Accomplished Dragon Hunter", 250),
                        new TitleEntry("Master of the Leather Winged", 1000),
                        new TitleEntry("Terror of the Skyborn", 5000)
                    }));
            }

            core.CrossReferenceDefinitions();
        }

        public static void SlayerTitleCore_MaxSlayerTitleAchieved(Mobile from, String titleDefinitionName, String titleAwarded)
        {
            if (!from.IsPlayer())
                return;

            // Award a dragon statuette when the maximum Dragon Slayer title is achieved
            if (titleDefinitionName == "Dragon Slayer")
            {
                MonsterStatuette statue = new MonsterStatuette(MonsterStatuetteType.Dragon);

                if (!from.PlaceInBackpack(statue))
                {
                    from.BankBox.AddItem(statue);

                    from.SendMessage("A reward statue has been placed in your bankbox.");
                }
                else
                {
                    from.SendMessage("A reward statue has been placed in your backpack.");
                }
            }
        }
    }
}