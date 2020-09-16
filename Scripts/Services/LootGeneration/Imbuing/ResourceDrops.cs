using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class IngredientDropEntry
    {
        private readonly Type m_CreatureType;
        private readonly bool m_DropMultiples;
        private readonly string m_Region;
        private readonly double m_Chance;
        private readonly Type[] m_Ingredients;

        public Type CreatureType => m_CreatureType;
        public bool DropMultiples => m_DropMultiples;
        public string Region => m_Region;
        public double Chance => m_Chance;
        public Type[] Ingredients => m_Ingredients;

        public IngredientDropEntry(Type creature, bool dropMultiples, double chance, params Type[] ingredients)
            : this(creature, dropMultiples, null, chance, ingredients)
        {
        }

        public IngredientDropEntry(Type creature, bool dropMultiples, string region, double chance, params Type[] ingredients)
        {
            m_CreatureType = creature;
            m_Ingredients = ingredients;
            m_DropMultiples = dropMultiples;
            m_Region = region;
            m_Chance = chance;
        }

        private static List<IngredientDropEntry> m_IngredientTable;
        public static List<IngredientDropEntry> IngredientTable => m_IngredientTable;

        public static void Initialize()
        {
            EventSink.CreatureDeath += OnCreatureDeath;

            m_IngredientTable = new List<IngredientDropEntry>();

            // Imbuing Gems
            m_IngredientTable.Add(new IngredientDropEntry(typeof(AncientLichRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(DevourerRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(FireElementalRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GrayGoblinMageRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GreenGoblinAlchemistRenowned), true, 0.5, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(PixieRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(RakktaviRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(SkeletalDragonRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(TikitaviRenowned), true, 0.50, ImbuingGems));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(VitaviRenowned), true, 0.50, ImbuingGems));

            //Bottle of Ichor/Spider Carapace
            m_IngredientTable.Add(new IngredientDropEntry(typeof(TrapdoorSpider), true, 0.05, typeof(SpiderCarapace)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(WolfSpider), true, 0.15, typeof(BottleIchor)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(SentinelSpider), true, 0.15, typeof(BottleIchor)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(Navrey), true, 0.50, typeof(BottleIchor), typeof(SpiderCarapace)));

            //Reflective wolf eye
            m_IngredientTable.Add(new IngredientDropEntry(typeof(ClanSSW), true, 0.20, typeof(ReflectiveWolfEye)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(LeatherWolf), true, 0.20, typeof(ReflectiveWolfEye)));

            //Faery Dust - drop from silver sapling mini champ
            m_IngredientTable.Add(new IngredientDropEntry(typeof(FairyDragon), true, "Abyss", 0.25, typeof(FaeryDust)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(Pixie), true, "Abyss", 0.25, typeof(FaeryDust)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(SAPixie), true, "Abyss", 0.25, typeof(FaeryDust)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(Wisp), true, "Abyss", 0.25, typeof(FaeryDust)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(DarkWisp), true, "Abyss", 0.25, typeof(FaeryDust)));

            m_IngredientTable.Add(new IngredientDropEntry(typeof(FairyDragon), true, "Abyss", 0.25, typeof(FeyWings)));

            //Boura Pelt
            m_IngredientTable.Add(new IngredientDropEntry(typeof(RuddyBoura), true, 0.05, typeof(BouraPelt)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(LowlandBoura), true, 0.05, typeof(BouraPelt)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(HighPlainsBoura), true, 1.00, typeof(BouraPelt)));

            //Silver snake skin
            m_IngredientTable.Add(new IngredientDropEntry(typeof(SilverSerpent), true, "TerMur", 0.10, typeof(SilverSnakeSkin)));

            //Harpsichord Roll / Not an ingredient
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), true, "TerMur", 0.01, typeof(HarpsichordRoll)));

            //Void Orb/Vial of Vitriol
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseVoidCreature), true, 0.05, typeof(VoidOrb)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(UnboundEnergyVortex), true, 0.25, typeof(VoidOrb), typeof(VialOfVitriol)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(AcidSlug), true, 0.10, typeof(VialOfVitriol)));

            //Slith Tongue
            m_IngredientTable.Add(new IngredientDropEntry(typeof(Slith), true, 0.05, typeof(SlithTongue)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(StoneSlith), true, 0.05, typeof(SlithTongue)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(ToxicSlith), true, 0.05, typeof(SlithTongue)));

            //Raptor Teeth
            m_IngredientTable.Add(new IngredientDropEntry(typeof(Raptor), true, 0.05, typeof(RaptorTeeth)));

            //Daemon Claw
            m_IngredientTable.Add(new IngredientDropEntry(typeof(FireDaemon), true, 0.60, typeof(DaemonClaw)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(FireDaemonRenowned), true, 1.00, typeof(DaemonClaw)));

            //Goblin Blood
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GreenGoblin), true, 0.10, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GreenGoblinAlchemist), true, 0.10, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GreenGoblinScout), true, 0.10, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GrayGoblin), true, 0.10, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GrayGoblinKeeper), true, 0.10, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GrayGoblinMage), true, 0.10, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(EnslavedGoblinKeeper), true, 0.25, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(EnslavedGoblinMage), true, 0.25, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(EnslavedGoblinScout), true, 0.25, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(EnslavedGrayGoblin), true, 0.25, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(EnslavedGreenGoblin), true, 0.25, typeof(GoblinBlood)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(EnslavedGreenGoblinAlchemist), true, 0.25, typeof(GoblinBlood)));

            //Lava Serpent Crust
            m_IngredientTable.Add(new IngredientDropEntry(typeof(LavaElemental), true, 0.25, typeof(LavaSerpentCrust)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(FireElementalRenowned), true, 1.00, typeof(LavaSerpentCrust)));

            //Undying Flesh
            m_IngredientTable.Add(new IngredientDropEntry(typeof(UndeadGuardian), true, 0.10, typeof(UndyingFlesh)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(Niporailem), true, 1.0, typeof(UndyingFlesh)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(ChaosVortex), true, 0.25, typeof(UndyingFlesh)));

            //Crystaline Blackrock
            m_IngredientTable.Add(new IngredientDropEntry(typeof(AgapiteElemental), true, 0.25, typeof(CrystallineBlackrock)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BronzeElemental), true, 0.25, typeof(CrystallineBlackrock)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(CopperElemental), true, 0.25, typeof(CrystallineBlackrock)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(GoldenElemental), true, 0.25, typeof(CrystallineBlackrock)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(ShadowIronElemental), true, 0.25, typeof(CrystallineBlackrock)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(ValoriteElemental), true, 0.25, typeof(CrystallineBlackrock)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(VeriteElemental), true, 0.25, typeof(CrystallineBlackrock)));

            m_IngredientTable.Add(new IngredientDropEntry(typeof(ChaosVortex), true, 0.25, typeof(ChagaMushroom)));

            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Cavern of the Discarded", 0.05, typeof(DelicateScales),
                typeof(ArcanicRuneStone), typeof(PowderedIron), typeof(EssenceBalance), typeof(CrushedGlass), typeof(CrystallineBlackrock),
                typeof(ElvenFletching), typeof(CrystalShards), typeof(Lodestone), typeof(AbyssalCloth), typeof(SeedOfRenewal)));

            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Passage of Tears", 0.05, typeof(EssenceSingularity)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Fairy Dragon Lair", 0.05, typeof(EssenceDiligence)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Abyssal Lair", 0.05, typeof(EssenceAchievement)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Crimson Veins", 0.05, typeof(EssencePrecision)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Lava Caldera", 0.05, typeof(EssencePassion)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Fire Temple Ruins", 0.05, typeof(EssenceOrder)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Enslaved Goblins", 0.05, typeof(GoblinBlood), typeof(EssenceControl)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Lands of the Lich", 0.05, typeof(EssenceDirection)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Secret Garden", 0.05, typeof(EssenceFeeling)));
            m_IngredientTable.Add(new IngredientDropEntry(typeof(BaseCreature), false, "Skeletal Dragon", 0.05, typeof(EssencePersistence)));
        }

        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature bc = e.Creature as BaseCreature;
            Container c = e.Corpse;

            if (bc != null && c != null && !c.Deleted && !bc.Controlled && !bc.Summoned)
            {
                CheckDrop(bc, c);
            }

            if (e.Killer is BaseVoidCreature)
            {
                ((BaseVoidCreature)e.Killer).Mutate(VoidEvolution.Killing);
            }
        }

        public static void CheckDrop(BaseCreature bc, Container c)
        {
            if (m_IngredientTable != null)
            {
                foreach (IngredientDropEntry entry in m_IngredientTable)
                {
                    if (entry == null)
                        continue;

                    if (entry.Region != null)
                    {
                        string reg = entry.Region;

                        if (reg == "TerMur" && c.Map != Map.TerMur)
                        {
                            continue;
                        }
                        else if (reg == "Abyss" && (c.Map != Map.TerMur || c.X < 235 || c.X > 1155 || c.Y < 40 || c.Y > 1040))
                        {
                            continue;
                        }
                        else if (reg != "TerMur" && reg != "Abyss")
                        {
                            Region r = Server.Region.Find(c.Location, c.Map);

                            if (r == null || !r.IsPartOf(entry.Region))
                                continue;
                        }
                    }

                    if (bc.GetType() != entry.CreatureType && !bc.GetType().IsSubclassOf(entry.CreatureType))
                    {
                        continue;
                    }

                    double toBeat = entry.Chance;
                    List<Item> drops = new List<Item>();

                    if (bc is BaseVoidCreature)
                    {
                        toBeat *= ((BaseVoidCreature)bc).Stage + 1;
                    }

                    if (entry.DropMultiples)
                    {
                        foreach (Type type in entry.Ingredients)
                        {
                            if (toBeat >= Utility.RandomDouble())
                            {
                                Item drop = Loot.Construct(type);

                                if (drop != null)
                                    drops.Add(drop);
                            }
                        }
                    }
                    else if (toBeat >= Utility.RandomDouble())
                    {
                        Item drop = Loot.Construct(entry.Ingredients);

                        if (drop != null)
                            drops.Add(drop);
                    }

                    foreach (Item item in drops)
                    {
                        c.DropItem(item);
                    }

                    ColUtility.Free(drops);
                }
            }
        }

        public static Type[] ImbuingGems =
        {
            typeof(FireRuby),
            typeof(WhitePearl),
            typeof(BlueDiamond),
            typeof(Turquoise)
        };
    }
}
