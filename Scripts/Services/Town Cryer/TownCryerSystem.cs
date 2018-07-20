using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CityLoyalty;
using Server.Gumps;
using Server.ContextMenus;
using Server.Guilds;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Services.TownCryer
{
    public static class TownCryerSystem
    {
        public static bool Enabled { get; set; }

        public static readonly int MaxNewsEntries = 100;
        public static readonly int MaxPerGuildEntries = 1;
        public static readonly int MaxPerCityGoverrnorEntries = 5;
        public static readonly int MaxEMEntries = 15;
        public static readonly int MinGuildMemberCount = 20;

        public static AccessLevel EMAccess = AccessLevel.Counselor;
        public static readonly string EMEventsPage = "https://uo.com/live-events/";

        public static List<TextDefinition> GreetingsEntries { get; private set; }
        public static List<TownCryerNewsEntry> NewsEntries { get; private set; }
        public static List<TownCryerModeratorEntry> ModeratorEntries { get; private set; }
        public static List<TownCryerCityEntry> CityEntries { get; private set; }
        public static List<TownCryerGuildEntry> GuildEntries { get; private set; }

        public static Dictionary<Mobile, DateTime> MysteriousPotionEffects { get; private set; }

        public static Timer Timer { get; private set; }

        public static void Configure()
        {
            ModeratorEntries = new List<TownCryerModeratorEntry>();
            CityEntries = new List<TownCryerCityEntry>();
            GuildEntries = new List<TownCryerGuildEntry>();
            GreetingsEntries = new List<TextDefinition>();
            NewsEntries = new List<TownCryerNewsEntry>();
        }

        public static void Initialize()
        {
            if (Enabled)
            {
                EventSink.Login += OnLogin;

                GreetingsEntries.Add(1158388);
                /*Greetings, Avatar!<br><br>Welcome to Britannia! Whether these are your first steps or you are a 
                         * seasoned veteran King Blackthorn welcomes you! The realm is bustling with opportunities for adventure!
                         * TownCryers can be visited at all banks and points of interest to learn about the latest goings on in 
                         * the realm. Many guilds are actively recruiting members, so be sure to check the Town Cryer guild 
                         * section for the latest recruitment events. <br><br>We wish you the best of luck in your
                         * <A HREF="https://uo.com/endless-journey/">Endless Journey</A>*/

                NewsEntries.Add(new TownCryerNewsEntry(1158083, 1158085, 0x617, typeof(TamingPetQuest), "https://uo.com/wiki/ultima-online-wiki/skills/animal-taming/animal-training/")); // Animal Training
                NewsEntries.Add(new TownCryerNewsEntry(1158086, 1158088, 0x61D, typeof(ExploringTheDeepQuest), null));
                NewsEntries.Add(new TownCryerNewsEntry(1158089, 1158091, 0x60F, null, "https://uo.com/wiki/ultima-online-wiki/gameplay/npc-commercial-transactions/clean-up-britannia/")); // Cleanup Britannia
                NewsEntries.Add(new TownCryerNewsEntry(1158092, 1158094, 0x651, typeof(HuntmastersChallengeQuest), "https://uo.com/wiki/ultima-online-wiki/gameplay/huntmasters-challenge/")); // Huntsmaster Challenge TODO: Quest
                NewsEntries.Add(new TownCryerNewsEntry(1158098, 1158100, 0x615, null, "https://uo.com/wiki/ultima-online-wiki/gameplay/crafting/bulk-orders/")); // New Bulk Orders
                NewsEntries.Add(new TownCryerNewsEntry(1158101, 1158103, 0x616, null, "https://uo.com/wiki/ultima-online-wiki/a-summary-for-returning-players/weapons-armor-and-loot-revamps-2016/")); // 2016 Loot Revamps
                NewsEntries.Add(new TownCryerNewsEntry(1158104, 1158106, 0x61C, typeof(PaladinsOfTrinsic), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-shame/")); //  New Shame TODO:Paladins of Trinsics QUEST?
                NewsEntries.Add(new TownCryerNewsEntry(1158107, 1158109, 0x61A, typeof(RightingWrongQuest), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-wrong/")); // New Wrong  TODO: Righting Wrong Quest
                NewsEntries.Add(new TownCryerNewsEntry(1158110, 1158112, 0x64E, typeof(AVisitToCastleBlackthornQuest), "https://uo.com/wiki/ultima-online-wiki/items/artifacts-castle-blackthorn/")); // Castle Blackthorn TODO: A Visit to Castle Blackthorn Quest
                NewsEntries.Add(new TownCryerNewsEntry(1158113, 1158115, 0x64C, typeof(BuriedRichesQuest), "https://uo.com/wiki/ultima-online-wiki/gameplay/treasure-maps/")); // New TMaps TODO: Buried Riches Quest
                NewsEntries.Add(new TownCryerNewsEntry(1158116, 1158118, 0x64F, null, "https://uo.com/wiki/ultima-online-wiki/gameplay/the-virtues/")); // Virues
                NewsEntries.Add(new TownCryerNewsEntry(1158119, 1158121, 0x64D, typeof(APleaFromMinocQuest), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-covetous/")); // New Covetous TODO: A Plea From Minoc Quest
                NewsEntries.Add(new TownCryerNewsEntry(1158122, 1158124, 0x650, typeof(WishesOfTheWispQuest), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-despise-trammel/")); // New Despise TODO: Wishes of the Wisp Quest
            }
        }

        public static void AddEntry(TownCryerModeratorEntry entry)
        {
            ModeratorEntries.Add(entry);
            CheckTimer();
        }

        public static void AddEntry(TownCryerCityEntry entry)
        {
            CityEntries.Add(entry);
            CheckTimer();
        }

        public static void AddEntry(TownCryerGuildEntry entry)
        {
            GuildEntries.Add(entry);
            CheckTimer();

            entry.GetExpiration();
        }

        public static void CompleteQuest(PlayerMobile pm, BaseQuest quest)
        {
            BaseGump.SendGump(new TownCrierQuestCompleteGump(pm, quest));
        }

        public static void CompleteQuest(PlayerMobile pm, object title, object body, int gumpID)
        {
            BaseGump.SendGump(new TownCrierQuestCompleteGump(pm, title, body, gumpID));
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (Enabled && e.Mobile is PlayerMobile && !((PlayerMobile)e.Mobile).HideTownCrierGreetingGump)
            {
                Timer.DelayCall<PlayerMobile>(TimeSpan.FromSeconds(1), player =>
                    {
                        IPooledEnumerable eable = player.Map.GetMobilesInRange(player.Location, 25);

                        foreach (Mobile m in eable)
                        {
                            if (m is TownCrier)
                            {
                                BaseGump.SendGump(new TownCryerGreetingsGump(player, (TownCrier)m));
                                break;
                            }
                        }

                        eable.Free();
                    }, (PlayerMobile)e.Mobile);
            }
        }

        public static int CityEntryCount(City city)
        {
            return CityEntries.Where(x => x.City == city).Count();
        }

        public static bool HasGuildEntry(Guild g)
        {
            if (g == null)
                return false;

            return GuildEntries.Any(x => x.Guild == g);
        }

        public static void CheckTimer()
        {
            if (ModeratorEntries.Count > 0 || CityEntries.Count > 0 || GuildEntries.Count > 0 || MysteriousPotionEffects != null)
            {
                if (Timer == null || !Timer.Running)
                {
                    Timer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), CheckExpiredEntries);
                    Timer.Priority = TimerPriority.OneMinute;
                }
            }
            else if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public static void CheckExpiredEntries()
        {
            for (int i = ModeratorEntries.Count - 1; i >= 0; i--)
            {
                if (ModeratorEntries[i].Expired)
                    ModeratorEntries.RemoveAt(i);
            }

            for (int i = CityEntries.Count - 1; i >= 0; i--)
            {
                if (CityEntries[i].Expired)
                    CityEntries.RemoveAt(i);
            }

            for (int i = GuildEntries.Count - 1; i >= 0; i--)
            {
                if (GuildEntries[i].Expired)
                    GuildEntries.RemoveAt(i);
            }

            if (MysteriousPotionEffects != null)
            {
                var list = new List<Mobile>(MysteriousPotionEffects.Keys);

                foreach (var m in list)
                {
                    if (MysteriousPotionEffects != null && MysteriousPotionEffects.ContainsKey(m) && MysteriousPotionEffects[m] < DateTime.UtcNow)
                    {
                        MysteriousPotionEffects.Remove(m);

                        if (MysteriousPotionEffects.Count == 0)
                        {
                            MysteriousPotionEffects = null;
                        }
                    }
                }

                ColUtility.Free(list);
            }

            CheckTimer();
        }

        public static bool IsGovernor(PlayerMobile pm, CityLoyaltySystem system)
        {
            return system != null && system.Governor == pm;
        }

        public static void GetContextMenus(TownCrier tc, Mobile from, List<ContextMenuEntry> list)
        {
            if (from is PlayerMobile)
            {
                var pm = from as PlayerMobile;

                if (pm.AccessLevel >= EMAccess)
                {
                    list.Add(new UpdateEMEntry(tc));
                }

                var system = CityLoyaltySystem.GetCitizenship(pm, false);

                if (IsGovernor(pm, system))
                {
                    list.Add(new UpdateCityEntry(tc, system.City));
                }

                Guild g = pm.Guild as Guild;

                if (g != null && pm.GuildRank != null && pm.GuildRank.Rank >= 3 && g.Leader == pm && (pm.AccessLevel > AccessLevel.Player || g.Members.Count >= MinGuildMemberCount))
                {
                    list.Add(new UpdateGuildEntry(from, tc));
                }
            }
        }

        public static int GetCityLoc(City city)
        {
            switch (city)
            {
                default:
                case City.Britain: return 1158043;
                case City.Jhelom: return 1158044;
                case City.Minoc: return 1158045;
                case City.Moonglow: return 1158046;
                case City.NewMagincia: return 1158047;
                case City.SkaraBrae: return 1158048;
                case City.Trinsic: return 1158049;
                case City.Vesper: return 1158050;
                case City.Yew: return 1158051;
            }
        }

        public static bool UnderMysteriousPotionEffects(Mobile m, bool checkQuest = false)
        {
            return 
                MysteriousPotionEffects != null && MysteriousPotionEffects.ContainsKey(m) && MysteriousPotionEffects[m] > DateTime.UtcNow &&
                (!checkQuest || (m is PlayerMobile && QuestHelper.HasQuest<AForcedSacraficeQuest2>((PlayerMobile)m)));
        }

        public static void AddMysteriousPotionEffects(Mobile m)
        {
            if (MysteriousPotionEffects == null)
            {
                MysteriousPotionEffects = new Dictionary<Mobile, DateTime>();
            }

            MysteriousPotionEffects[m] = DateTime.UtcNow + TimeSpan.FromDays(3);

            CheckTimer();
        }

        public static void Save(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(ModeratorEntries.Count);
            foreach (var e in ModeratorEntries)
                e.Serialize(writer);

            writer.Write(CityEntries.Count);
            foreach (var e in CityEntries)
                e.Serialize(writer);

            writer.Write(GuildEntries.Count);
            foreach (var e in GuildEntries)
                e.Serialize(writer);

            writer.Write(MysteriousPotionEffects != null ? MysteriousPotionEffects.Count : 0);

            if (MysteriousPotionEffects != null)
            {
                foreach (var kvp in MysteriousPotionEffects)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }

        public static void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var entry = new TownCryerModeratorEntry(reader);

                if (!entry.Expired)
                {
                    ModeratorEntries.Add(entry);
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var entry = new TownCryerCityEntry(reader);

                if (!entry.Expired)
                {
                    CityEntries.Add(entry);
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var entry = new TownCryerGuildEntry(reader);

                if (!entry.Expired)
                {
                    GuildEntries.Add(entry);
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                DateTime dt = reader.ReadDateTime();

                if (m != null)
                {
                    if (MysteriousPotionEffects == null)
                        MysteriousPotionEffects = new Dictionary<Mobile, DateTime>();

                    MysteriousPotionEffects[m] = dt;
                }
            }

            CheckTimer();
        }
    }

    public class UpdateEMEntry : ContextMenuEntry
    {
        public TownCrier Cryer { get; set; }

        public UpdateEMEntry(TownCrier cryer)
            : base(1158022, 3) // Update EM Town Crier
        {
            Cryer = cryer;
            Enabled = TownCryerSystem.ModeratorEntries.Count < TownCryerSystem.MaxEMEntries;
        }

        public override void OnClick()
        {
            if (Owner.From is PlayerMobile)
            {
                if (TownCryerSystem.ModeratorEntries.Count < TownCryerSystem.MaxEMEntries)
                {
                    BaseGump.SendGump(new CreateEMEntryGump((PlayerMobile)Owner.From, Cryer));
                }
                else
                {
                    Owner.From.SendLocalizedMessage(1158038); // You have reached the maximum entry count.  Please remove some and try again.
                }
            }
        }
    }

    public class UpdateCityEntry : ContextMenuEntry
    {
        public City City { get; set; }
        public TownCrier Cryer { get; set; }

        public UpdateCityEntry(TownCrier cryer, City city)
            : base(1158023, 3) // Update City Town Crier
        {
            Cryer = cryer;
            City = city;
        }

        public override void OnClick()
        {
            if (Owner.From is PlayerMobile)
            {
                var pm = Owner.From as PlayerMobile;
                var system = CityLoyaltySystem.GetCitizenship(pm, false);

                if (TownCryerSystem.IsGovernor(pm, system))
                {
                    if (TownCryerSystem.CityEntryCount(system.City) < TownCryerSystem.MaxPerCityGoverrnorEntries)
                    {
                        BaseGump.SendGump(new CreateCityEntryGump(pm, Cryer, system.City));
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1158038); // You have reached the maximum entry count.  Please remove some and try again.
                    }
                }
            }
        }
    }

    public class UpdateGuildEntry : ContextMenuEntry
    {
        public TownCrier Cryer { get; set; }

        public UpdateGuildEntry(Mobile from, TownCrier cryer)
            : base(1158024, 3) // Update Guild Town Crier
        {
            Cryer = cryer;
            Enabled = from.Guild != null && !TownCryerSystem.HasGuildEntry(from.Guild as Guild);
        }

        public override void OnClick()
        {
            PlayerMobile pm = Owner.From as PlayerMobile;

            if(pm != null)
            {
                Guild g = pm.Guild as Guild;

                if (g != null && pm.GuildRank != null && pm.GuildRank.Rank >= 3 && (pm.AccessLevel > AccessLevel.Player || g.Members.Count >= TownCryerSystem.MinGuildMemberCount))
                {
                    if (TownCryerSystem.HasGuildEntry(g))
                    {
                        Owner.From.SendLocalizedMessage(1158038); // You have reached the maximum entry count.  Please remove some and try again.
                    }
                    else
                    {
                        BaseGump.SendGump(new CreateGuildEntryGump(pm, Cryer));
                    }
                }
                else
                {
                    pm.SendLocalizedMessage(1158025); // Only Guild Leaders and Warlords of guilds with at least 20 members may post in the Town Cryer.
                }
            }
        }
    }
}