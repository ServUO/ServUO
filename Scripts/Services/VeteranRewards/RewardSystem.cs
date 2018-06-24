using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.VeteranRewards
{
    public interface IRewardItem
    {
        bool IsRewardItem { get; set; }
    }

    public class RewardSystem
    {
        public static bool Enabled = Config.Get("VetRewards.Enabled", true);
        public static bool SkillCapRewards = Config.Get("VetRewards.SkillCapRewards", true);
        public static int SkillCap = Config.Get("PlayerCaps.TotalSkillCap", 7000);
        public static int SkillCapBonus = Config.Get("VetRewards.SkillCapBonus", 200);
        public static int SkillCapBonusLevels = Config.Get("VetRewards.SkillCapBonusLevels", 4);
        public static float SkillCapBonusIncrement = SkillCapBonus / SkillCapBonusLevels;
        public static TimeSpan RewardInterval = Config.Get("VetRewards.RewardInterval", TimeSpan.FromDays(30.0d));
        public static int StartingLevel = Config.Get("VetRewards.StartingLevel", 0);

        private static RewardCategory[] m_Categories;
        private static RewardList[] m_Lists;
        public static RewardCategory[] Categories
        {
            get
            {
                if (m_Categories == null)
                    SetupRewardTables();

                return m_Categories;
            }
        }
        public static RewardList[] Lists
        {
            get
            {
                if (m_Lists == null)
                    SetupRewardTables();

                return m_Lists;
            }
        }
        public static bool HasAccess(Mobile mob, RewardCategory category)
        {
            List<RewardEntry> entries = category.Entries;

            for (int j = 0; j < entries.Count; ++j)
            {
                //RewardEntry entry = entries[j];
                if (RewardSystem.HasAccess(mob, entries[j]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasAccess(Mobile mob, RewardEntry entry)
        {
            if (Core.Expansion < entry.RequiredExpansion)
            {
                return false;
            }

            TimeSpan ts;
            return HasAccess(mob, entry.List, out ts);
        }

        public static bool HasAccess(Mobile mob, RewardList list, out TimeSpan ts)
        {
            if (list == null)
            {
                ts = TimeSpan.Zero;
                return false;
            }

            Account acct = mob.Account as Account;

            if (acct == null)
            {
                ts = TimeSpan.Zero;
                return false;
            }

            TimeSpan totalTime = (DateTime.UtcNow - acct.Created) + TimeSpan.FromDays(RewardInterval.TotalDays * StartingLevel);

            ts = (list.Age - totalTime);


            if (ts <= TimeSpan.Zero)
                return true;

            return false;
        }

        public static int GetRewardLevel(Mobile mob)
        {
            Account acct = mob.Account as Account;

            if (acct == null)
                return 0;

            return GetRewardLevel(acct);
        }

        public static int GetRewardLevel(Account acct)
        {
            TimeSpan totalTime = (DateTime.UtcNow - acct.Created);
            TimeSpan ositotalTime = (DateTime.UtcNow - new DateTime(1997, 9, 24));

            int level = (int)(totalTime.TotalDays / RewardInterval.TotalDays);
            int levelosi = (int)(ositotalTime.TotalDays / 365);

            if (level < 0)
                level = 0;

            level += StartingLevel;

            return Math.Min(level, levelosi);
        }

        public static bool HasHalfLevel(Mobile mob)
        {
            Account acct = mob.Account as Account;

            if (acct == null)
                return false;

            return HasHalfLevel(acct);
        }

        public static bool HasHalfLevel(Account acct)
        {
            TimeSpan totalTime = (DateTime.UtcNow - acct.Created);

            Double level = (totalTime.TotalDays / RewardInterval.TotalDays);

            return level >= 0.5;
        }

        public static bool ConsumeRewardPoint(Mobile mob)
        {
            int cur, max;

            ComputeRewardInfo(mob, out cur, out max);

            if (cur >= max)
                return false;

            Account acct = mob.Account as Account;

            if (acct == null)
                return false;

            //if ( mob.AccessLevel < AccessLevel.GameMaster )
            acct.SetTag("numRewardsChosen", (cur + 1).ToString());

            return true;
        }

        public static void ComputeRewardInfo(Mobile mob, out int cur, out int max)
        {
            int level;

            ComputeRewardInfo(mob, out cur, out max, out level);
        }

        public static void ComputeRewardInfo(Mobile mob, out int cur, out int max, out int level)
        {
            Account acct = mob.Account as Account;

            if (acct == null)
            {
                cur = max = level = 0;
                return;
            }

            level = GetRewardLevel(acct);

            if (level == 0)
            {
                cur = max = 0;
                return;
            }

            string tag = acct.GetTag("numRewardsChosen");

            if (String.IsNullOrEmpty(tag))
                cur = 0;
            else
                cur = Utility.ToInt32(tag);

            if (level >= 6)
                max = 9 + ((level - 6) * 2);
            else
                max = 2 + level;
        }

        public static bool CheckIsUsableBy(Mobile from, Item item, object[] args)
        {
            if (from.AccessLevel > AccessLevel.GameMaster || UseableByAnyone(item.GetType()))
                return true;

            if (m_Lists == null)
                SetupRewardTables();

            Type type = item.GetType();

            for (int i = 0; i < m_Lists.Length; ++i)
            {
                RewardList list = m_Lists[i];
                RewardEntry[] entries = list.Entries;
                TimeSpan ts;

                for (int j = 0; j < entries.Length; ++j)
                {
                    if (entries[j].ItemType == type)
                    {
                        if (args == null && entries[j].Args.Length == 0)
                        {
                            if (i > 0 && !HasAccess(from, list, out ts))
                            {
                                from.SendLocalizedMessage(1008126, true, Math.Ceiling(ts.TotalDays / 30.0).ToString()); // Your account is not old enough to use this item. Months until you can use this item : 
                                return false;
                            }

                            return true;
                        }

                        if (args.Length == entries[j].Args.Length)
                        {
                            bool match = true;

                            for (int k = 0; match && k < args.Length; ++k)
                                match = (args[k].Equals(entries[j].Args[k]));

                            if (match)
                            {
                                if (i > 0 && !HasAccess(from, list, out ts))
                                {
                                    from.SendLocalizedMessage(1008126, true, Math.Ceiling(ts.TotalDays / 30.0).ToString()); // Your account is not old enough to use this item. Months until you can use this item : 
                                    return false;
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            // no entry?
            return true;
        }

        private static bool UseableByAnyone(Type type)
        {
            foreach (Type t in _AnyoneTypes)
            {
                if (t == type || type.IsSubclassOf(t))
                    return true;
            }

            return false;
        }

        private static Type[] _AnyoneTypes =
        {
            typeof(DyeTub), typeof(MonsterStatuette)
        };

        public static int GetRewardYearLabel(Item item, object[] args)
        {
            int level = GetRewardYear(item, args);

            return 1076216 + ((level < 10) ? level : (level < 12) ? ((level - 9) + 4240) : ((level - 11) + 37585));
        }

        public static int GetRewardYear(Item item, object[] args)
        {
            if (m_Lists == null)
                SetupRewardTables();

            Type type = item.GetType();

            for (int i = 0; i < m_Lists.Length; ++i)
            {
                RewardList list = m_Lists[i];
                RewardEntry[] entries = list.Entries;

                for (int j = 0; j < entries.Length; ++j)
                {
                    if (entries[j].ItemType == type)
                    {
                        if (args == null && entries[j].Args.Length == 0)
                            return i + 1;

                        if (args.Length == entries[j].Args.Length)
                        {
                            bool match = true;

                            for (int k = 0; match && k < args.Length; ++k)
                                match = (args[k].Equals(entries[j].Args[k]));

                            if (match)
                                return i + 1;
                        }
                    }
                }
            }

            // no entry?
            return 0;
        }

        public static void SetupRewardTables()
        {
            RewardCategory monsterStatues = new RewardCategory(1049750);
            RewardCategory cloaksAndRobes = new RewardCategory(1049752);
            RewardCategory etherealSteeds = new RewardCategory(1049751);
            RewardCategory specialDyeTubs = new RewardCategory(1049753);
            RewardCategory houseAddOns = new RewardCategory(1049754);
            RewardCategory miscellaneous = new RewardCategory(1078596);

            m_Categories = new RewardCategory[]
            {
                monsterStatues,
                cloaksAndRobes,
                etherealSteeds,
                specialDyeTubs,
                houseAddOns,
                miscellaneous
            };

            const int Bronze = 0x972;
            const int Copper = 0x96D;
            const int Golden = 0x8A5;
            const int Agapite = 0x979;
            const int Verite = 0x89F;
            const int Valorite = 0x8AB;
            const int IceGreen = 0x47F;
            const int IceBlue = 0x482;
            const int DarkGray = 0x497;
            const int Fire = 0x489;
            const int IceWhite = 0x47E;
            const int JetBlack = 0x001;
            const int Pink = 0x490;
            const int Crimson = 0x485;
            const int GreenForest = 0x4A9;
            const int RoyalBlue = 0x538;

            m_Lists = new RewardList[]
            {
                new RewardList(RewardInterval, 1, new RewardEntry[]
                {
                    new RewardEntry(specialDyeTubs, 1006008, typeof(RewardBlackDyeTub)),
                    new RewardEntry(specialDyeTubs, 1006013, typeof(FurnitureDyeTub)),
                    new RewardEntry(specialDyeTubs, 1006047, typeof(SpecialDyeTub)),

                    new RewardEntry(cloaksAndRobes, 1006009, typeof(RewardCloak), Bronze, 1041286),
                    new RewardEntry(cloaksAndRobes, 1006010, typeof(RewardRobe), Bronze, 1041287),
                    new RewardEntry(cloaksAndRobes, 1113874, typeof(RewardGargishFancyRobe), Expansion.SA, Bronze, 1113874),
                    new RewardEntry(cloaksAndRobes, 1113875, typeof(RewardGargishRobe), Expansion.SA, Bronze, 1113875),
                    new RewardEntry(cloaksAndRobes, 1080366, typeof(RewardDress), Expansion.ML, Bronze, 1080366),
                    new RewardEntry(cloaksAndRobes, 1006011, typeof(RewardCloak), Copper, 1041288),
                    new RewardEntry(cloaksAndRobes, 1006012, typeof(RewardRobe), Copper, 1041289),
                    new RewardEntry(cloaksAndRobes, 1113876, typeof(RewardGargishFancyRobe), Expansion.SA, Copper, 1113876),
                    new RewardEntry(cloaksAndRobes, 1113877, typeof(RewardGargishRobe), Expansion.SA, Copper, 1113877),
                    new RewardEntry(cloaksAndRobes, 1080367, typeof(RewardDress), Expansion.ML, Copper, 1080367),

                    new RewardEntry(monsterStatues, 1006024, typeof(MonsterStatuette), MonsterStatuetteType.Crocodile),
                    new RewardEntry(monsterStatues, 1006025, typeof(MonsterStatuette), MonsterStatuetteType.Daemon),
                    new RewardEntry(monsterStatues, 1006026, typeof(MonsterStatuette), MonsterStatuetteType.Dragon),
                    new RewardEntry(monsterStatues, 1006027, typeof(MonsterStatuette), MonsterStatuetteType.EarthElemental),
                    new RewardEntry(monsterStatues, 1006028, typeof(MonsterStatuette), MonsterStatuetteType.Ettin),
                    new RewardEntry(monsterStatues, 1006029, typeof(MonsterStatuette), MonsterStatuetteType.Gargoyle),
                    new RewardEntry(monsterStatues, 1006030, typeof(MonsterStatuette), MonsterStatuetteType.Gorilla),
                    new RewardEntry(monsterStatues, 1006031, typeof(MonsterStatuette), MonsterStatuetteType.Lich),
                    new RewardEntry(monsterStatues, 1006032, typeof(MonsterStatuette), MonsterStatuetteType.Lizardman),
                    new RewardEntry(monsterStatues, 1006033, typeof(MonsterStatuette), MonsterStatuetteType.Ogre),
                    new RewardEntry(monsterStatues, 1006034, typeof(MonsterStatuette), MonsterStatuetteType.Orc),
                    new RewardEntry(monsterStatues, 1006035, typeof(MonsterStatuette), MonsterStatuetteType.Ratman),
                    new RewardEntry(monsterStatues, 1006036, typeof(MonsterStatuette), MonsterStatuetteType.Skeleton),
                    new RewardEntry(monsterStatues, 1006037, typeof(MonsterStatuette), MonsterStatuetteType.Troll),
                    new RewardEntry(monsterStatues, 1155746, typeof(MonsterStatuette), MonsterStatuetteType.FleshRenderer),

                    new RewardEntry(etherealSteeds, 1006019, typeof(EtherealHorse)),

                    new RewardEntry(houseAddOns,    1062692, typeof(ContestMiniHouseDeed), Expansion.AOS, MiniHouseType.MalasMountainPass),
                    new RewardEntry(houseAddOns,    1072216, typeof(ContestMiniHouseDeed), Expansion.SE, MiniHouseType.ChurchAtNight),
                    
                    new RewardEntry(houseAddOns,    1156371,  typeof(Auction.AuctionSafeDeed), Expansion.TOL),

                    new RewardEntry(miscellaneous,  1076155, typeof(RedSoulstone),         Expansion.ML),
                    new RewardEntry(miscellaneous,  1080523, typeof(CommodityDeedBox),     Expansion.ML),
                    new RewardEntry(miscellaneous,  1113945,  typeof(CrystalPortal),        Expansion.SA),
                    new RewardEntry(miscellaneous,  1150074,  typeof(CorruptedCrystalPortal), Expansion.SA),
               
                    new RewardEntry(miscellaneous,    1123603,  typeof(CoralTheOwl), Expansion.ML),
                    new RewardEntry(miscellaneous,    1151769,  typeof(GreaterBraceletOfBinding), Expansion.ML),
                }),
                new RewardList(RewardInterval, 2, new RewardEntry[]
                {
                    new RewardEntry(specialDyeTubs, 1006052, typeof(LeatherDyeTub)),

                    new RewardEntry(cloaksAndRobes, 1006014, typeof(RewardCloak), Agapite, 1041290),
                    new RewardEntry(cloaksAndRobes, 1006015, typeof(RewardRobe), Agapite, 1041291),
                    new RewardEntry(cloaksAndRobes, 1113878, typeof(RewardGargishFancyRobe), Expansion.SA, Agapite, 1113878),
                    new RewardEntry(cloaksAndRobes, 1113879, typeof(RewardGargishRobe), Expansion.SA, Agapite, 1113879),
                    new RewardEntry(cloaksAndRobes, 1080369, typeof(RewardDress), Expansion.ML, Agapite, 1080369),
                    new RewardEntry(cloaksAndRobes, 1006016, typeof(RewardCloak), Golden, 1041292),
                    new RewardEntry(cloaksAndRobes, 1006017, typeof(RewardRobe), Golden, 1041293),
                    new RewardEntry(cloaksAndRobes, 1113880, typeof(RewardGargishFancyRobe), Expansion.SA, Golden, 1113880),
                    new RewardEntry(cloaksAndRobes, 1113881, typeof(RewardGargishRobe), Expansion.SA, Golden, 1113881),
                    new RewardEntry(cloaksAndRobes, 1080368, typeof(RewardDress), Expansion.ML, Golden, 1080368),

                    new RewardEntry(monsterStatues, 1155747, typeof(MonsterStatuette), MonsterStatuetteType.CrystalElemental),
                    new RewardEntry(monsterStatues, 1157078, typeof(MonsterStatuette), Expansion.TOL, MonsterStatuetteType.TRex),

                    new RewardEntry(houseAddOns,    1006048, typeof(BannerDeed)),
                    new RewardEntry(houseAddOns,    1006049, typeof(FlamingHeadDeed)),
                    new RewardEntry(houseAddOns,    1080409, typeof(MinotaurStatueDeed), Expansion.ML)
                }),
                new RewardList(RewardInterval, 3, new RewardEntry[]
                {
                    new RewardEntry(cloaksAndRobes, 1006020, typeof(RewardCloak), Verite, 1041294),
                    new RewardEntry(cloaksAndRobes, 1006021, typeof(RewardRobe), Verite, 1041295),
                    new RewardEntry(cloaksAndRobes, 1113882, typeof(RewardGargishFancyRobe), Expansion.SA, Verite, 1113882),
                    new RewardEntry(cloaksAndRobes, 1113883, typeof(RewardGargishRobe), Expansion.SA, Verite, 1113883),
                    new RewardEntry(cloaksAndRobes, 1080370, typeof(RewardDress), Expansion.ML, Verite, 1080370),
                    new RewardEntry(cloaksAndRobes, 1006022, typeof(RewardCloak), Valorite, 1041296),
                    new RewardEntry(cloaksAndRobes, 1113884, typeof(RewardGargishFancyRobe), Expansion.SA, Valorite, 1113884),
                    new RewardEntry(cloaksAndRobes, 1113885, typeof(RewardGargishRobe), Expansion.SA, Valorite, 1113885),
                    new RewardEntry(cloaksAndRobes, 1006023, typeof(RewardRobe), Valorite, 1041297),
                    new RewardEntry(cloaksAndRobes, 1080371, typeof(RewardDress), Expansion.ML, Valorite, 1080371),

                    new RewardEntry(monsterStatues, 1006038, typeof(MonsterStatuette), MonsterStatuetteType.Cow),
                    new RewardEntry(monsterStatues, 1006039, typeof(MonsterStatuette), MonsterStatuetteType.Zombie),
                    new RewardEntry(monsterStatues, 1006040, typeof(MonsterStatuette), MonsterStatuetteType.Llama),
                    new RewardEntry(monsterStatues, 1155748, typeof(MonsterStatuette), MonsterStatuetteType.DarkFather),
                    new RewardEntry(monsterStatues, 1157079, typeof(MonsterStatuette), Expansion.TOL, MonsterStatuetteType.Zipactriotal),

                    new RewardEntry(etherealSteeds, 1006051, typeof(EtherealLlama)),
                    new RewardEntry(etherealSteeds, 1006050, typeof(EtherealOstard)),

                    new RewardEntry(houseAddOns,    1080407, typeof(PottedCactusDeed), Expansion.ML)
                }),
                new RewardList(RewardInterval, 4, new RewardEntry[]
                {
                    new RewardEntry(specialDyeTubs, 1049740, typeof(RunebookDyeTub)),

                    new RewardEntry(cloaksAndRobes, 1049725, typeof(RewardCloak), DarkGray, 1049757),
                    new RewardEntry(cloaksAndRobes, 1049726, typeof(RewardRobe), DarkGray, 1049756),
                    new RewardEntry(cloaksAndRobes, 1113886, typeof(RewardGargishFancyRobe), Expansion.SA, DarkGray, 1113886),
                    new RewardEntry(cloaksAndRobes, 1113887, typeof(RewardGargishRobe), Expansion.SA, DarkGray, 1113887),
                    new RewardEntry(cloaksAndRobes, 1080374, typeof(RewardDress), Expansion.ML, DarkGray, 1080374),
                    new RewardEntry(cloaksAndRobes, 1049727, typeof(RewardCloak), IceGreen, 1049759),
                    new RewardEntry(cloaksAndRobes, 1049728, typeof(RewardRobe), IceGreen, 1049758),
                    new RewardEntry(cloaksAndRobes, 1113888, typeof(RewardGargishFancyRobe), Expansion.SA, IceGreen, 1113888),
                    new RewardEntry(cloaksAndRobes, 1113889, typeof(RewardGargishRobe), Expansion.SA, IceGreen, 1113889),
                    new RewardEntry(cloaksAndRobes, 1080372, typeof(RewardDress), Expansion.ML, IceGreen, 1080372),
                    new RewardEntry(cloaksAndRobes, 1049729, typeof(RewardCloak), IceBlue, 1049761),
                    new RewardEntry(cloaksAndRobes, 1049730, typeof(RewardRobe), IceBlue, 1049760),
                    new RewardEntry(cloaksAndRobes, 1113890, typeof(RewardGargishFancyRobe), Expansion.SA, IceBlue, 1113890),
                    new RewardEntry(cloaksAndRobes, 1113891, typeof(RewardGargishRobe), Expansion.SA, IceBlue, 1113891),
                    new RewardEntry(cloaksAndRobes, 1080373, typeof(RewardDress), Expansion.ML, IceBlue, 1080373),

                    new RewardEntry(monsterStatues, 1049742, typeof(MonsterStatuette), MonsterStatuetteType.Ophidian),
                    new RewardEntry(monsterStatues, 1049743, typeof(MonsterStatuette), MonsterStatuetteType.Reaper),
                    new RewardEntry(monsterStatues, 1049744, typeof(MonsterStatuette), MonsterStatuetteType.Mongbat),
                    new RewardEntry(monsterStatues, 1155745, typeof(MonsterStatuette), MonsterStatuetteType.PlatinumDragon),

                    new RewardEntry(etherealSteeds, 1049746, typeof(EtherealKirin)),
                    new RewardEntry(etherealSteeds, 1049745, typeof(EtherealUnicorn)),
                    new RewardEntry(etherealSteeds, 1049747, typeof(EtherealRidgeback)),

                    new RewardEntry(houseAddOns,    1049737, typeof(DecorativeShieldDeed)),
                    new RewardEntry(houseAddOns,    1049738, typeof(HangingSkeletonDeed)),

                    new RewardEntry(miscellaneous,  1098160, typeof(Plants.SeedBox))
                }),
                new RewardList(RewardInterval, 5, new RewardEntry[]
                {
                    new RewardEntry(specialDyeTubs, 1049741, typeof(StatuetteDyeTub)),
                    new RewardEntry(specialDyeTubs, 1153495, typeof(MetallicLeatherDyeTub)),
                    new RewardEntry(specialDyeTubs, 1150067, typeof(MetallicDyeTub)),
                    new RewardEntry(cloaksAndRobes, 1049731, typeof(RewardCloak), JetBlack, 1049763),
                    new RewardEntry(cloaksAndRobes, 1049732, typeof(RewardRobe), JetBlack, 1049762),
                    new RewardEntry(cloaksAndRobes, 1113892, typeof(RewardGargishFancyRobe), Expansion.SA, JetBlack, 1113892),
                    new RewardEntry(cloaksAndRobes, 1113893, typeof(RewardGargishRobe), Expansion.SA, JetBlack, 1113893),
                    new RewardEntry(cloaksAndRobes, 1080377, typeof(RewardDress), Expansion.ML, JetBlack, 1080377),
                    new RewardEntry(cloaksAndRobes, 1049733, typeof(RewardCloak), IceWhite, 1049765),
                    new RewardEntry(cloaksAndRobes, 1049734, typeof(RewardRobe), IceWhite, 1049764),
                    new RewardEntry(cloaksAndRobes, 1113894, typeof(RewardGargishFancyRobe), Expansion.SA, IceWhite, 1113894),
                    new RewardEntry(cloaksAndRobes, 1113895, typeof(RewardGargishRobe), Expansion.SA, IceWhite, 1113895),
                    new RewardEntry(cloaksAndRobes, 1080376, typeof(RewardDress), Expansion.ML, IceWhite, 1080376),
                    new RewardEntry(cloaksAndRobes, 1049735, typeof(RewardCloak), Fire, 1049767),
                    new RewardEntry(cloaksAndRobes, 1049736, typeof(RewardRobe), Fire, 1049766),
                    new RewardEntry(cloaksAndRobes, 1113896, typeof(RewardGargishFancyRobe), Expansion.SA, Fire, 1113896),
                    new RewardEntry(cloaksAndRobes, 1113897, typeof(RewardGargishRobe), Expansion.SA, Fire, 1113897),
                    new RewardEntry(cloaksAndRobes, 1080375, typeof(RewardDress), Expansion.ML, Fire, 1080375),

                    new RewardEntry(monsterStatues, 1049768, typeof(MonsterStatuette), MonsterStatuetteType.Gazer),
                    new RewardEntry(monsterStatues, 1049769, typeof(MonsterStatuette), MonsterStatuetteType.FireElemental),
                    new RewardEntry(monsterStatues, 1049770, typeof(MonsterStatuette), MonsterStatuetteType.Wolf),
                    new RewardEntry(monsterStatues, 1157080, typeof(MonsterStatuette), Expansion.TOL, MonsterStatuetteType.MyrmidexQueen),

                    new RewardEntry(etherealSteeds, 1049749, typeof(EtherealSwampDragon)),
                    new RewardEntry(etherealSteeds, 1049748, typeof(EtherealBeetle)),
                    new RewardEntry(houseAddOns,    1049739, typeof(StoneAnkhDeed)),
                    new RewardEntry(houseAddOns,    1080384, typeof(BloodyPentagramDeed), Expansion.ML),
                    new RewardEntry(houseAddOns,    1154582, typeof(LighthouseAddonDeed), Expansion.HS),
                }),
                new RewardList(RewardInterval, 6, new RewardEntry[]
                {
                    new RewardEntry(houseAddOns,    1076188, typeof(CharacterStatueMaker), Expansion.ML, StatueType.Jade),
                    new RewardEntry(houseAddOns,    1076189, typeof(CharacterStatueMaker), Expansion.ML, StatueType.Marble),
                    new RewardEntry(houseAddOns,    1076190, typeof(CharacterStatueMaker), Expansion.ML, StatueType.Bronze),
                    new RewardEntry(houseAddOns,    1080527, typeof(RewardBrazierDeed), Expansion.ML)
                }),
                new RewardList(RewardInterval, 7, new RewardEntry[]
                {
                    new RewardEntry(houseAddOns,    1076157, typeof(CannonDeed),           Expansion.ML),
                    new RewardEntry(houseAddOns,    1080550, typeof(TreeStumpDeed),        Expansion.ML),
                    new RewardEntry(houseAddOns,    1151835, typeof(SheepStatueDeed),      Expansion.ML),
                    new RewardEntry(houseAddOns,    1123504, typeof(SewingMachineDeed),    Expansion.TOL),
                    new RewardEntry(houseAddOns,    1123577, typeof(SmithingPressDeed),    Expansion.TOL),
                    new RewardEntry(houseAddOns,    1156369, typeof(SpinningLatheDeed),    Expansion.TOL),
                    new RewardEntry(houseAddOns,    1156370, typeof(FletchingStationDeed), Expansion.TOL),
                    new RewardEntry(houseAddOns,    1157071, typeof(BBQSmokerDeed),        Expansion.TOL),
                    new RewardEntry(houseAddOns,    1157070, typeof(AlchemyStationDeed),   Expansion.TOL),
                    new RewardEntry(houseAddOns,    1157989, typeof(WritingDeskDeed),      Expansion.TOL),
                }),
                new RewardList(RewardInterval, 8, new RewardEntry[]
                {
                    new RewardEntry(miscellaneous,  1076158, typeof(WeaponEngravingTool), Expansion.ML),
                    new RewardEntry(houseAddOns,   1153535, typeof(DaviesLockerAddonDeed), Expansion.SA),
                }),
                new RewardList(RewardInterval, 9, new RewardEntry[]
                {
                    new RewardEntry(monsterStatues, 1153592, typeof(MonsterStatuette), Expansion.TOL, MonsterStatuetteType.Virtuebane),
                    new RewardEntry(etherealSteeds, 1076159, typeof(RideablePolarBear), Expansion.ML),
                    new RewardEntry(houseAddOns,    1080549, typeof(WallBannerDeed), Expansion.ML)
                }),
                new RewardList(RewardInterval, 10, new RewardEntry[]
                {
                    new RewardEntry(monsterStatues, 1080520, typeof(MonsterStatuette), Expansion.ML, MonsterStatuetteType.Harrower),
                    new RewardEntry(monsterStatues, 1080521, typeof(MonsterStatuette), Expansion.ML, MonsterStatuetteType.Efreet),

                    new RewardEntry(cloaksAndRobes, 1080382, typeof(RewardCloak), Expansion.ML, Pink, 1080382),
                    new RewardEntry(cloaksAndRobes, 1080380, typeof(RewardRobe), Expansion.ML, Pink, 1080380),
                    new RewardEntry(cloaksAndRobes, 1113898, typeof(RewardGargishFancyRobe), Expansion.SA, Pink, 1113898),
                    new RewardEntry(cloaksAndRobes, 1113899, typeof(RewardGargishRobe), Expansion.SA, Pink, 1113899),
                    new RewardEntry(cloaksAndRobes, 1080378, typeof(RewardDress), Expansion.ML, Pink, 1080378),
                    new RewardEntry(cloaksAndRobes, 1080383, typeof(RewardCloak), Expansion.ML, Crimson, 1080383),
                    new RewardEntry(cloaksAndRobes, 1080381, typeof(RewardRobe), Expansion.ML, Crimson, 1080381),
                    new RewardEntry(cloaksAndRobes, 1113900, typeof(RewardGargishFancyRobe), Expansion.SA, Crimson, 1113900),
                    new RewardEntry(cloaksAndRobes, 1113901, typeof(RewardGargishRobe), Expansion.SA, Crimson, 1113901),
                    new RewardEntry(cloaksAndRobes, 1080379, typeof(RewardDress), Expansion.ML, Crimson, 1080379),

                    new RewardEntry(etherealSteeds, 1080386, typeof(EtherealCuSidhe), Expansion.ML),

                    new RewardEntry(houseAddOns,    1080548, typeof(MiningCartDeed), Expansion.ML),
                    new RewardEntry(houseAddOns,    1080397, typeof(AnkhOfSacrificeDeed), Expansion.ML),
                    new RewardEntry(houseAddOns,    1150120, typeof(SkullRugAddonDeed), Expansion.SA),
                    new RewardEntry(houseAddOns,    1150121, typeof(RoseRugAddonDeed), Expansion.SA),
                    new RewardEntry(houseAddOns,    1150122, typeof(DolphinRugAddonDeed), Expansion.SA),
                    new RewardEntry(houseAddOns,    1157996, typeof(KoiPondDeed), Expansion.TOL),
                }),
                new RewardList(RewardInterval, 11, new RewardEntry[]
                {
                    new RewardEntry(etherealSteeds, 1113908, typeof(EtherealReptalon), Expansion.ML),
                    
                    new RewardEntry(cloaksAndRobes, 1113902, typeof(RewardCloak), GreenForest, 1113902),
                    new RewardEntry(cloaksAndRobes, 1113903, typeof(RewardDress), GreenForest, 1113903),
                    new RewardEntry(cloaksAndRobes, 1113904, typeof(RewardRobe), GreenForest, 1113904),
                    new RewardEntry(cloaksAndRobes, 1113905, typeof(RewardGargishFancyRobe), Expansion.SA, GreenForest, 1113905),
                    new RewardEntry(cloaksAndRobes, 1113906, typeof(RewardGargishRobe), Expansion.SA, GreenForest, 1113906),

                    new RewardEntry(monsterStatues, 1113800, typeof(MonsterStatuette), MonsterStatuetteType.TerathanMatriarch),                    
                    new RewardEntry(monsterStatues, 1153593, typeof(MonsterStatuette), MonsterStatuetteType.Navrey),

                    new RewardEntry(miscellaneous,  1113814, typeof(EtherealRetouchingTool), Expansion.SA),
                }),
                new RewardList(RewardInterval, 12, new RewardEntry[]
                {
                    new RewardEntry(etherealSteeds, 1113813, typeof(EtherealHiryu), Expansion.ML),
                    
                    new RewardEntry(cloaksAndRobes, 1113910, typeof(RewardCloak), RoyalBlue, 1113910),
                    new RewardEntry(cloaksAndRobes, 1113911, typeof(RewardDress), RoyalBlue, 1113911),
                    new RewardEntry(cloaksAndRobes, 1113912, typeof(RewardRobe), RoyalBlue, 1113912),
                    new RewardEntry(cloaksAndRobes, 1113913, typeof(RewardGargishFancyRobe), Expansion.SA, RoyalBlue, 1113913),
                    new RewardEntry(cloaksAndRobes, 1113914, typeof(RewardGargishRobe), Expansion.SA, RoyalBlue, 1113914),

                    new RewardEntry(monsterStatues, 1113801, typeof(MonsterStatuette), MonsterStatuetteType.FireAnt),

                    new RewardEntry(houseAddOns,    1113954, typeof(AllegiancePouch)),
                }),
                new RewardList(RewardInterval, 13, new RewardEntry[]
                {
                    new RewardEntry(etherealSteeds, 1150006, typeof(EtherealBoura), Expansion.SA),
                    new RewardEntry(monsterStatues, 1153594, typeof(MonsterStatuette), MonsterStatuetteType.Exodus),
                }),
                new RewardList(RewardInterval, 15, new RewardEntry[]
                {
                    new RewardEntry(etherealSteeds, 1154589, typeof(EtherealTiger), Expansion.TOL),
                    new RewardEntry(etherealSteeds, 1155723, typeof(EtherealAncientHellHound), Expansion.TOL),
                    new RewardEntry(etherealSteeds, 1157081, typeof(EtherealTarantula), Expansion.TOL),

                    new RewardEntry(houseAddOns,    1153491, typeof(GardenShedDeed), Expansion.TOL),
                }),
                new RewardList(RewardInterval, 20, new RewardEntry[]
                {
                    new RewardEntry(etherealSteeds, 1157995, typeof(EtherealSerpentineDragon), Expansion.TOL)
                }),
            };
        }

        public static void Initialize()
        {
            if (Enabled)
                EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_Login(LoginEventArgs e)
        {
            if (!e.Mobile.Alive)
                return;

            int cur, max, level;

            ComputeRewardInfo(e.Mobile, out cur, out max, out level);

            if (level > SkillCapBonusLevels)
                level = SkillCapBonusLevels;
            else if (level < 0)
                level = 0;

            if (!Core.SA)
            {
                if (SkillCapRewards)
                {
                    int newLevel = SkillCap + (int)((float)level * SkillCapBonusIncrement);
                    if (newLevel > SkillCap + SkillCapBonus)
                    {
                        newLevel = SkillCap + SkillCapBonus;
                    }
                    e.Mobile.SkillsCap = newLevel;
                }
                else
                {
                    e.Mobile.SkillsCap = SkillCap;
                }
            }
            else
            {
                e.Mobile.SkillsCap = SkillCap + SkillCapBonus;
            }

            if (Core.ML && e.Mobile is PlayerMobile && !((PlayerMobile)e.Mobile).HasStatReward && HasHalfLevel(e.Mobile))
            {
                Server.Gumps.BaseGump.SendGump(new StatRewardGump((PlayerMobile)e.Mobile));
            }

            if (cur < max)
                e.Mobile.SendGump(new RewardNoticeGump(e.Mobile));
        }
    }
}
