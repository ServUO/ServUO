using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.ArenaSystem
{
    public class PVPArenaSystemSetupGump : BaseGump
    {
        public PVPArenaSystemSetupGump(PlayerMobile pm)
            : base(pm, 150, 20)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 300, 295, 9200);

            AddImageTiled(8, 8, 284, 27, 2624);
            AddAlphaRegion(8, 8, 284, 27);

            AddImageTiled(8, 41, 284, 51, 2624);
            AddAlphaRegion(8, 41, 284, 51);

            AddImageTiled(8, 98, 284, 164, 2624);
            AddAlphaRegion(8, 98, 284, 164);

            AddImageTiled(8, 268, 284, 20, 2624);
            AddAlphaRegion(8, 268, 284, 20);

            AddHtml(0, 12, 300, 20, ColorAndCenter("#FFFFFF", "Arena Setup"), false, false);
            AddHtml(12, 47, 274, 40, Color("#FFFFFF", "Below are the available PVP Arena's."), false, false);

            for (int i = 0; i < ArenaDefinition.Definitions.Length; i++)
            {
                ArenaDefinition def = ArenaDefinition.Definitions[i];
                bool exists = PVPArenaSystem.Arenas != null && PVPArenaSystem.Arenas.Any(arena => arena.Definition == def);

                AddHtml(45, 105 + (i * 25), 200, 20, Color("#FFFFFF", string.Format("{0} [{1}]", def.Name, exists ? "Enabled" : PVPArenaSystem.Instance != null && PVPArenaSystem.Instance.IsBlocked(def) ? "Blocked" : "Disabled")), false, false);
                AddButton(10, 105 + (i * 25), !exists ? 4023 : 4017, !exists ? 4024 : 4018, i + 500, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(42, 268, 150, 20, 1150300, 0xFFFF, false, false); // CANCEL
            AddButton(8, 268, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            int id = info.ButtonID - 500;

            if (id >= 0 && id < ArenaDefinition.Definitions.Length)
            {
                ArenaDefinition def = ArenaDefinition.Definitions[id];
                bool exists = PVPArenaSystem.Arenas != null && PVPArenaSystem.Arenas.Any(arena => arena.Definition == def);

                SendGump(new GenericConfirmCallbackGump<ArenaDefinition>(
                    User,
                    string.Format("{0} {1}", exists ? "Disable" : "Enable", def.Name),
                    exists ? _DisableBody : _EnableBody,
                    def,
                    null,
                    (m, d) =>
                    {
                        if (PVPArenaSystem.Instance != null)
                        {
                            if (PVPArenaSystem.Instance.IsBlocked(d))
                            {
                                PVPArenaSystem.Instance.RemoveBlockedArena(d);
                            }
                            else
                            {
                                if (PVPArenaSystem.Arenas != null)
                                {
                                    PVPArena arena = PVPArenaSystem.Arenas.FirstOrDefault(a => a.Definition == d);

                                    if (arena != null)
                                    {
                                        PVPArenaSystem.Instance.AddBlockedArena(arena);
                                    }
                                }
                            }
                        }

                        SendGump(m as PlayerMobile);
                    },
                    (m, d) =>
                    {
                        SendGump(m as PlayerMobile);
                    }));
            }
        }

        private static void SendGump(PlayerMobile pm)
        {
            if (pm == null)
                return;

            PVPArenaSystemSetupGump gump = GetGump<PVPArenaSystemSetupGump>(pm, null);

            if (gump == null)
            {
                SendGump(new PVPArenaSystemSetupGump(pm));
            }
            else
            {
                gump.Refresh();
            }
        }

        private readonly string _DisableBody = "By disabling this arena, the arena stone, regions and any stats associated with the arena will be lost forever. Do you wish to proceed?";
        private readonly string _EnableBody = "By enabling this region, an arena stone and regions will be placed. Be sure to clear any custom content you have at or near the arena.";
    }

    public class BaseArenaGump : BaseGump
    {
        public int LabelHue = 0x480;

        public PVPArena Arena { get; private set; }

        public override bool CloseOnMapChange => true;

        public BaseArenaGump(PlayerMobile pm, PVPArena arena)
            : base(pm, 30, 30)
        {
            Arena = arena;
            CloseArenaGumps(pm);
        }

        public override void AddGumpLayout()
        {
        }

        public static void CloseArenaGumps(Mobile m)
        {
            m.CloseGump(typeof(CreateDuelGump));
            m.CloseGump(typeof(ArenaStoneGump));
            m.CloseGump(typeof(PendingDuelGump));
            m.CloseGump(typeof(CreateDuelGump));
            m.CloseGump(typeof(JoinDuelGump));
            m.CloseGump(typeof(OfferDuelGump));
            m.CloseGump(typeof(BookedDuelsGump));
            m.CloseGump(typeof(IndividualStatsGump));
            m.CloseGump(typeof(DuelResultsGump));
            m.CloseGump(typeof(DuelRulesGump));
            m.CloseGump(typeof(ArenaRankingsGump));
        }
    }

    public class BaseDuelGump : BaseArenaGump
    {
        public ArenaDuel Duel { get; protected set; }

        public BaseDuelGump(PlayerMobile pm, PVPArena arena, ArenaDuel duel)
            : base(pm, arena)
        {
            Duel = duel;
        }

        protected string GetTimeLimit()
        {
            switch (Duel.TimeLimit)
            {
                default:
                case TimeLimit.FiveMinutes: return "5 minutes";
                case TimeLimit.TenMinutes: return "10 minutes";
                case TimeLimit.FifteenMinutes: return "15 minutes";
                case TimeLimit.TwentyMinutes: return "20 minutes";
                case TimeLimit.ThirtyMinutes: return "30 minutes";
            }
        }

        protected string GetEntryFee()
        {
            switch (Duel.EntryFee)
            {
                default:
                case EntryFee.Zero: return "0 gold";
                case EntryFee.OneThousand: return "1,000 gold";
                case EntryFee.FiveThousand: return "5,000 gold";
                case EntryFee.TenThousand: return "10,000 gold";
                case EntryFee.TwentyFiveThousand: return "25,000 gold";
                case EntryFee.FiftyThousand: return "50,000 gold";
            }
        }

        protected string GetSlots()
        {
            return string.Format("{0} slot(s)", Duel.PetSlots);
        }

        protected string GetPotion()
        {
            switch (Duel.PotionRules)
            {
                default:
                case PotionRules.All: return "ALL OK";
                case PotionRules.None: return "NONE";
                case PotionRules.NoHealing: return "NO HEALING";
            }
        }
    }

    public class ArenaStoneGump : BaseArenaGump
    {
        public ArenaStoneGump(PlayerMobile pm, PVPArena arena)
            : base(pm, arena)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 300, 295, 9200);

            AddImageTiled(8, 8, 284, 27, 2624);
            AddAlphaRegion(8, 8, 284, 27);

            AddImageTiled(8, 41, 284, 51, 2624);
            AddAlphaRegion(8, 41, 284, 51);

            AddImageTiled(8, 98, 284, 164, 2624);
            AddAlphaRegion(8, 98, 284, 164);

            AddImageTiled(8, 268, 284, 20, 2624);
            AddAlphaRegion(8, 268, 284, 20);

            AddHtmlLocalized(0, 12, 300, 20, CenterLoc, "#1115619", 0xFFFF, false, false); // Arena Menu - Main
            AddHtmlLocalized(12, 47, 274, 40, 1115620, 0xFFFF, false, false); // You can host/join a duel or check your duel stats.

            PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

            AddHtmlLocalized(45, 105, 200, 20, 1115621, 0xFFFF, false, false); // Host a duel
            AddHtmlLocalized(45, 130, 200, 20, 1115622, 0xFFFF, false, false); // Join a duel
            AddHtmlLocalized(45, 155, 200, 20, 1115925, 0xFFFF, false, false); // See booked duels
            AddHtmlLocalized(45, 180, 200, 20, 1115623, 0xFFFF, false, false); // Check your stats
            AddHtmlLocalized(45, 205, 200, 20, 1150128, 0xFFFF, false, false); // Check arena rankings
            AddHtmlLocalized(45, 230, 200, 20, entry.IgnoreInvites ? 1116155 : 1116156, 0xFFFF, false, false); // Ignore duel invite (ON) / Ignore duel invite (OFF)

            AddButton(10, 105, 4023, 4024, 1, GumpButtonType.Reply, 0);
            AddButton(10, 130, 4023, 4024, 2, GumpButtonType.Reply, 0);
            AddButton(10, 155, 4023, 4024, 3, GumpButtonType.Reply, 0);
            AddButton(10, 180, 4023, 4024, 4, GumpButtonType.Reply, 0);
            AddButton(10, 205, 4023, 4024, 5, GumpButtonType.Reply, 0);
            AddButton(10, 230, 4023, 4024, 6, GumpButtonType.Reply, 0);

            AddHtmlLocalized(42, 268, 150, 20, 1150300, 0xFFFF, false, false); // CANCEL
            AddButton(8, 268, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (Arena.Stone != null && !User.InRange(Arena.Stone.Location, 15))
            {
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // host
                    SendGump(new CreateDuelGump(User, Arena));
                    break;
                case 2: // join
                    List<ArenaDuel> list = Arena.GetPendingPublic();

                    if (list != null && list.Count > 0)
                    {
                        if (list.Count < ArenaDuel.MaxEntries)
                        {
                            SendGump(new JoinDuelGump(User, list, Arena));
                        }
                        else
                        {
                            User.SendLocalizedMessage(1115954); // This session is already full.
                        }
                    }
                    else
                    {
                        User.SendLocalizedMessage(1115966); // There are no duel sessions available.
                    }
                    break;
                case 3: // see booked
                    SendGump(new BookedDuelsGump(User, Arena));
                    break;
                case 4: // check stats
                    SendGump(new IndividualStatsGump(User, Arena, User));
                    break;
                case 5: // arena rankings
                    SendGump(new ArenaRankingsGump(User, Arena));
                    break;
                case 6: // ignore invites
                    PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);
                    entry.IgnoreInvites = entry.IgnoreInvites ? false : true;

                    User.SendLocalizedMessage(entry.IgnoreInvites ? 1116210 : 1116211); // You are now ignoring invitations. / You are now accepting invitations.

                    Refresh();
                    break;
            }
        }
    }

    public class CreateDuelGump : BaseDuelGump
    {
        public CreateDuelGump(PlayerMobile pm, PVPArena arena)
            : base(pm, arena, null)
        {
            Duel = new ArenaDuel(arena, pm);
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 315, 410, 9200);

            AddImageTiled(8, 8, 299, 27, 2624);
            AddAlphaRegion(8, 8, 299, 27);

            AddImageTiled(8, 40, 200, 337, 2624);
            AddAlphaRegion(8, 40, 200, 337);

            AddImageTiled(210, 40, 97, 337, 2624);
            AddAlphaRegion(210, 40, 97, 337);

            AddImageTiled(8, 383, 299, 20, 2624);
            AddAlphaRegion(8, 383, 299, 20);

            AddHtmlLocalized(0, 12, 315, 20, CenterLoc, "#1115787", 0xFFFF, false, false); // Arena Menu - Rules

            AddButton(10, 47, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddButton(10, 72, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddButton(10, 97, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddButton(10, 122, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddButton(10, 147, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddButton(10, 172, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddButton(10, 197, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddButton(10, 222, 4005, 4007, 8, GumpButtonType.Reply, 0);
            AddButton(10, 247, 4005, 4007, 9, GumpButtonType.Reply, 0);
            AddButton(10, 272, 4005, 4007, 10, GumpButtonType.Reply, 0);
            AddButton(10, 297, 4005, 4007, 11, GumpButtonType.Reply, 0);
            AddButton(10, 322, 4005, 4007, 12, GumpButtonType.Reply, 0);
            AddButton(10, 347, 4005, 4007, 13, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 47, 170, 20, 1115780, 0xFFFF, false, false);  // Maximum Entries
            AddHtmlLocalized(45, 72, 170, 20, 1116146, 0xFFFF, false, false);  // Room Type
            AddHtmlLocalized(45, 97, 170, 20, 1115785, 0xFFFF, false, false);  // Arena
            AddHtmlLocalized(45, 122, 170, 20, 1115781, 0xFFFF, false, false); // Battle Mode
            AddHtmlLocalized(45, 147, 170, 20, 1150173, 0xFFFF, false, false); // Ranked
            AddHtmlLocalized(45, 172, 170, 20, 1115782, 0xFFFF, false, false); // Time Limit
            AddHtmlLocalized(45, 197, 170, 20, 1149605, 0xFFFF, false, false); // Entry Fee
            AddHtmlLocalized(45, 222, 170, 20, 1115783, 0xFFFF, false, false); // Pet Slots
            AddHtmlLocalized(45, 247, 170, 20, 1115784, 0xFFFF, false, false); // Riding/Flying
            AddHtmlLocalized(45, 272, 170, 20, 1115786, 0xFFFF, false, false); // Ranged Weapons
            AddHtmlLocalized(45, 297, 170, 20, 1149604, 0xFFFF, false, false); // Summoning Spells
            AddHtmlLocalized(45, 322, 170, 20, 1151915, 0xFFFF, false, false); // Field Spells
            AddHtmlLocalized(45, 347, 170, 20, 1151916, 0xFFFF, false, false); // Potion

            AddHtmlLocalized(220, 47, 170, 20, 1114057, Duel.Entries.ToString(), 0xFFFF, false, false);  // Maximum Entries
            AddHtmlLocalized(220, 72, 170, 20, Duel.RoomType == RoomType.Public ? 1116494 : 1116495, 0xFFFF, false, false);  // Room Type
            AddHtmlLocalized(220, 97, 170, 20, 1114057, Arena.Definition.Name, 0xFFFF, false, false);  // Arena
            AddHtmlLocalized(220, 122, 170, 20, Duel.BattleMode == BattleMode.Survival ? 1116492 : 1116493, 0xFFFF, false, false); // Battle Mode
            AddHtmlLocalized(220, 147, 170, 20, Duel.Ranked ? 1152994 : 1152995, 0xFFFF, false, false); // Ranked
            AddHtml(220, 172, 170, 20, Color("#FFFFFF", GetTimeLimit()), false, false); // Time Limit
            AddHtml(220, 197, 170, 20, Color("#FFFFFF", GetEntryFee()), false, false); // Entry Fee
            AddHtml(220, 222, 170, 20, Color("#FFFFFF", GetSlots()), false, false); // Pet Slots
            AddHtmlLocalized(220, 247, 170, 20, Duel.RidingFlyingAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Riding/Flying
            AddHtmlLocalized(220, 272, 170, 20, Duel.RangedWeaponsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Ranged Weapons
            AddHtmlLocalized(220, 297, 170, 20, Duel.SummonSpellsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Summoning Spells
            AddHtmlLocalized(220, 322, 170, 20, Duel.FieldSpellsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Field Spells
            AddHtml(220, 347, 170, 20, Color("#FFFFFF", GetPotion()), false, false); // Potion

            AddButton(8, 383, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 383, 100, 20, 1150154, 0xFFFF, false, false); // BACK

            AddButton(225, 383, 4023, 4025, 50, GumpButtonType.Reply, 0);
            AddHtml(260, 383, 60, 20, Color("#FFFFFF", "OK"), false, false);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                SendGump(new ArenaStoneGump(User, Arena));
            }
            else
            {
                switch (info.ButtonID)
                {
                    case 1:
                        Duel.Entries++;
                        if (Duel.Entries > ArenaDuel.MaxEntries)
                            Duel.Entries = 2;
                        Refresh();
                        break;
                    case 2:
                        Duel.RoomType = Duel.RoomType == RoomType.Public ? RoomType.Private : RoomType.Public;
                        Refresh();
                        break;
                    case 3:
                        Refresh();
                        break;
                    case 4:
                        Duel.BattleMode = Duel.BattleMode == BattleMode.Survival ? BattleMode.Team : BattleMode.Survival;
                        Refresh();
                        break;
                    case 5:
                        Duel.Ranked = !Duel.Ranked;
                        Refresh();
                        break;
                    case 6:
                        switch (Duel.TimeLimit)
                        {
                            case TimeLimit.FiveMinutes: Duel.TimeLimit = TimeLimit.TenMinutes; break;
                            case TimeLimit.TenMinutes: Duel.TimeLimit = TimeLimit.FifteenMinutes; break;
                            case TimeLimit.FifteenMinutes: Duel.TimeLimit = TimeLimit.TwentyMinutes; break;
                            case TimeLimit.TwentyMinutes: Duel.TimeLimit = TimeLimit.ThirtyMinutes; break;
                            case TimeLimit.ThirtyMinutes: Duel.TimeLimit = TimeLimit.FiveMinutes; break;
                        }
                        Refresh();
                        break;
                    case 7:
                        switch (Duel.EntryFee)
                        {
                            case EntryFee.Zero: Duel.EntryFee = EntryFee.OneThousand; break;
                            case EntryFee.OneThousand: Duel.EntryFee = EntryFee.FiveThousand; break;
                            case EntryFee.FiveThousand: Duel.EntryFee = EntryFee.TenThousand; break;
                            case EntryFee.TenThousand: Duel.EntryFee = EntryFee.TwentyFiveThousand; break;
                            case EntryFee.TwentyFiveThousand: Duel.EntryFee = EntryFee.FiftyThousand; break;
                            case EntryFee.FiftyThousand: Duel.EntryFee = EntryFee.Zero; break;
                        }
                        Refresh();
                        break;
                    case 8:
                        Duel.PetSlots++;
                        if (Duel.PetSlots > ArenaDuel.MaxPetSlots)
                            Duel.PetSlots = 0;
                        Refresh();
                        break;
                    case 9:
                        Duel.RidingFlyingAllowed = !Duel.RidingFlyingAllowed;
                        Refresh();
                        break;
                    case 10:
                        Duel.RangedWeaponsAllowed = !Duel.RangedWeaponsAllowed;
                        Refresh();
                        break;
                    case 11:
                        Duel.SummonSpellsAllowed = !Duel.SummonSpellsAllowed;
                        Refresh();
                        break;
                    case 12:
                        Duel.FieldSpellsAllowed = !Duel.FieldSpellsAllowed;
                        Refresh();
                        break;
                    case 13:
                        switch (Duel.PotionRules)
                        {
                            case PotionRules.All: Duel.PotionRules = PotionRules.None; break;
                            case PotionRules.None: Duel.PotionRules = PotionRules.NoHealing; break;
                            case PotionRules.NoHealing: Duel.PotionRules = PotionRules.All; break;
                        }
                        Refresh();
                        break;
                    case 50:
                        Arena.AddPendingDuel(Duel);
                        SendGump(new PendingDuelGump(User, Duel, Arena));
                        PVPArenaSystem.SendMessage(User, 1115800); // You have created a new duel session.

                        PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

                        if (entry != null)
                        {
                            entry.Profile = new PlayerStatsEntry.DuelProfile(Duel);
                        }

                        break;
                }
            }
        }
    }

    public class DuelRulesGump : BaseDuelGump
    {
        public DuelRulesGump(PlayerMobile pm, PVPArena arena, ArenaDuel duel)
            : base(pm, arena, duel)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 290, 410, 9200);

            AddImageTiled(8, 8, 274, 25, 2624);
            AddAlphaRegion(8, 8, 274, 25);

            AddImageTiled(8, 40, 175, 335, 2624);
            AddAlphaRegion(8, 40, 175, 335);

            AddImageTiled(186, 40, 97, 335, 2624);
            AddAlphaRegion(186, 40, 97, 335);

            AddImageTiled(8, 383, 274, 20, 2624);
            AddAlphaRegion(8, 383, 274, 20);

            AddHtmlLocalized(0, 10, 315, 20, CenterLoc, "#1115787", 0xFFFF, false, false); // Arena Menu - Rules

            AddHtmlLocalized(20, 47, 170, 20, 1115780, 0xFFFF, false, false);  // Maximum Entries
            AddHtmlLocalized(20, 72, 170, 20, 1116146, 0xFFFF, false, false);  // Room Type
            AddHtmlLocalized(20, 97, 170, 20, 1115785, 0xFFFF, false, false);  // Arena
            AddHtmlLocalized(20, 122, 170, 20, 1115781, 0xFFFF, false, false); // Battle Mode
            AddHtmlLocalized(20, 147, 170, 20, 1150173, 0xFFFF, false, false); // Ranked
            AddHtmlLocalized(20, 172, 170, 20, 1115782, 0xFFFF, false, false); // Time Limit
            AddHtmlLocalized(20, 197, 170, 20, 1149605, 0xFFFF, false, false); // Entry Fee
            AddHtmlLocalized(20, 222, 170, 20, 1115783, 0xFFFF, false, false); // Pet Slots
            AddHtmlLocalized(20, 247, 170, 20, 1115784, 0xFFFF, false, false); // Riding/Flying
            AddHtmlLocalized(20, 272, 170, 20, 1115786, 0xFFFF, false, false); // Ranged Weapons
            AddHtmlLocalized(20, 297, 170, 20, 1149604, 0xFFFF, false, false); // Summoning Spells
            AddHtmlLocalized(20, 322, 170, 20, 1151915, 0xFFFF, false, false); // Field Spells
            AddHtmlLocalized(20, 347, 170, 20, 1151916, 0xFFFF, false, false); // Potion

            AddHtmlLocalized(193, 47, 170, 20, 1114057, Duel.Entries.ToString(), 0xFFFF, false, false);  // Maximum Entries
            AddHtmlLocalized(193, 72, 170, 20, Duel.RoomType == RoomType.Public ? 1116494 : 1116495, 0xFFFF, false, false);  // Room Type
            AddHtmlLocalized(193, 97, 170, 20, 1114057, Arena.Definition.Name, 0xFFFF, false, false);  // Arena
            AddHtmlLocalized(193, 122, 170, 20, Duel.BattleMode == BattleMode.Survival ? 1116492 : 1116493, 0xFFFF, false, false); // Battle Mode
            AddHtmlLocalized(193, 147, 170, 20, Duel.Ranked ? 1152994 : 1152995, 0xFFFF, false, false); // Ranked
            AddHtml(193, 172, 170, 20, Color("#FFFFFF", GetTimeLimit()), false, false); // Time Limit
            AddHtml(193, 197, 170, 20, Color("#FFFFFF", GetEntryFee()), false, false); // Entry Fee
            AddHtml(193, 222, 170, 20, Color("#FFFFFF", GetSlots()), false, false); // Pet Slots
            AddHtmlLocalized(193, 247, 170, 20, Duel.RidingFlyingAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Riding/Flying
            AddHtmlLocalized(193, 272, 170, 20, Duel.RangedWeaponsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Ranged Weapons
            AddHtmlLocalized(193, 297, 170, 20, Duel.SummonSpellsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Summoning Spells
            AddHtmlLocalized(193, 322, 170, 20, Duel.FieldSpellsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Field Spells
            AddHtml(193, 347, 170, 20, Color("#FFFFFF", GetPotion()), false, false); // Potion

            AddButton(8, 383, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 383, 100, 20, 1150154, 0xFFFF, false, false); // BACK
        }

        public override void OnResponse(RelayInfo info)
        {
            SendGump(new PendingDuelGump(User, Duel, Arena));
        }
    }

    public class PendingDuelGump : BaseDuelGump
    {
        public List<PlayerMobile> Participants { get; private set; }

        public PendingDuelGump(PlayerMobile pm, ArenaDuel duel, PVPArena arena)
            : base(pm, arena, duel)
        {
            Participants = Duel.ParticipantList();
        }

        public override void AddGumpLayout()
        {
            int width = Duel.ParticipantCount > 5 ? 400 : 300;
            AddBackground(0, 0, width, 377, 9200);

            AddImageTiled(8, 8, width - 16, 25, 2624);
            AddAlphaRegion(8, 8, width - 16, 25);

            AddHtmlLocalized(0, 10, width, 20, CenterLoc, "#1115624", 0xFFFF, false, false); // Arena Menu - Queuing

            AddImageTiled(8, 40, width - 16, 133, 2624);
            AddAlphaRegion(8, 40, width - 16, 133);

            AddImageTiled(8, 180, width - 16, 25, 2624);
            AddAlphaRegion(8, 180, width - 16, 25);

            AddHtmlLocalized(0, 183, 300, 20, CenterLoc, Duel.BattleMode == BattleMode.Survival ? "#1115763" : "#1116392", 0xFFFF, false, false); // Participant list : Team: Order (Blue) / Chaos (Red)

            AddImageTiled(8, 212, width - 16, 130, 2624);
            AddAlphaRegion(8, 212, width - 16, 130);

            AddImageTiled(8, 350, width - 16, 20, 2624);
            AddAlphaRegion(8, 350, width - 16, 20);

            AddButton(10, 45, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddButton(10, 70, 4023, 4025, 2, GumpButtonType.Reply, 0);
            AddButton(10, 95, 4023, 4025, 3, GumpButtonType.Reply, 0);
            AddButton(10, 120, 4023, 4025, 4, GumpButtonType.Reply, 0);
            AddButton(10, 145, 4023, 4025, 5, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 45, 150, 20, 1115625, 0xFFFF, false, false); // Start the duel
            AddHtmlLocalized(45, 70, 150, 20, 1115626, 0xFFFF, false, false); // Cancel the duel
            AddHtmlLocalized(45, 95, 150, 20, 1116147, 0xFFFF, false, false); // Invite a duelist
            AddHtmlLocalized(45, 120, 150, 20, 1115925, 0xFFFF, false, false); // See booked duels
            AddHtmlLocalized(45, 145, 150, 20, 1153084, 0xFFFF, false, false); // See the rules

            int y = 215;

            if (Duel.BattleMode == BattleMode.Survival)
            {
                for (int i = 0; i < Participants.Count; i++)
                {
                    int x = 10;

                    if (i >= 5)
                        x += 200;

                    if (i == 5)
                        y = 215;

                    AddButton(x, y, 4011, 4013, 50 + i, GumpButtonType.Reply, 0);
                    x += 37;

                    if (User == Duel.Host)
                    {
                        AddButton(x, y, 4017, 4019, 100 + i, GumpButtonType.Reply, 0);
                        x += 37;
                    }

                    AddLabelCropped(x, y, 135, 20, LabelHue, Participants[i].Name);

                    y += 25;
                }
            }
            else
            {
                for (int i = 0; i < Participants.Count; i++)
                {
                    int x = 10;

                    if (i >= 5)
                        x += 200;

                    if (i == 5)
                        y = 215;

                    AddButton(x, y, 4011, 4013, 50 + i, GumpButtonType.Reply, 0);
                    x += 37;

                    if (User == Duel.Host)
                    {
                        AddButton(x, y, 4017, 4019, 100 + i, GumpButtonType.Reply, 0);
                        x += 37;
                        AddButton(x, y, 4026, 4029, 200 + i, GumpButtonType.Reply, 0);
                        x += 37;
                    }

                    AddLabelCropped(x, y, 135, 20, Duel.TeamOrder.Contains(Participants[i]) ? 487 : 37, Participants[i].Name);

                    y += 25;
                }
            }

            AddImage(width - 82, 70, 5587);

            AddButton(8, 349, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 349, 120, 20, 1150300, 0xFFFF, false, false); // CANCEL
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID > 0 && !Arena.PendingDuels.ContainsKey(Duel))
            {
                PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                return;
            }

            switch (info.ButtonID)
            {
                default:
                    if (info.ButtonID < 100)
                    {
                        int id = info.ButtonID - 50;

                        if (id >= 0 && id < Participants.Count)
                        {
                            Refresh();
                            SendGump(new IndividualStatsGump(User, Arena, Participants[id]));
                        }
                    }
                    else if (info.ButtonID < 200)
                    {
                        int id = info.ButtonID - 100;

                        if (id >= 0 && id < Participants.Count)
                        {
                            if (Participants[id] == Duel.Host)
                            {
                                User.SendLocalizedMessage(1115838); // You cannot ban yourself!
                                Refresh();
                            }
                            else
                            {
                                User.SendGump(new ConfirmCallbackGump(User, 1115834, 1115835, null, null, (from, state) =>
                                {
                                    if (!Arena.PendingDuels.ContainsKey(Duel))
                                    {
                                        PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                                    }
                                    else
                                    {
                                        Duel.RemovePlayer(User, true);
                                    }
                                }, confirmLoc: 1115836, closeLoc: 1115837));
                            }
                        }
                    }
                    else
                    {
                        int id = info.ButtonID - 200;

                        if (id >= 0 && id < Participants.Count)
                        {
                            if (Duel.SwapParticipant(Participants[id]))
                            {
                                PVPArenaSystem.SendMessage(Participants[id], 1116462); // The host player has moved you to another team.
                            }

                            Refresh();
                        }
                    }
                    break;
                case 1:
                    if (User != Duel.Host)
                    {
                        PVPArenaSystem.SendMessage(User, 1115949); // Only the session's host may use this function.
                    }
                    else if (Duel.ParticipantCount == 1)
                    {
                        PVPArenaSystem.SendMessage(User, 1115958); // To start a duel, you will need to gather more players/participants.
                    }
                    else
                    {
                        Arena.TryBeginDuel(Duel);
                    }
                    break;
                case 2:
                    User.SendGump(new ConfirmCallbackGump(User, 1115819, 1115820, null, null, (from, obj) =>
                        {
                            if (!Arena.PendingDuels.ContainsKey(Duel))
                            {
                                PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                            }
                            else
                            {
                                if (from == Duel.Host)
                                {
                                    Arena.RemovePendingDuel(Duel, true);
                                }
                                else
                                {
                                    Duel.RemovePlayer((PlayerMobile)from, false);
                                }
                            }
                        }, confirmLoc: 1115821, closeLoc: 1115822));
                    break;
                case 3:
                    if (User != Duel.Host)
                    {
                        PVPArenaSystem.SendMessage(User, 1115949); // Only the session's host may use this function.
                    }
                    else
                    {
                        User.Target = new InternalTarget(Arena, Duel);
                    }
                    break;
                case 4:
                    SendGump(new BookedDuelsGump(User, Arena));
                    break;
                case 5:
                    SendGump(new DuelRulesGump(User, Arena, Duel));
                    break;
            }
        }

        public static void RefreshAll(ArenaDuel duel)
        {
            foreach (KeyValuePair<PlayerMobile, PlayerStatsEntry> kvp in duel.GetParticipants())
            {
                PlayerMobile pm = kvp.Key;

                if (pm.HasGump(typeof(PendingDuelGump)))
                {
                    pm.CloseGump(typeof(PendingDuelGump));
                    SendGump(new PendingDuelGump(pm, duel, duel.Arena));
                }
            }
        }

        private class InternalTarget : Targeting.Target
        {
            public PVPArena Arena { get; private set; }
            public ArenaDuel Duel { get; private set; }

            public InternalTarget(PVPArena arena, ArenaDuel duel)
                : base(10, false, Targeting.TargetFlags.None)
            {
                Arena = arena;
                Duel = duel;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!Arena.PendingDuels.ContainsKey(Duel))
                {
                    PVPArenaSystem.SendMessage(from, 1115957); // This session has expired. Please create a new session and try again.
                }
                else if (targeted is PlayerMobile)
                {
                    PlayerMobile pm = targeted as PlayerMobile;
                    PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(pm);

                    if (Duel.ParticipantCount >= Duel.Entries)
                    {
                        // TODO: Message?
                    }
                    else if (!pm.Alive)
                    {
                        PVPArenaSystem.SendMessage(from, 1116148); // You cannot invite dead players.
                    }
                    else if (pm.Young)
                    {
                        PVPArenaSystem.SendMessage(from, 1116154); // You cannot invite young players.
                    }
                    else if (entry.IgnoreInvites)
                    {
                        PVPArenaSystem.SendMessage(from, 1116157); // The player is not accepting invitations.
                    }
                    else if (Duel.IsParticipant(pm))
                    {
                        PVPArenaSystem.SendMessage(from, 1116150); // The player has already joined in the session.
                    }
                    else if (Duel.IsBanned(pm))
                    {
                        PVPArenaSystem.SendMessage(from, 1116151); // You have already banned the player once.
                    }
                    else if (!pm.InRange(Arena.Stone.Location, 12))
                    {
                        PVPArenaSystem.SendMessage(from, 1116149); // The targeted player is too far away from the arena stone.
                    }
                    else if (PVPArenaSystem.BlockSameIP && PVPArenaSystem.IsSameIP(from, pm))
                    {
                        PVPArenaSystem.SendMessage(from, 1116150); // The player has already joined in the session.
                    }
                    else if (from != pm)
                    {
                        PVPArenaSystem.SendMessage(from, 1116152); // You have sent the invitation to the player.
                        PVPArenaSystem.SendMessage(pm, 1116212); // You have been invited to a duel.  Select the “OK” button to join this duel.

                        SendGump(new OfferDuelGump(pm, Duel, Arena, true));
                    }
                }
                else
                {
                    from.SendLocalizedMessage(503348); // You can only do this to players.
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (from is PlayerMobile)
                {
                    SendGump(new PendingDuelGump((PlayerMobile)from, Duel, Arena));
                }
            }
        }
    }

    public class JoinDuelGump : BaseArenaGump
    {
        public List<ArenaDuel> PendingDuels { get; private set; }

        public JoinDuelGump(PlayerMobile pm, List<ArenaDuel> list, PVPArena arena)
            : base(pm, arena)
        {
            PendingDuels = list;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 310, 365, 9200);
            AddPage(0);

            AddImageTiled(8, 8, 294, 25, 2624);
            AddAlphaRegion(8, 8, 294, 25);

            AddImageTiled(8, 40, 85, 27, 2624);
            AddAlphaRegion(8, 40, 85, 27);

            AddImageTiled(95, 40, 207, 27, 2624);
            AddAlphaRegion(95, 40, 207, 27);

            AddImageTiled(8, 70, 37, 260, 2624);
            AddAlphaRegion(8, 70, 37, 260);

            AddImageTiled(48, 70, 45, 260, 2624);
            AddAlphaRegion(48, 70, 45, 260);

            AddImageTiled(95, 70, 205, 260, 2624);
            AddAlphaRegion(95, 70, 205, 260);

            AddImageTiled(8, 337, 294, 20, 2624);
            AddAlphaRegion(8, 337, 294, 20);

            AddHtmlLocalized(0, 11, 310, 20, CenterLoc, "#1115842", 0xFFFF, false, false); // Arena Menu - Entry
            AddHtmlLocalized(8, 42, 310, 20, CenterLoc, "#1115847", 0xFFFF, false, false); // Entry
            AddHtmlLocalized(10, 42, 310, 20, 1115848, 0xFFFF, false, false); // Host Player

            int perPage = 8;
            int page = 1;
            int y = 70;

            AddPage(page);

            for (int i = 0; i < PendingDuels.Count; i++)
            {
                ArenaDuel duel = PendingDuels[i];

                if (!PVPArenaSystem.BlockSameIP || !PVPArenaSystem.HasSameIP(User, duel))
                {
                    AddButton(10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
                }

                AddLabel(54, y, LabelHue, string.Format("{0}/{1}", duel.ParticipantCount.ToString(), duel.Entries.ToString()));
                AddLabel(103, y, LabelHue, duel.Host.Name);

                if (i != 0 && i % perPage == 0)
                {
                    page++;

                    if (i < PendingDuels.Count - 1)
                    {
                        AddButton(272, 337, 4005, 4007, 0, GumpButtonType.Page, i + 1);
                        AddPage(page);
                    }

                    if (page > 1)
                    {
                        AddButton(240, 337, 4005, 4007, 0, GumpButtonType.Page, i - 1);
                    }

                    y = 44;
                }
            }

            AddButton(8, 337, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 337, 100, 20, 1150154, 0xFFFF, false, false); // BACK
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                SendGump(new ArenaStoneGump(User, Arena));
            }
            else
            {
                int id = info.ButtonID - 1;

                if (id >= 0 && id < PendingDuels.Count)
                {
                    ArenaDuel duel = PendingDuels[id];

                    if (!Arena.PendingDuels.ContainsKey(duel))
                    {
                        PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                    }
                    else
                    {
                        SendGump(new OfferDuelGump(User, duel, Arena, false));
                    }
                }
            }
        }
    }

    public class OfferDuelGump : BaseDuelGump
    {
        public bool FromPlayer { get; private set; }
        public bool DetailsOnly { get; private set; }

        public OfferDuelGump(PlayerMobile pm, ArenaDuel duel, PVPArena arena, bool fromPlayer, bool detailsOnly = false)
            : base(pm, arena, duel)
        {
            FromPlayer = fromPlayer;
            DetailsOnly = detailsOnly;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 500, 460, 9200);

            AddImageTiled(8, 8, 484, 25, 2624);
            AddAlphaRegion(8, 8, 484, 25);

            AddImageTiled(8, 38, 198, 25, 2624);
            AddAlphaRegion(8, 38, 198, 25);

            AddImageTiled(214, 38, 278, 25, 2624);
            AddAlphaRegion(214, 38, 278, 25);

            AddImageTiled(8, 70, 199, DetailsOnly ? 330 : 355, 2624);
            AddAlphaRegion(8, 70, 199, DetailsOnly ? 330 : 355);

            AddImageTiled(214, 70, 170, DetailsOnly ? 330 : 355, 2624);
            AddAlphaRegion(214, 70, 170, DetailsOnly ? 330 : 355);

            AddImageTiled(386, 70, 107, DetailsOnly ? 330 : 355, 2624);
            AddAlphaRegion(386, 70, 107, DetailsOnly ? 330 : 355);

            AddImageTiled(8, DetailsOnly ? 408 : 433, 484, DetailsOnly ? 45 : 20, 2624);
            AddAlphaRegion(8, DetailsOnly ? 408 : 433, 484, DetailsOnly ? 45 : 20);

            AddHtmlLocalized(0, 10, 500, 20, CenterLoc, "#1115872", 0xFFFF, false, false); // Arena Menu - Details
            AddHtmlLocalized(8, 40, 200, 20, CenterLoc, "#1115763", 0xFFFF, false, false); // Participant List
            AddHtmlLocalized(214, 40, 278, 20, CenterLoc, "#1115818", 0xFFFF, false, false); // Rules

            List<PlayerMobile> partList = Duel.ParticipantList();

            for (int i = 0; i < partList.Count; i++)
            {
                int hue = LabelHue;

                if (Duel.BattleMode == BattleMode.Team)
                {
                    hue = Duel.TeamOrder.Contains(partList[i]) ? 487 : 37;
                }

                AddLabel(17, 80 + (i * 20), hue, partList[i].Name);
            }

            AddHtmlLocalized(220, 80, 170, 20, 1115780, 0xFFFF, false, false);  // Maximum Entries
            AddHtmlLocalized(220, 105, 170, 20, 1116146, 0xFFFF, false, false);  // Room Type
            AddHtmlLocalized(220, 130, 170, 20, 1115785, 0xFFFF, false, false);  // Arena
            AddHtmlLocalized(220, 155, 170, 20, 1115781, 0xFFFF, false, false); // Battle Mode
            AddHtmlLocalized(220, 180, 170, 20, 1150173, 0xFFFF, false, false); // Ranked
            AddHtmlLocalized(220, 205, 170, 20, 1115782, 0xFFFF, false, false); // Time Limit
            AddHtmlLocalized(220, 230, 170, 20, 1149605, 0xFFFF, false, false); // Entry Fee
            AddHtmlLocalized(220, 255, 170, 20, 1115783, 0xFFFF, false, false); // Pet Slots
            AddHtmlLocalized(220, 280, 170, 20, 1115784, 0xFFFF, false, false); // Riding/Flying
            AddHtmlLocalized(220, 305, 170, 20, 1115786, 0xFFFF, false, false); // Ranged Weapons
            AddHtmlLocalized(220, 330, 170, 20, 1149604, 0xFFFF, false, false); // Summoning Spells
            AddHtmlLocalized(220, 355, 170, 20, 1151915, 0xFFFF, false, false); // Field Spells
            AddHtmlLocalized(220, 380, 170, 20, 1151916, 0xFFFF, false, false); // Potion

            AddHtmlLocalized(395, 80, 170, 20, 1114057, Duel.Entries.ToString(), 0xFFFF, false, false);  // Maximum Entries
            AddHtmlLocalized(395, 105, 170, 20, Duel.RoomType == RoomType.Public ? 1116494 : 1116495, 0xFFFF, false, false);  // Room Type
            AddHtmlLocalized(395, 130, 170, 20, 1114057, Arena.Definition.Name, 0xFFFF, false, false);  // Arena
            AddHtmlLocalized(395, 155, 170, 20, Duel.BattleMode == BattleMode.Survival ? 1116492 : 1116493, 0xFFFF, false, false); // Battle Mode
            AddHtmlLocalized(395, 180, 170, 20, Duel.Ranked ? 1152994 : 1152995, 0xFFFF, false, false); // Ranked
            AddHtml(395, 205, 170, 20, Color("#FFFFFF", GetTimeLimit()), false, false); // Time Limit
            AddHtml(395, 230, 170, 20, Color("#FFFFFF", GetEntryFee()), false, false); // Entry Fee
            AddHtml(395, 255, 170, 20, Color("#FFFFFF", GetSlots()), false, false); // Pet Slots
            AddHtmlLocalized(395, 280, 170, 20, Duel.RidingFlyingAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Riding/Flying
            AddHtmlLocalized(395, 305, 170, 20, Duel.RangedWeaponsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Ranged Weapons
            AddHtmlLocalized(395, 330, 170, 20, Duel.SummonSpellsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Summoning Spells
            AddHtmlLocalized(395, 355, 170, 20, Duel.FieldSpellsAllowed ? 1152994 : 1152995, 0xFFFF, false, false); // Field Spells
            AddHtml(395, 380, 170, 20, Color("#FFFFFF", GetPotion()), false, false); // Potion

            if (DetailsOnly)
            {
                AddHtmlLocalized(20, 412, 460, 40, CenterLoc, "#1115874", C32216(0xFF6347), false, false);
                // The arena gate has opened near the arena stone. <br>You've ninety seconds to use the gate or you'll be removed from this duel.
            }
            else
            {
                AddButton(8, 433, 4014, 4016, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 433, 100, 20, FromPlayer ? 1150300 : 1150154, 0xFFFF, false, false); // CANCEL : BACK

                AddButton(407, 433, 4023, 4025, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(442, 433, 60, 20, 1115873, 0xFFFF, false, false); // ENTRY
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    if (!FromPlayer)
                    {
                        List<ArenaDuel> list = Arena.GetPendingPublic();

                        if (list != null && list.Count > 0)
                        {
                            SendGump(new JoinDuelGump(User, list, Arena));
                        }
                    }
                    break;
                case 1:
                    if (Duel.TryAddPlayer(User))
                    {
                        PendingDuelGump.RefreshAll(Duel);
                        SendGump(new PendingDuelGump(User, Duel, Arena));
                    }
                    break;
            }
        }
    }

    public class BookedDuelsGump : BaseArenaGump
    {
        public List<ArenaDuel> BookedDuels { get; set; }

        public BookedDuelsGump(PlayerMobile pm, PVPArena arena)
            : base(pm, arena)
        {
            BookedDuels = PVPArenaSystem.Instance.GetBookedDuels();
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 310, 370, 9200);
            AddPage(0);

            AddImageTiled(8, 8, 294, 25, 2624);
            AddAlphaRegion(8, 8, 294, 25);

            AddImageTiled(8, 38, 85, 27, 2624);
            AddAlphaRegion(8, 38, 85, 27);

            AddImageTiled(96, 38, 207, 27, 2624);
            AddAlphaRegion(96, 38, 207, 27);

            AddImageTiled(8, 70, 85, 268, 2624);
            AddAlphaRegion(8, 70, 85, 268);

            AddImageTiled(96, 70, 207, 268, 2624);
            AddAlphaRegion(96, 70, 207, 268);

            AddImageTiled(8, 343, 294, 20, 2624);
            AddAlphaRegion(8, 343, 294, 20);

            AddHtmlLocalized(0, 12, 310, 20, CenterLoc, "#1115926", 0xFFFF, false, false); // Arena Menu - Booking Status
            AddHtmlLocalized(0, 42, 93, 20, CenterLoc, "#1115785", 0xFFFF, false, false); // Arena
            AddHtmlLocalized(96, 42, 207, 20, CenterLoc, "#1115848", 0xFFFF, false, false); // Host Player

            int y = 80;
            int page = 1;
            int perPage = 10;

            AddPage(1);

            for (int i = 0; i < BookedDuels.Count; i++)
            {
                ArenaDuel duel = BookedDuels[i];

                AddLabel(10, y, LabelHue, duel.Arena.Definition.Name);
                AddLabel(100, y, LabelHue, duel.Host.Name);

                y += 20;

                if (i != 0 && i % perPage == 0)
                {
                    y = 80;

                    if (i < BookedDuels.Count - 1)
                        AddButton(273, 343, 4005, 4007, 0, GumpButtonType.Page, page + 1);

                    page++;
                    AddPage(page);

                    AddButton(240, 343, 4014, 4015, 0, GumpButtonType.Page, page - 1);
                }
            }

            AddButton(8, 343, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 343, 100, 20, 1150154, 0xFFFF, false, false); // BACK
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                ArenaDuel duel = Arena.GetPendingDuel(User);

                if (duel == null)
                {
                    SendGump(new ArenaStoneGump(User, Arena));
                }
                else
                {
                    SendGump(new PendingDuelGump(User, duel, Arena));
                }
            }
        }
    }

    public class IndividualStatsGump : BaseArenaGump
    {
        public PlayerMobile WhosStats { get; private set; }

        public IndividualStatsGump(PlayerMobile pm, PVPArena arena, PlayerMobile stats)
            : base(pm, arena)
        {
            WhosStats = stats;
        }

        public override void AddGumpLayout()
        {
            PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(WhosStats);
            PlayerStatsEntry userEntry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

            AddBackground(0, 0, 580, 440, 9200);

            AddImageTiled(8, 8, 564, 48, 2624);
            AddAlphaRegion(8, 8, 564, 48);

            AddImageTiled(8, 62, 217, 28, 2624);
            AddAlphaRegion(8, 62, 217, 28);

            AddImageTiled(230, 62, 342, 28, 2624);
            AddAlphaRegion(230, 62, 342, 28);

            AddImageTiled(8, 95, 110, 80, 2624);
            AddAlphaRegion(8, 95, 110, 80);

            AddImageTiled(120, 95, 105, 80, 2624);
            AddAlphaRegion(120, 95, 105, 80);

            AddImageTiled(8, 180, 217, 25, 2624);
            AddAlphaRegion(8, 180, 217, 25);

            AddImageTiled(8, 210, 110, 80, 2624);
            AddAlphaRegion(8, 210, 110, 80);

            AddImageTiled(120, 210, 105, 80, 2624);
            AddAlphaRegion(120, 210, 105, 80);

            AddImageTiled(8, 295, 217, 25, 2624);
            AddAlphaRegion(8, 295, 217, 25);

            AddImageTiled(8, 325, 110, 82, 2624);
            AddAlphaRegion(8, 325, 110, 80);

            AddImageTiled(120, 325, 105, 82, 2624);
            AddAlphaRegion(120, 325, 105, 80);

            AddImageTiled(230, 95, 95, 312, 2624);
            AddAlphaRegion(230, 95, 95, 312);

            AddImageTiled(330, 95, 85, 312, 2624);
            AddAlphaRegion(330, 95, 85, 312);

            AddImageTiled(420, 95, 152, 312, 2624);
            AddAlphaRegion(420, 95, 152, 312);

            AddImageTiled(8, 412, 564, 20, 2624);
            AddAlphaRegion(8, 412, 564, 20);

            int titleIndex = WhosStats.SelectedTitle;
            object title = titleIndex >= 0 && titleIndex < WhosStats.RewardTitles.Count ? WhosStats.RewardTitles[titleIndex] : null;
            string rewardTitle = "None";

            if (title is int)
                rewardTitle = string.Format("#{0}", (int)title);
            else if (title is string)
                rewardTitle = (string)title;

            AddHtmlLocalized(0, 12, 580, 20, CenterLoc, "#1115976", 0xFFFF, false, false); // <CENTER>Arena Menu - Stats</CENTER>
            AddHtmlLocalized(0, 32, 580, 20, 1149602, string.Format("{0}\t{1}", WhosStats.Name, rewardTitle), 0xFFFF, false, false); // <CENTER>Arena Menu - Stats</CENTER>

            AddHtmlLocalized(8, 66, 222, 20, CenterLoc, "#1115983", 0xFFFF, false, false); // Stats - Survival
            AddHtmlLocalized(15, 100, 100, 20, 1115977, 0xFFFF, false, false); // Wins
            AddHtmlLocalized(15, 125, 100, 20, 1115978, 0xFFFF, false, false); // Losses
            AddHtmlLocalized(15, 150, 100, 20, 1115979, 0xFFFF, false, false); // Draws

            AddHtmlLocalized(8, 182, 222, 20, CenterLoc, "#1116485", 0xFFFF, false, false); // Stats - Team Battle
            AddHtmlLocalized(15, 215, 100, 20, 1115977, 0xFFFF, false, false); // Wins
            AddHtmlLocalized(15, 240, 100, 20, 1115978, 0xFFFF, false, false); // Losses
            AddHtmlLocalized(15, 265, 100, 20, 1115979, 0xFFFF, false, false); // Draws

            AddHtmlLocalized(15, 298, 222, 20, CenterLoc, "#1116486", 0xFFFF, false, false); // Stats - Others
            AddHtmlLocalized(15, 330, 100, 20, 1115980, 0xFFFF, false, false); // Latest Stats
            AddHtmlLocalized(15, 355, 100, 20, 1115981, 0xFFFF, false, false); // Kills
            AddHtmlLocalized(15, 380, 100, 20, 1115982, 0xFFFF, false, false); // Deaths

            AddHtmlLocalized(229, 66, 344, 20, CenterLoc, "#1115984", 0xFFFF, false, false); // Kill/Death Stats

            if (entry == null)
                return;

            AddLabel(128, 100, LabelHue, entry.SurvivalWins.ToString());
            AddLabel(128, 125, LabelHue, entry.SurvivalLosses.ToString());
            AddLabel(128, 150, LabelHue, entry.SurvivalDraws.ToString());

            AddLabel(128, 215, LabelHue, entry.TeamWins.ToString());
            AddLabel(128, 240, LabelHue, entry.TeamLosses.ToString());
            AddLabel(128, 265, LabelHue, entry.TeamDraws.ToString());

            AddLabel(128, 355, LabelHue, entry.Kills.ToString());
            AddLabel(128, 380, LabelHue, entry.Deaths.ToString());

            string latest = "";
            int y = 103;

            for (int i = entry.Record.Count - 1; i >= 0; i--)
            {
                PlayerStatsEntry.DuelRecord record = entry.Record[i];

                if (i > entry.Record.Count - 6)
                {
                    latest += string.Format("{0}-", record.KilledBy ? "L" : "W");
                }

                AddLabel(237, y, LabelHue, record.DuelDate.ToShortDateString());
                AddHtmlLocalized(340, y, 80, 20, record.KilledBy ? 1115987 : 1115986, 0xFFFF, false, false); // KILLED BY : KILLED
                AddLabel(429, y, LabelHue, record.Opponent == null ? "Unknown" : record.Opponent.Name);

                y += 25;
            }

            AddLabel(128, 330, LabelHue, latest);

            AddButton(8, 412, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 412, 100, 20, 1150154, 0xFFFF, false, false); // BACK

            AddButton(408, 412, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(442, 412, 200, 20, userEntry.OpenStats ? 1149594 : 1149595, 0xFFFF, false, false); // OPEN STATS (ON) / OPEN STATS (OFF)
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                ArenaDuel duel = Arena.GetPendingDuel(User);

                if (duel == null)
                {
                    SendGump(new ArenaStoneGump(User, Arena));
                }
                else
                {
                    SendGump(new PendingDuelGump(User, duel, Arena));
                }
            }
            else if (info.ButtonID == 1)
            {
                PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

                if (entry.OpenStats)
                {
                    User.SendLocalizedMessage(1149598); // You set your stats to the closed mode. Only you can see your stats.
                    entry.OpenStats = false;
                }
                else
                {
                    User.SendLocalizedMessage(1149597); // You set your stats to the open mode. Other players can see your stats from now on.
                    entry.OpenStats = true;
                }

                Refresh();
            }
        }
    }

    public class ArenaRankingsGump : BaseArenaGump
    {
        public int Page { get; private set; }
        public BattleMode ViewType { get; private set; }

        public List<ArenaStats> Stats { get; private set; }

        public ArenaRankingsGump(PlayerMobile pm, PVPArena arena, int page = 0)
            : base(pm, arena)
        {
            Page = page;
            ViewType = BattleMode.Team;

            Stats = Arena.TeamRankings;
        }

        public override void AddGumpLayout()
        {
            PlayerStatsEntry entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

            AddBackground(0, 0, 500, 395, 9200);

            AddImageTiled(8, 8, 484, 27, 2624);
            AddAlphaRegion(8, 8, 484, 27);

            AddImageTiled(8, 42, 484, 20, 2624);
            AddAlphaRegion(8, 42, 484, 20);

            AddImageTiled(8, 69, 484, 292, 2624);
            AddAlphaRegion(8, 69, 484, 292);

            AddImageTiled(8, 367, 484, 20, 2624);
            AddAlphaRegion(8, 367, 484, 20);

            AddHtmlLocalized(0, 12, 500, 20, CenterLoc, ViewType == BattleMode.Team ? "#1150130" : "#1150131", 0xFFFF, false, false); // Arena Menu - Survival Rankings : Arena Menu - Team Rankings

            AddButton(8, 42, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 42, 100, 20, 1150159, 0xFFFF, false, false); // TOP RANKING

            AddButton(143, 42, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(178, 42, 100, 20, 1150160, 0xFFFF, false, false); // MY RANKING

            AddButton(279, 42, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(313, 42, 150, 20, ViewType == BattleMode.Team ? 1150133 : 1150132, 0xFFFF, false, false); // VIEW TEAM RANKINGS

            AddHtmlLocalized(10, 70, 50, 20, 1150170, 0xFFFF, false, false); // RANK
            AddHtmlLocalized(60, 70, 50, 20, 1150171, 0xFFFF, false, false); // NAME
            AddHtmlLocalized(435, 70, 75, 20, 1150172, 0xFFFF, false, false); // RATING

            int index = Page * 10;
            int pageIndex = 0;

            for (int i = index; i < Stats.Count && pageIndex < 10; i++)
            {
                ArenaStats stats = Stats[i];
                int hue = stats.Owner == User ? 0x488 : LabelHue;

                AddLabel(10, 98 + (pageIndex * 25), hue, (index + pageIndex + 1).ToString());
                AddLabel(60, 98 + (pageIndex * 25), hue, stats.Owner.Name);
                AddLabel(435, 98 + (pageIndex * 25), hue, stats.Ranking.ToString());

                pageIndex++;
            }

            if (Page < (Stats.Count / 10) - 1)
            {
                AddButton(462, 367, 4005, 4007, 4, GumpButtonType.Reply, 0);
            }

            if (Page > 0)
            {
                AddButton(428, 367, 4014, 4016, 5, GumpButtonType.Reply, 0);
            }

            AddButton(8, 367, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 367, 100, 20, 1150154, 0xFFFF, false, false); // BACK
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                ArenaDuel duel = Arena.GetPendingDuel(User);

                if (duel == null)
                {
                    SendGump(new ArenaStoneGump(User, Arena));
                }
                else
                {
                    SendGump(new PendingDuelGump(User, duel, Arena));
                }
            }
            else
            {
                switch (info.ButtonID)
                {
                    case 1:
                        Stats = ViewType == BattleMode.Team ? Arena.TeamRankings : Arena.SurvivalRankings;
                        Refresh();
                        break;
                    case 2:
                        ArenaStats stats = Stats.FirstOrDefault(s => s.Owner == User);

                        if (stats != null)
                        {
                            int index = Stats.IndexOf(stats);
                            Page = index / 10;
                        }
                        Refresh();
                        break;
                    case 3:
                        if (ViewType == BattleMode.Survival)
                        {
                            Stats = Arena.TeamRankings;
                            ViewType = BattleMode.Team;
                        }
                        else
                        {
                            Stats = Arena.SurvivalRankings;
                            ViewType = BattleMode.Survival;
                        }
                        Refresh();
                        break;
                    case 4:
                        Page++;
                        Refresh();
                        break;
                    case 5:
                        Page--;
                        Refresh();
                        break;
                }
            }
        }
    }

    public class DuelResultsGump : BaseDuelGump
    {
        public ArenaTeam Winners { get; private set; }

        public DuelResultsGump(PlayerMobile pm, ArenaDuel duel, ArenaTeam winners)
            : base(pm, duel.Arena, duel)
        {
            Winners = winners;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 371, 370, 9200);

            AddImageTiled(8, 8, 355, 25, 2624);
            AddAlphaRegion(8, 8, 355, 25);

            AddImageTiled(8, 38, 355, 44, 2624);
            AddAlphaRegion(8, 38, 355, 44);

            AddImageTiled(8, 90, 139, 246, 2624);
            AddAlphaRegion(8, 90, 139, 246);

            AddImageTiled(150, 90, 69, 246, 2624);
            AddAlphaRegion(150, 90, 69, 246);

            AddImageTiled(222, 90, 139, 246, 2624);
            AddAlphaRegion(222, 90, 139, 246);

            AddImageTiled(8, 343, 355, 20, 2624);
            AddAlphaRegion(8, 343, 355, 20);

            string winner = "#1116483"; // DRAW

            if (Winners != null)
            {
                if (Duel.BattleMode == BattleMode.Team)
                {
                    if (Winners == Duel.TeamOrder)
                        winner = "#1116479"; // Order Team (Blue)
                    else if (Winners == Duel.TeamChaos)
                        winner = "#1116480"; // Chaos Team (Red)
                }
                else
                {
                    PlayerMobile pm = Winners.PlayerZero;
                    winner = pm == null ? "Someone" : pm.Name;
                }
            }

            AddHtmlLocalized(0, 10, 371, 20, CenterLoc, "#1115990", 0xFFFF, false, false); // Arena Menu - Results
            AddHtmlLocalized(100, winner == "#1116483" ? 75 : 50, 371, 20, 1153207, winner, 0xFFFF, false, false); // Winner: ~1_VAL~

            int i = 0;

            foreach (KeyValuePair<string, string> kvp in Duel.KillRecord)
            {
                AddLabel(15, 100 + (i * 25), LabelHue, kvp.Value);
                AddHtmlLocalized(158, 100 + (i * 25), 100, 20, 1115986, 0xFFFF, false, false); // KILLED
                AddLabel(231, 100 + (i * 25), LabelHue, kvp.Key);

                i++;

                if (i >= 10)
                    break;
            }

            AddHtmlLocalized(42, 343, 150, 20, 1150300, 0xFFFF, false, false); // CANCEL
            AddButton(8, 343, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }
    }
}
