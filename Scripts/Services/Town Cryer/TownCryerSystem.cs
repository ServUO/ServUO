using Server.Commands;
using Server.ContextMenus;
using Server.Engines.CityLoyalty;
using Server.Engines.Khaldun;
using Server.Engines.Quests;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Server.Services.TownCryer
{
    public static class TownCryerSystem
    {
        public static bool Enabled { get; set; }

        public static readonly int MaxNewsEntries = 100;
        public static readonly int MaxPerGuildEntries = 1;
        public const int MaxPerCityGoverrnorEntries = 5;
        public const int MaxEMEntries = 15;
        public const int MinGuildMemberCount = 20;

        public static bool UsePreloadedMessages = false;
        public static AccessLevel EMAccess = AccessLevel.Counselor;
        public static readonly string EMEventsPage = "https://uo.com/live-events/";

        private static readonly string PreLoadedPath = "Data/PreLoadedTC.xml";

        public static List<TownCryerGreetingEntry> GreetingsEntries { get; private set; }
        //public static List<TextDefinition> GreetingsEntries { get; private set; }
        public static List<TownCryerNewsEntry> NewsEntries { get; private set; }
        public static List<TownCryerModeratorEntry> ModeratorEntries { get; private set; }
        public static List<TownCryerCityEntry> CityEntries { get; private set; }
        public static List<TownCryerGuildEntry> GuildEntries { get; private set; }
        public static List<PlayerMobile> TownCryerExempt { get; private set; }

        public static Dictionary<Mobile, DateTime> MysteriousPotionEffects { get; private set; }

        public static Timer Timer { get; private set; }
        public static bool NewGreeting { get; private set; }

        public static void Configure()
        {
            GreetingsEntries = new List<TownCryerGreetingEntry>();
            ModeratorEntries = new List<TownCryerModeratorEntry>();
            CityEntries = new List<TownCryerCityEntry>();
            GuildEntries = new List<TownCryerGuildEntry>();
            NewsEntries = new List<TownCryerNewsEntry>();
            TownCryerExempt = new List<PlayerMobile>();

            GreetingsEntries.Add(new TownCryerGreetingEntry(1158955));
            /*<center>Rising Tide</center><br><br>The Seas call to us once more! A powerful pirate called Hook has
             * taken control of the Guild, an organization of cutthroats and brigands engaged in high seas piracy!
             * Great peril stands in the way of those brave enough to challenge Hook's vile plan - read the latest
             * headlines in the Town Cryer to learn more!<br><br>The realms tinkers have been busy at work and are
             * proud to announce advancements in ship to ship ballistics!  The cannon firing process has been streamlined
             * - from crafting supplies through loading the cannons and lighting the fuse!  FIRE IN THE HOLE!
             * <br><br>Whether you are celebrating your first year in Britannia or your 22nd we want to extend a
             * very special thank you to our veteran players!  New veteran rewards are available! New MONSTER STATUETTES
             * featuring Krampus, Khal Ankur, and the Krampus Minion, are available!  Decorate your home with the WATER
             * WHEEL and personalize your clothes with the EMBROIDERY TOOL.  Every crafter will want to get their hands
             * on the REPAIR BENCH and TINKER BENCH!*/

            GreetingsEntries.Add(new TownCryerGreetingEntry(1158757));
            /*Fall is approaching and strangeness is afoot in Britannia!<br><br>Britannians are looking skyward in 
             * search of constellations and other celestial objects using the new telescope!<br><br>The pumpkin patches 
             * of Britannia once again bearing fruit as the Grimms hold their carveable pumpkins close!<br><br>Visit a 
             * cemetery to battle against the Butchers and carve new Jack o' Lantern designs!<br><br>Beware the 
             * skeletons! Zombie skeletons roam the cemeteries!<br><br>Trick or Treat? Shopkeepers and citizens 
             * alike have new treats to share!<br><br>Strange events worth investigating? A new article from the 
             * Town Cryer on the new Royal Britannian Guard Detective Branch!*/

            GreetingsEntries.Add(new TownCryerGreetingEntry(1158388));
            /* Greetings, Avatar!<br><br>Welcome to Britannia! Whether these are your first steps or you are a 
             * seasoned veteran King Blackthorn welcomes you! The realm is bustling with opportunities for adventure!
             * TownCryers can be visited at all banks and points of interest to learn about the latest goings on in 
             * the realm. Many guilds are actively recruiting members, so be sure to check the Town Cryer guild 
             * section for the latest recruitment events. <br><br>We wish you the best of luck in your
             * <A HREF="https://uo.com/endless-journey/">Endless Journey</A>*/

            LoadPreloadedMessages();
        }

        public static void Initialize()
        {
            if (Enabled)
            {
                EventSink.Login += OnLogin;

                NewsEntries.Add(new TownCryerNewsEntry(1159346, 1159347, 0x9D3E, null, "https://uo.com/wiki/ultima-online-wiki/combat/jolly-roger/")); // Jolly Roger
                NewsEntries.Add(new TownCryerNewsEntry(1159262, 1159263, 0x64E, null, "https://uo.com/wiki/ultima-online-wiki/seasonal-events/halloween-treasures-of-the-sea/")); // Forsaken Foes
                NewsEntries.Add(new TownCryerNewsEntry(1158944, 1158945, 0x9CEA, null, "https://uo.com/wiki/ultima-online-wiki/combat/pvm-player-versus-monster/rising-tide/")); // Rising Tide
                NewsEntries.Add(new TownCryerNewsEntry(1158552, 1158553, 0x6CE, typeof(GoingGumshoeQuest), null)); // Going Gumshoe
                NewsEntries.Add(new TownCryerNewsEntry(1158095, 1158097, 0x61E, null, "https://uo.com/")); // Britain Commons
                NewsEntries.Add(new TownCryerNewsEntry(1158089, 1158091, 0x60F, null, "https://uo.com/wiki/ultima-online-wiki/gameplay/npc-commercial-transactions/clean-up-britannia/")); // Cleanup Britannia
                NewsEntries.Add(new TownCryerNewsEntry(1158098, 1158100, 0x615, null, "https://uo.com/wiki/ultima-online-wiki/gameplay/crafting/bulk-orders/")); // New Bulk Orders
                NewsEntries.Add(new TownCryerNewsEntry(1158101, 1158103, 0x616, null, "https://uo.com/wiki/ultima-online-wiki/a-summary-for-returning-players/weapons-armor-and-loot-revamps-2016/")); // 2016 Loot Revamps
                NewsEntries.Add(new TownCryerNewsEntry(1158116, 1158118, 0x64F, null, "https://uo.com/wiki/ultima-online-wiki/gameplay/the-virtues/")); // Virtues
                NewsEntries.Add(new TownCryerNewsEntry(1158083, 1158085, 0x617, typeof(TamingPetQuest), "https://uo.com/wiki/ultima-online-wiki/skills/animal-taming/animal-training/")); // Animal Training
                NewsEntries.Add(new TownCryerNewsEntry(1158086, 1158088, 0x61D, typeof(ExploringTheDeepQuest), null));
                NewsEntries.Add(new TownCryerNewsEntry(1158092, 1158094, 0x651, typeof(HuntmastersChallengeQuest), "https://uo.com/wiki/ultima-online-wiki/gameplay/huntmasters-challenge/")); // Huntsmaster Challenge 
                NewsEntries.Add(new TownCryerNewsEntry(1158104, 1158106, 0x61C, typeof(PaladinsOfTrinsic), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-shame/")); //  New Shame 
                NewsEntries.Add(new TownCryerNewsEntry(1158107, 1158109, 0x61A, typeof(RightingWrongQuest), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-wrong/")); // New Wrong

                if (TreasureMapInfo.NewSystem)
                {
                    NewsEntries.Add(new TownCryerNewsEntry(1158113, 1158115, 0x64C, typeof(BuriedRichesQuest), "https://uo.com/wiki/ultima-online-wiki/gameplay/treasure-maps/")); // New TMaps
                }

                NewsEntries.Add(new TownCryerNewsEntry(1158119, 1158121, 0x64D, typeof(APleaFromMinocQuest), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-covetous/")); // New Covetous
                NewsEntries.Add(new TownCryerNewsEntry(1158110, 1158112, 0x64E, typeof(AVisitToCastleBlackthornQuest), "https://uo.com/wiki/ultima-online-wiki/items/artifacts-castle-blackthorn/")); // Castle Blackthorn
                NewsEntries.Add(new TownCryerNewsEntry(1158122, 1158124, 0x650, typeof(WishesOfTheWispQuest), "https://uo.com/wiki/ultima-online-wiki/world/dungeons/dungeon-despise-trammel/")); // New Despise

                // New greeting, resets all TC hiding
                if (NewGreeting)
                {
                    TownCryerExempt.Clear();
                }

                if (UsePreloadedMessages)
                {
                    CommandSystem.Register("ReloadTCGreetings", AccessLevel.Administrator, Reload_OnCommand);
                }
            }
        }

        public static bool IsExempt(Mobile m)
        {
            return m is PlayerMobile && TownCryerExempt.Contains((PlayerMobile)m);
        }

        public static void AddExempt(PlayerMobile pm)
        {
            if (!TownCryerExempt.Contains(pm))
            {
                TownCryerExempt.Add(pm);
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

        public static void AddEntry(TownCryerGreetingEntry entry)
        {
            GreetingsEntries.Add(entry);
            CheckTimer();
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
            if (Enabled && e.Mobile is PlayerMobile mobile && !IsExempt(mobile))
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), player =>
                {
                    if (HasCustomEntries())
                    {
                        BaseGump.SendGump(new TownCryerGreetingsGump(player, null));
                    }
                    else
                    {
                        IPooledEnumerable eable = player.Map.GetMobilesInRange(player.Location, 20);

                        foreach (Mobile m in eable)
                        {
                            if (m is TownCrier)
                            {
                                BaseGump.SendGump(new TownCryerGreetingsGump(player, (TownCrier)m));
                                break;
                            }
                        }

                        eable.Free();
                    }

                }, mobile);
            }
        }

        public static int CityEntryCount(City city)
        {
            return CityEntries.Count(x => x.City == city);
        }

        public static bool HasGuildEntry(Guild g)
        {
            if (g == null)
                return false;

            return GuildEntries.Any(x => x.Guild == g);
        }

        public static bool HasCustomEntries()
        {
            return GreetingsEntries.Any(x => x.Saves || x.Expires != DateTime.MinValue);
        }

        public static void CheckTimer()
        {
            if (ModeratorEntries.Count > 0 ||
                CityEntries.Count > 0 ||
                GuildEntries.Count > 0 ||
                GreetingsEntries.Any(e => e.Expires != DateTime.MinValue) ||
                MysteriousPotionEffects != null)
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
            for (int i = GreetingsEntries.Count - 1; i >= 0; i--)
            {
                if (GreetingsEntries[i].Expires != DateTime.MinValue && GreetingsEntries[i].Expired)
                    GreetingsEntries.RemoveAt(i);
            }

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
                List<Mobile> list = new List<Mobile>(MysteriousPotionEffects.Keys);

                foreach (Mobile m in list)
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
                PlayerMobile pm = from as PlayerMobile;

                if (pm.AccessLevel >= EMAccess)
                {
                    list.Add(new AddGreetingEntry(tc));
                    list.Add(new UpdateEMEntry(tc));
                }

                CityLoyaltySystem system = CityLoyaltySystem.GetCitizenship(pm, false);

                if (IsGovernor(pm, system))
                {
                    list.Add(new UpdateCityEntry(tc));
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
                (!checkQuest || m is PlayerMobile && QuestHelper.HasQuest<AForcedSacraficeQuest2>((PlayerMobile)m));
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

        #region Pre-Loaded 
        private static void LoadPreloadedMessages()
        {
            if (!Enabled || !UsePreloadedMessages)
                return;

            if (File.Exists(PreLoadedPath))
            {
                XmlDocument doc = new XmlDocument();
                Utility.WriteConsoleColor(ConsoleColor.Cyan, "*** Loading Pre-Loaded Town Crier Messages...");

                try
                {
                    doc.Load(PreLoadedPath);
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                    Utility.WriteConsoleColor(ConsoleColor.Cyan, "...FAILED! ***");
                    return;
                }

                XmlElement root = doc["preloadedTC"];
                int good = 0;
                int expired = 0;
                int errors = 0;

                if (root != null)
                {
                    int index = 0;

                    foreach (XmlElement reg in root.GetElementsByTagName("message"))
                    {
                        string title = Utility.GetText(reg["title"], null);
                        string body = Utility.GetText(reg["body"], null);
                        DateTime created = GetDateTime(Utility.GetText(reg["created"], null));
                        DateTime expires = GetDateTime(Utility.GetText(reg["expires"], null));
                        string link = Utility.GetText(reg["link"], null);
                        string linktext = Utility.GetText(reg["linktext"], null);

                        if (title == null)
                        {
                            ErrorToConsole("Invalid title", index);
                            errors++;
                        }
                        else if (body == null)
                        {
                            ErrorToConsole("Invalid body", index);
                            errors++;
                        }
                        else if (created == DateTime.MinValue)
                        {
                            ErrorToConsole("Invalid creation time", index);
                            errors++;
                        }
                        else if (expires > DateTime.Now || expires == DateTime.MinValue)
                        {
                            TownCryerGreetingEntry entry = new TownCryerGreetingEntry(title, body, -1, link, linktext)
                            {
                                PreLoaded = true,
                                Created = created
                            };

                            if (expires > created)
                            {
                                entry.Expires = expires;
                            }

                            AddEntry(entry);
                            good++;
                        }
                        else
                        {
                            ErrorToConsole("Expired message", index);
                            expired++;
                        }

                        index++;
                    }
                }

                if (expired > 0 || errors > 0)
                {
                    Utility.WriteConsoleColor(ConsoleColor.Cyan, "...Complete! Loaded {0} Pre-Loaded Messages. {1} expired messages and {2} erroneous messages not loaded! ***", good, expired, errors);
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Cyan, "...Complete! Loaded {0} Pre-Loaded Messages. ***", good);
                }
            }
        }

        public static void ErrorToConsole(string type, int index)
        {
            Utility.WriteConsoleColor(ConsoleColor.Red, "[TC Pre-Loaded Message]: {0} for pre-loaded Message #{1}", type, index.ToString());
        }

        public static DateTime GetDateTime(string text)
        {
            DateTime datetime = DateTime.MinValue;

            try
            {
                datetime = DateTime.Parse(text, CultureInfo.CreateSpecificCulture("en-US"));
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }

            return datetime;
        }

        [Usage("ReloadTCGreetings")]
        [Description("Reloads Pre-Loaded Town Cryer Messages. This enables changes to be made to the PreLoadedTC.xml and show in game without a server restart.")]
        public static void Reload_OnCommand(CommandEventArgs e)
        {
            bool clear = false;

            GreetingsEntries.IterateReverse(entry =>
                {
                    if (entry.PreLoaded)
                    {
                        GreetingsEntries.Remove(entry);

                        if (!clear)
                            clear = true;
                    }
                });

            LoadPreloadedMessages();

            if (clear)
            {
                TownCryerExempt.Clear();
            }

            e.Mobile.SendMessage("Pre-Loaded TC messages re-loaded from {0}!", PreLoadedPath);
        }
        #endregion

        public static void Save(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(GreetingsEntries.Count);

            writer.Write(TownCryerExempt.Count);
            foreach (PlayerMobile pm in TownCryerExempt)
                writer.Write(pm);

            writer.Write(GreetingsEntries.Count(x => x.Saves));
            foreach (TownCryerGreetingEntry e in GreetingsEntries.Where(x => x.Saves))
                e.Serialize(writer);

            writer.Write(ModeratorEntries.Count);
            foreach (TownCryerModeratorEntry e in ModeratorEntries)
                e.Serialize(writer);

            writer.Write(CityEntries.Count);
            foreach (TownCryerCityEntry e in CityEntries)
                e.Serialize(writer);

            writer.Write(GuildEntries.Count);
            foreach (TownCryerGuildEntry e in GuildEntries)
                e.Serialize(writer);

            writer.Write(MysteriousPotionEffects != null ? MysteriousPotionEffects.Count : 0);

            if (MysteriousPotionEffects != null)
            {
                foreach (KeyValuePair<Mobile, DateTime> kvp in MysteriousPotionEffects)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }

        public static void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            int greetingsCount = 0;

            switch (version)
            {
                case 1:
                    greetingsCount = reader.ReadInt();

                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        PlayerMobile pm = reader.ReadMobile() as PlayerMobile;

                        if (pm != null)
                        {
                            AddExempt(pm);
                        }
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        TownCryerGreetingEntry entry = new TownCryerGreetingEntry(reader);

                        if (!entry.Expired)
                        {
                            GreetingsEntries.Add(entry);
                        }
                    }
                    goto case 0;
                case 0:
                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        TownCryerModeratorEntry entry = new TownCryerModeratorEntry(reader);

                        if (!entry.Expired)
                        {
                            ModeratorEntries.Add(entry);
                        }
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        TownCryerCityEntry entry = new TownCryerCityEntry(reader);

                        if (!entry.Expired)
                        {
                            CityEntries.Add(entry);
                        }
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        TownCryerGuildEntry entry = new TownCryerGuildEntry(reader);

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
                    break;
            }

            if (greetingsCount < GreetingsEntries.Count)
            {
                NewGreeting = true;
            }

            CheckTimer();
        }
    }

    public class AddGreetingEntry : ContextMenuEntry
    {
        public TownCrier Cryer { get; }

        public AddGreetingEntry(TownCrier cryer)
            : base(1011405, 3) // Change Greeting
        {
            Cryer = cryer;
        }

        public override void OnClick()
        {
            if (Owner.From is PlayerMobile && Owner.From.AccessLevel >= AccessLevel.GameMaster)
            {
                //BaseGump.SendGump(new CreateGreetingEntryGump((PlayerMobile)Owner.From, Cryer));
                BaseGump.SendGump(new TownCryerGreetingsGump((PlayerMobile)Owner.From, Cryer));
            }
        }
    }

    public class UpdateEMEntry : ContextMenuEntry
    {
        public TownCrier Cryer { get; }

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
        public TownCrier Cryer { get; }

        public UpdateCityEntry(TownCrier cryer)
            : base(1158023, 3) // Update City Town Crier
        {
            Cryer = cryer;
        }

        public override void OnClick()
        {
            if (Owner.From is PlayerMobile)
            {
                PlayerMobile pm = Owner.From as PlayerMobile;
                CityLoyaltySystem system = CityLoyaltySystem.GetCitizenship(pm, false);

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
        public TownCrier Cryer { get; }

        public UpdateGuildEntry(Mobile from, TownCrier cryer)
            : base(1158024, 3) // Update Guild Town Crier
        {
            Cryer = cryer;
            Enabled = from.Guild != null && !TownCryerSystem.HasGuildEntry(from.Guild as Guild);
        }

        public override void OnClick()
        {
            PlayerMobile pm = Owner.From as PlayerMobile;

            if (pm != null)
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
