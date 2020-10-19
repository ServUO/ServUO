using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public static class FishQuestHelper
    {
        public static Type[] Fish => m_Fish;
        private static readonly Type[] m_Fish =
        {
	        //to level 80.0 (shore fish) index to 11
	        typeof(BluegillSunfish), typeof(BrookTrout),        typeof(GreenCatfish),
            typeof(KokaneeSalmon),   typeof(PikeFish),          typeof(PumpkinSeedSunfish),
            typeof(RainbowTrout),    typeof(RedbellyBream),     typeof(SmallmouthBass),
            typeof(UncommonShiner),  typeof(Walleye),           typeof(YellowPerch),

	        //to level 106 (deepwater and crustaceans) crustaceans index to 23 fish to 41

	        typeof(CrustyLobster),   typeof(FredLobster),       typeof(HummerLobster),
            typeof(RockLobster),     typeof(ShovelNoseLobster), typeof(SpineyLobster),
            typeof(AppleCrab),       typeof(BlueCrab),          typeof(DungeonessCrab),
            typeof(KingCrab),        typeof(RockCrab),          typeof(SnowCrab),

            typeof(Amberjack),         typeof(BlackSeabass),      typeof(BlueGrouper),
            typeof(BlueFish),        typeof(Bonefish),          typeof(Bonito),
            typeof(CapeCod),         typeof(CaptainSnook),      typeof(Cobia),
            typeof(GraySnapper),     typeof(Haddock),           typeof(MahiMahi),
            typeof(RedDrum),         typeof(RedGrouper),        typeof(RedSnook),
            typeof(Shad),            typeof(Tarpon),            typeof(YellowfinTuna),
  
	        //skill elvel to 120.0 for dungeon index to 53

	        typeof(CragSnapper),     typeof(CutThroatTrout),    typeof(DarkFish),
            typeof(DemonTrout),      typeof(DrakeFish),         typeof(DungeonChub),
            typeof(GrimCisco),       typeof(InfernalTuna),      typeof(LurkerFish),
            typeof(OrcBass),         typeof(SnaggletoothBass),  typeof(TormentedPike)

        };

        public static int[] Labels => m_Labels;
        private static readonly int[] m_Labels =
        {
	        //to level 80.0 (shore fish) index to 11
	        1116417,     1116415,      1116421,
            1116423,     1116414,      1116412,
            1116416,     1116418,      1116419,
            1116420,     1116422,      1116413,

	        //to level 106 (deepwater and crustaceans) crustaceans index to 23 fish to 41

	        1116383,     1116382,      1116381,
            1116380,     1116384,      1116379,
            1116378,     1116374,      1116373,
            1116375,     1116376,      1116377,

            1116402,    1116396,      1116411,
            1116406,     1116409,      1116405,
            1116395,     1116408,      1116400,
            1116399,     1116394,      1116401,
            1116410,     1116407,      1116398,
            1116403,     1116397,      1116404,
  
	        //skill elvel to 120.0 for dungeon index to 53

	        1116432,     1116427,      1116431,
            1116425,     1116429,      1116424,
            1116428,     1116433,      1116435,
            1116430,     1116426,      1116434

        };

        private static readonly Type[][][] m_RewardTable =
        {
            new Type[][]
            {
                new Type[] { typeof(Bait) },
                new Type[] { typeof(LavaLobsterTrap) },
                new Type[] { typeof(FishingGuideBook1), typeof(FishingGuideBook2) },
                new Type[] { typeof(PowerScroll), typeof(FishingPole) },
            },

            new Type[][]
            {
                new Type[] { typeof(Bait) },
                new Type[] { typeof(LavaHook), typeof(LavaLobsterTrap), typeof(JunkProofHook) },
                new Type[] { typeof(FishingGuideBook1), typeof(FishingGuideBook2), typeof(FishingGuideBook3), typeof(FishingPole) },
                new Type[] { typeof(PowerScroll), typeof(OracleOfTheSea), typeof(DredgingHook) },
            },

            new Type[][]
            {
                new Type[] { typeof(Bait) },
                new Type[] { typeof(LavaHook), typeof(DredgingHook), typeof(JunkProofHook), typeof(FishingPole) },
                new Type[] { typeof(FishingGuideBook3), typeof(FishingGuideBook4), typeof(FishingGuideBook5), },
                new Type[] { typeof(PowerScroll), typeof(OracleOfTheSea) },
            },

            new Type[][]
            {
                new Type[] { typeof(Bait),  typeof(JunkProofHook) },
                new Type[] { typeof(OracleOfTheSea), typeof(LavaHook), typeof(FishingPole) },
                new Type[] { typeof(FishingGuideBook4), typeof(FishingGuideBook5), typeof(FishingGuideBook6) },
                new Type[] { typeof(PowerScroll), typeof(PermanentBoatPaint) },
            }
        };

        public static void GiveRewards(Mobile from, PlayerFishingEntry entry, double points)
        {
            Container pack = from.Backpack;

            if (pack == null)
                pack = from.BankBox;

            int tier = 1;
            int category = 1;

            double roll = Utility.RandomDouble();

            if (points >= 100)
                tier = 2;
            else if (points >= 150)
                tier = 3;
            else if (points >= 210)
                tier = 4;

            if (roll < .10)
                category = 4;
            else if (roll < .25)
                category = 3;
            else if (roll < .50)
                category = 2;

            Type type = m_RewardTable[tier - 1][category - 1][Utility.Random(m_RewardTable[tier - 1][category - 1].Length)];

            Item item = Loot.Construct(type);

            if (item != null)
            {
                if (item is PowerScroll)
                {
                    int value;
                    double chance = Utility.RandomDouble();

                    switch (tier)
                    {
                        default:
                        case 1:
                            if (0.01 > chance)
                                value = 120;
                            else if (0.05 > chance)
                                value = 115;
                            else if (chance > 0.25)
                                value = 110;
                            else value = 105;
                            break;
                        case 2:
                            if (0.05 > chance)
                                value = 120;
                            else if (0.25 > chance)
                                value = 115;
                            else
                                value = 110;
                            break;
                        case 3:
                            if (0.10 > chance)
                                value = 120;
                            else
                                value = 115;
                            break;
                        case 4:
                            value = 120;
                            break;
                    }

                    ((PowerScroll)item).Skill = SkillName.Fishing;
                    ((PowerScroll)item).Value = value;
                    from.SendLocalizedMessage(1149591); //
                }
                else if (item is BaseBook)
                    from.SendLocalizedMessage(1149590); //You receive a rare book.
                else if (item is Bait)
                {
                    Bait bait = (Bait)item;

                    switch (tier)
                    {
                        case 1:
                            bait.Index = Utility.Random(15);
                            if (0.001 >= Utility.RandomDouble())
                                bait.Enhanced = true;
                            break;
                        case 2:
                            bait.Index = Utility.Random(34);
                            if (0.005 >= Utility.RandomDouble())
                                bait.Enhanced = true;
                            break;
                        case 3:
                            bait.Index = Utility.Random(34);
                            if (0.01 >= Utility.RandomDouble())
                                bait.Enhanced = true;
                            break;
                        case 4:
                            if (Utility.RandomBool())
                                bait.Index = 35;
                            else
                                bait.Index = Utility.RandomMinMax(16, 34);
                            if (0.05 >= Utility.RandomDouble())
                                bait.Enhanced = true;
                            break;
                    }

                    if (FishInfo.GetTypeFromIndex(bait.Index) == typeof(Charydbis))
                    {
                        bait.UsesRemaining = 5;
                        from.SendLocalizedMessage(1150871); //You receive charybdis bait

                        if (0.08 >= Utility.RandomDouble())
                            bait.Enhanced = true;
                    }
                    else
                    {
                        object label = FishInfo.GetFishLabel(bait.Index);

                        if (label is int)
                            from.SendLocalizedMessage(1149588, string.Format("#{0}\t", (int)label)); //You receive bait to catch ~1_val~
                        else
                            from.SendLocalizedMessage(1149588, (string)label);      //You receive bait to catch ~1_val~

                        bait.UsesRemaining = (int)Math.Min(200, points / 2);
                    }
                }
                else if (item is PermanentBoatPaint)
                {
                    from.SendMessage("You recieve permanent boat paint!");
                }
                else
                {
                    if (item is FishingPole)
                        BaseRunicTool.ApplyAttributesTo((FishingPole)item, false, 0, Utility.RandomMinMax(1, tier + 1), 25, 100);

                    from.SendLocalizedMessage(1149589); //You receive some rare fishing equipment.
                }

                pack.DropItem(item);

                if (entry != null)
                    entry.OnAfterReward(points);
            }
        }

        public static List<FishMonger> Mongers => m_Mongers;
        private static readonly List<FishMonger> m_Mongers = new List<FishMonger>();

        public static void AddMonger(Mobile mob)
        {
            if (mob == null || !(mob is FishMonger) || m_Mongers.Contains((FishMonger)mob))
                return;

            m_Mongers.Add((FishMonger)mob);
        }

        public static FishMonger GetRandomMonger(PlayerMobile player, FishMonger monger)
        {
            bool NOGO = true;
            FishMonger mob = null;
            Map map = player.Map;

            List<FishMonger> mongers = new List<FishMonger>(m_Mongers);

            //First, remove quester
            if (mongers.Contains(monger))
                mongers.Remove(monger);

            //Next, remove mongers from other facets in same region as quest giver
            foreach (FishMonger m in m_Mongers)
            {
                if (m.Region != null && monger.Region != null && m.Region.Name == monger.Region.Name)
                    mongers.Remove(m);
            }

            //Now, remove mongers from other quests
            if (player.Quests != null)
            {
                for (int i = 0; i < player.Quests.Count; i++)
                {
                    if (player.Quests[i] is ProfessionalFisherQuest)
                    {
                        ProfessionalFisherQuest q = (ProfessionalFisherQuest)player.Quests[i];

                        if (mongers.Contains(q.TurnIn))
                            mongers.Remove(q.TurnIn);
                    }
                }
            }

            if (mongers.Count < 1)
                return null;

            while (NOGO)
            {
                mob = mongers[Utility.Random(mongers.Count)];

                if (mob.Region != null && mob.Region.Name != null)
                    NOGO = false;
            }

            return mob;
        }

        public static BaseBoat GetBoat(Mobile from)
        {
            List<BaseBoat> boats = new List<BaseBoat>();

            foreach (BaseBoat boat in BaseBoat.Boats)
            {
                if (boat.Owner == from && !boat.IsRowBoat)
                    boats.Add(boat);
            }

            BaseBoat closest = null;
            int range = 5000;

            foreach (BaseBoat boat in boats)
            {
                int dist = (int)from.GetDistanceToSqrt(boat.Location);
                if (closest == null || dist < range)
                {
                    closest = boat;
                    range = dist;
                }
            }

            return closest;
        }

        public static bool HasFishQuest(PlayerMobile player, MondainQuester quester, bool inRange)
        {
            //We need to bump the completed quests to the front so we can get credit before the incomplete quests.
            List<ProfessionalFisherQuest> quests = new List<ProfessionalFisherQuest>();

            if (player.Quests != null)
            {
                for (int i = 0; i < player.Quests.Count; i++)
                {
                    if (player.Quests[i] is ProfessionalFisherQuest)
                    {
                        ProfessionalFisherQuest quest = player.Quests[i] as ProfessionalFisherQuest;

                        if (quest.Completed)
                            quests.Insert(0, quest);
                        else
                            quests.Add(quest);
                    }
                }
            }

            for (int i = 0; i < quests.Count; i++)
            {
                ProfessionalFisherQuest quest = quests[i];

                if (quest.Quester is Mobile && (Mobile)quest.Quester == quester)
                {
                    player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                    quest.InProgress();
                    return true;
                }

                if (quest.TurnIn == quester || (quest.TurnIn != null && quest.TurnIn.Region != null && quester.Region != null && quest.TurnIn.Region.Name == quester.Region.Name))
                {
                    if (!inRange)
                    {
                        quester.SayTo(player, 1116519); //I can't find your ship! You need to bring it in closer.
                        return true;
                    }

                    if (quest.Completed)
                    {
                        quest.OnCompleted();
                        player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                    }
                    else
                    {
                        player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                        quest.InProgress();
                    }
                    return true;
                }
            }

            if (!inRange)
            {
                quester.SayTo(player, 1116514); //Bring yer ship around, I might have some work for ye!
                return true;
            }

            return false;
        }

        public static int GetIndexForSkillLevel(Mobile from)
        {
            if (from == null)
                return 0;

            double skill = from.Skills[SkillName.Fishing].Base;

            if (skill < 80.0)
                return 11;
            if (skill < 106.0)
                return 41;
            else
                return m_Fish.Length; //TODO CHECK
        }

        public static int GetIndexForType(Type type)
        {
            for (int i = 0; i < m_Fish.Length; i++)
            {
                if (m_Fish[i] == type)
                    return i;
            }

            return 0;
        }

        public static Type GetTypeFromIndex(int index)
        {
            if (index < 0 || index >= m_Fish.Length)
                return null;

            return m_Fish[index];
        }

        public static bool IsShallowWaterFish(Type type)
        {
            return GetIndexForType(type) <= 11;
        }

        public static bool IsCrustacean(Type type)
        {
            int index = GetIndexForType(type);
            return index > 11 && index <= 23;
        }

        public static bool IsDeepWaterFish(Type type)
        {
            int index = GetIndexForType(type);
            return index > 11 && index <= 41;
        }

        public static bool IsDungeonFish(Type type)
        {
            int index = GetIndexForType(type);
            return index > 41 && index < m_Fish.Length;
        }
    }
}

