using Server;
using System;
using Server.Items;
using Server.Gumps;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.ArenaSystem
{
    public class ArenaStoneGump : BaseGump
    {
        public PVPArena Arena { get; private set; }

        public ArenaStoneGump(PlayerMobile pm, PVPArena arena)
            : base(pm)
        {
            Arena = arena;
            CloseArenaGumps(pm);
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 300, 295, 9200);

            AddImageTiled(8, 8, 284, 25, 2624);
            AddAlphaRegion(8, 8, 284, 25);

            AddImageTiled(8, 43, 284, 45, 2624);
            AddAlphaRegion(8, 43, 284, 45);

            AddImageTiled(8, 98, 284, 160, 2624);
            AddAlphaRegion(8, 98, 284, 160);

            AddImageTiled(8, 268, 284, 20, 2624);
            AddAlphaRegion(8, 268, 284, 20);

            AddHtmlLocalized(0, 10, 300, 20, CenterLoc, "#1115619", 0xFFFF, false, false); // Arena Menu - Main
            AddHtmlLocalized(12, 45, 274, 40, 1115620, 0xFFFF, false, false); // You can host/join a duel or check your duel stats.

            var entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

            AddHtmlLocalized(45, 105, 200, 20, 1115621, 0xFFFF, false, false); // Host a duel
            AddHtmlLocalized(45, 130, 200, 20, 1115622, 0xFFFF, false, false); // Join a duel
            AddHtmlLocalized(45, 155, 200, 20, 1115925, 0xFFFF, false, false); // See booked duels
            AddHtmlLocalized(45, 180, 200, 20, 1115623, 0xFFFF, false, false); // Check your stats
            AddHtmlLocalized(45, 205, 200, 20, 1150128, 0xFFFF, false, false); // Check arena rankings
            AddHtmlLocalized(45, 230, 200, 20, entry.IgnoreInvites ? 1116155 : 1116156, 0xFFFF, false, false); // Ignore duel invite (ON) / Ignore duel invite (OFF)

            AddButton(10, 105, 4023, 4025, 1, GumpButtonType.Reply, 0);
            AddButton(10, 130, 4023, 4025, 2, GumpButtonType.Reply, 0);
            AddButton(10, 155, 4023, 4025, 3, GumpButtonType.Reply, 0);
            AddButton(10, 180, 4023, 4025, 4, GumpButtonType.Reply, 0);
            AddButton(10, 205, 4023, 4025, 5, GumpButtonType.Reply, 0);
            AddButton(10, 230, 4023, 4025, 6, GumpButtonType.Reply, 0);

            AddHtmlLocalized(42, 268, 150, 20, 1150300, 0xFFFF, false, false); // CANCEL
            AddButton(8, 268, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1: // host
                    BaseGump.SendGump(new CreateDuelGump(User, Arena));
                    break;
                case 2: // join
                    List<ArenaDuel> list = Arena.GetPendingPublic();

                    if (list != null && list.Count > 0)
                    {
                        BaseGump.SendGump(new JoinDuelGump(User, list, Arena));
                    }
                    else
                    {
                        User.SendLocalizedMessage(1115966); // There are no duel sessions available.
                    }
                    break;
                case 3: // see booked
                    BaseGump.SendGump(new BookedDuelsGump(User, Arena));
                    break;
                case 4: // check stats
                    BaseGump.SendGump(new IndividualStatsGump(User));
                    break;
                case 5: // arena rankings
                    BaseGump.SendGump(new ArenaRankingsGump(User, Arena));
                    break;
                case 6: // ignore invites
                    var entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);
                    entry.IgnoreInvites = entry.IgnoreInvites ? false : true;

                    User.SendLocalizedMessage(entry.IgnoreInvites ? 1116210 : 1116211); // You are now ignoring invitations. / You are now accepting invitations.

                    Refresh();
                    break;
            }
        }

        public static void CloseArenaGumps(Mobile m)
        {
            m.CloseGump(typeof(CreateDuelGump));
            m.CloseGump(typeof(ArenaStoneGump));
            m.CloseGump(typeof(PendingDuelGump));
            m.CloseGump(typeof(CreateDuelGump));
            m.CloseGump(typeof(JoinDuelGump));
            m.CloseGump(typeof(BookedDuelsGump));
            m.CloseGump(typeof(IndividualStatsGump));
            m.CloseGump(typeof(DuelResultsGump));
            m.CloseGump(typeof(ArenaRankingsGump));
            m.CloseGump(typeof(OfferDuelGump));
        }
    }

    public class CreateDuelGump : BaseGump
    {
        public PVPArena Arena { get; private set; }
        public ArenaDuel Duel { get; private set; }

        public CreateDuelGump(PlayerMobile pm, PVPArena arena)
            : base(pm, 50, 50)
        {
            Arena = arena;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 315, 410, 9200);

            AddImageTiled(8, 8, 299, 25, 2624);
            AddAlphaRegion(8, 8, 299, 25);

            AddImageTiled(8, 40, 200, 335, 2624);
            AddAlphaRegion(8, 40, 200, 335);

            AddImageTiled(210, 40, 97, 335, 2624);
            AddAlphaRegion(210, 40, 97, 335);

            AddImageTiled(8, 383, 299, 20, 2624);
            AddAlphaRegion(8, 383, 299, 20);

            AddHtmlLocalized(0, 10, 315, 20, CenterLoc, "#1115787", 0xFFFF, false, false); // Arena Menu - Rules

            AddButton(10, 47, 4007, 4009, 1, GumpButtonType.Reply, 0);
            AddButton(10, 72, 4007, 4009, 2, GumpButtonType.Reply, 0);
            AddButton(10, 97, 4007, 4009, 3, GumpButtonType.Reply, 0);
            AddButton(10, 122, 4007, 4009, 4, GumpButtonType.Reply, 0);
            AddButton(10, 147, 4007, 4009, 5, GumpButtonType.Reply, 0);
            AddButton(10, 172, 4007, 4009, 6, GumpButtonType.Reply, 0);
            AddButton(10, 197, 4007, 4009, 7, GumpButtonType.Reply, 0);
            AddButton(10, 222, 4007, 4009, 8, GumpButtonType.Reply, 0);
            AddButton(10, 247, 4007, 4009, 9, GumpButtonType.Reply, 0);
            AddButton(10, 272, 4007, 4009, 10, GumpButtonType.Reply, 0);
            AddButton(10, 297, 4007, 4009, 11, GumpButtonType.Reply, 0);
            AddButton(10, 322, 4007, 4009, 12, GumpButtonType.Reply, 0);
            AddButton(10, 347, 4007, 4009, 13, GumpButtonType.Reply, 0);

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
                BaseGump.SendGump(new ArenaStoneGump(User, Arena));
            }
            else
            {
                switch (info.ButtonID)
                {
                    case 1:
                        Duel.Entries++;
                        if (Duel.Entries > ArenaDuel.MaxEntries)
                            Duel.Entries = 0;
                        Refresh();
                        break;
                    case 2:
                        Duel.RoomType = Duel.RoomType.Public ? RoomType.Private : RoomType.Public;
                        Refresh();
                        break;
                    case 3:
                        Refresh();
                        break;
                    case 4:
                        Duel.BattleMode = Duel.BattleMode.Survival ? BattleMode.Team : BattleMode.Survival;
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
                            case EntryFee.TenThousand: Duel.EntryFee = EntryFee.TwentyThousand; break;
                            case EntryFee.TwentyThousand: Duel.EntryFee = EntryFee.ThirtyThousand; break;
                            case EntryFee.ThirtyThousand: Duel.EntryFee = EntryFee.Zero; break;
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
                            case PotionRules.All: Duel.PotionRules = PotionRules.None;
                            case PotionRules.None: Duel.PotionRules = PotionRules.AllHealing;
                            case PotionRules.NoHealing: Duel.PotionRules = PotionRules.All;
                        }
                        Refresh();
                        break;
                    case 50:
                        Arena.AddPendingDuel(Duel);
                        BaseGump.SendGump(new PendingDuelGump(User, Duel, Arena));
                        PVPArenaSystem.SendMessage(User, 1115800); // You have created a new duel session.
                        break;
                }
            }
        }

        private string GetTimeLimit()
        {
            switch (Duel.TimeLimit)
            {
                default:
                case TimeLimit.FiveMinutes: return "5 minutes";
                case TimeLimit.TenMinutes: return "10 minutes";
                case TimeLimit.FifteenMinutes: return "15 minutes";
                case TimeLimit.TwentyMinutes: return "20 minutes";
                case TimeLimit.ThrityMinutes: return "30 minutes";
            }
        }

        private string GetEntryFee()
        {
            switch (Duel.EntryFee)
            {
                default:
                case EntryFee.Zero: return "0 gold";
                case EntryFee.OneThousand: return "1000 gold";
                case EntryFee.FiveThousand: return "5000 gold";
                case EntryFee.TenThousand: return "10000 gold";
                case EntryFee.TwentyThousand: return "20000 gold";
                case EntryFee.FiftyThousand: return "50000 gold";
            }
        }

        private string GetSlots()
        {
            return String.Format("{0} slot(s)", Duel.PetSlots);
        }

        private string GetPotion()
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

    public class PendingDuelGump : BaseGump
    {
        public PVPArena Arena { get; private set; }
        public ArenaDuel Duel { get; private set; }
        public List<PlayerMobile> Participants { get; private set; }

        public PendingDuelGump(PlayerMobile pm, ArenaDuel duel, PVPArena arena)
            : base(pm, 50, 50)
        {
            Duel = duel;
            Arena = arena;
            Participants = Duel.ParticipantList();
        }

        public override void AddGumpLayout()
        {
            int width = duel.ParticipantCount > 5 ? 400 : 300;
            AddBackground(0, 0, width, 377, 9200);

            AddImageTiled(8, 8, width - 16, 25, 2624);
            AddAlphaRegion(8, 8, width - 16, 25);

            AddHtmlLocalized(0, 10, 300, 20, CenterLoc, "#1115624", 0xFFFF, false, false); // Arena Menu - Queuing

            AddImageTiled(8, 40, 284, 132, 2624);
            AddAlphaRegion(8, 40, 284, 132);

            AddImageTiled(8, 180, 284, 25, 2624);
            AddAlphaRegion(8, 180, 284, 25);

            AddHtmlLocalized(0, 183, 300, 20, CenterLoc, Duel.BattleMode == BattleMode.Survival ? "#1115763" : "#1116392", 0xFFFF, false, false); // Participant list : Team: Order (Blue) / Chaos (Red)

            AddImageTiled(8, 212, 284, 129, 2624);
            AddAlphaRegion(8, 212, 284, 129);

            AddImageTiled(8, 350, 284, 20, 2624);
            AddAlphaRegion(8, 350, 284, 20);

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

            int x = 10;
            int y = 215;
            int index = 0;

            if (Duel.BattleMode == BattleMode.Survival)
            {
                for(int i = 0; i < Participants.Count; i++)
                {
                    AddButton(x, y, 4011, 4013, 50 + i, GumpButtonType.Reply, 0);
                    AddButton(x + 37, y, 4017, 4019, 100 + i, GumpButtonType.Reply, 0);
                    AddLabelCropped(x + 105, y, 135, 20, 0x481, kvp.Key.Name);

                    y += 25;

                    if (i == 5)
                    {
                        x = 210;
                        y = 215;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Participants.Count; i++)
                {
                    AddButton(x, y, 4011, 4013, 50 + i, GumpButtonType.Reply, 0);
                    AddButton(x + 37, y, 4017, 4019, 100 + i, GumpButtonType.Reply, 0);
                    AddButton(x + 74, y, 4026, 4029, 200 + i, GumpButtonType.Reply, 0);
                    AddLabelCropped(x + 105, y, 135, 20, Duel.TeamOrder.Contains(kvp.Key) ? 493 : 437, kvp.Key.Name);

                    y += 25;

                    if (i == 5)
                    {
                        x = 210;
                        y = 215;
                    }
                }
            }

            AddImage(218, 245, 5587);

            AddButton(8, 349, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 349, 120, 20, 1150300, 0xFFFF, false, false); // CANCEL
        }

        public override void OnResponse(RelayInfo info)
        {
            if(info.ButtonID > 0 && !Arena.PendingDuels.ContainsKey(Duel))
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
                            BaseGump.SendGump(new IndividualStatsGump(User, Participants[id]));
                        }
                    }
                    else if (info.ButtonID < 200)
                    {
                        int id = info.ButtonID - 100;

                        if (id >= 0 && id < Participants.Count)
                        {
                            if (Participant[id] == Duel.Host)
                            {
                                User.SendLocalizedMessage(1115838); // You cannot ban yourself!
                                Refresh();
                            }
                            else
                            {
                                User.SendGump(new WarningGump(1115834, C32216(0xB22222), 1115835, 0xFFFF, 348, 222, (from, okeedokee, state) =>
                                {
                                    if (!Arena.PendingDuels.ContainsKey(Duel))
                                    {
                                        PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                                    }
                                    else if (okeedokee)
                                    {
                                        Duel.RemovePlayer(User, true);
                                    }
                                }, null, true, 1115836, 1115837));
                            }
                        }
                    }
                    else
                    {
                        int id = info.ButtonID - 100;

                        if (id >= 0 && id < Participants.Count)
                        {
                            Duel.SwapParticipant(Participants[id]);
                            PVPArenaSystem.SendMessage(Participants[id], 1116462); // The host player has moved you to another team.
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
                        PvpArenaSystem.SendMessage(User, 1115958); // To start a duel, you will need to gather more players/participants.
                    }
                    else
                    {
                        Arena.TryBeginDuel(Duel);
                    }
                    break;
                case 2:
                    if (User != Duel.Host)
                    {
                        PVPArenaSystem.SendMessage(User, 1115949); // Only the session's host may use this function.
                    }
                    else
                    {
                        User.SendGump(new WarningGump(1115819, C32216(0xB22222), 1115820, 0xFFFF, 348, 222, (from, okeedokee, state) =>
                            {
                                if (!Arena.PendingDuels.ContainsKey(Duel))
                                {
                                    PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                                }
                                else if (okeedokee)
                                {
                                    Arena.RemovePendingDuel(Duel, true);
                                }
                            }, null, true, 1115821, 1115822));
                        
                    }
                    break;
                case 3:
                    if (User != Duel.Host)
                    {
                        PVPArenaSystem.SendMessage(User, 1115949); // Only the session's host may use this function.
                    }
                    else
                    {
                        User.BeginTarget(10, false, TargetFlags.None, (from, targeted) =>
                            {
                                if (!Arena.PendingDuels.ContainsKey(Duel))
                                {
                                    PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                                }
                                else if (from is PlayerMobile)
                                {
                                    var pm = from as PlayerMobile;
                                    var entry = PVPArenaSystem.Instance.GetPlayerEntry<PlayerStatsEntry>(User);

                                    if (!pm.Alive)
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116148); // You cannot invite dead players.
                                    }
                                    if (pm.IsYoung)
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116152); // You have sent the invitation to the player.
                                    }
                                    else if (entry.IgnoreInvites)
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116157); // The player is not accepting invitations.
                                    }
                                    else if (Duel.IsParticipant(pm))
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116150); // The player has already joined in the session.
                                    }
                                    else if (Duel.IsBanned(pm))
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116151); // You have already banned the player once.
                                    }
                                    else if (!pm.InRange(Arena.Stone.Location, 12))
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116149); // The targeted player is too far away from the arena stone.
                                    }
                                    else
                                    {
                                        PVPArenaSystem.SendMessage(User, 1116152); // You have sent the invitation to the player.
                                        PVPArenaSystem.SendMessage(pm, 1116212); // You have been invited to a duel.  Select the “OK” button to join this duel.

                                        BaseGump.SendGump(new OfferDuelGump(pm, Duel, Arena, true));
                                    }
                                }
                                else
                                {
                                    User.SendLocalizedMessage(503348); // You can only do this to players.
                                    Refresh();
                                }
                            });
                    }
                case 4:
                    BaseGump.SendGump(new BookedDuelsGump(User));
                    break;
                case 5:
                    BaseGump.SendGump(new DuelRulesGump(User));
                    break;
            }
        }
    }

    public class JoinDuelGump : BaseGump
    {
        public List<ArenaDuel> PendingDuels { get; private set; }
        public PVPArena Arena { get; private set; }

        public JoinDuelGump(PlayerMobile pm, List<ArenaDuel> list, PVPArena arena)
            : base(pm, 50, 50)
        {
            PendingDuels = list;
            Arena = arena;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 310, 365, 9200);
            AddPage(0);

            AddImageTiled(8, 8, 294, 25, 2624);
            AddAlphaRegion(8, 8, 294, 25);

            AddImageTiled(8, 38, 84, 25, 2624);
            AddAlphaRegion(8, 38, 84, 25);

            AddImageTiled(95, 38, 207, 25, 2624);
            AddAlphaRegion(95, 38, 207, 25);

            AddImageTiled(8, 70, 37, 260, 2624);
            AddAlphaRegion(8, 70, 37, 260);

            AddImageTiled(48, 70, 45, 260, 2624);
            AddAlphaRegion(48, 70, 45, 260);

            AddImageTiled(95, 70, 205, 260, 2624);
            AddAlphaRegion(95, 70, 205, 260);

            AddImageTiled(8, 337, 294, 20, 2624);
            AddAlphaRegion(8, 337, 294, 20);

            AddHtmlLocalized(0, 10, 310, 20, CenterLoc, "#1115842", 0xFFFF, false, false); // Arena Menu - Entry
            AddHtmlLocalized(8, 42, 310, 20, CenterLoc, "#1115847", 0xFFFF, false, false); // Entry
            AddHtmlLocalized(103, 42, 310, 20, 1115848, 0xFFFF, false, false); // Host Player

            int perPage = 8;
            int page = 1;
            int y = 44;

            AddPage(page);

            for (int i = 0; i < PendingDuels.Count; i++)
            {
                var duel = PendingDuels[i];

                AddButton(10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
                AddLabel(54, y, 0x481, String.Format("{0}/{1}", (i + 1).ToString, duel.Entries.ToString()));
                AddLabel(103, y, 0x481, duel.Host.Name);

                if (i != 0 && i % perPage == 0)
                {
                    page++;

                    if (i < PendingDuels.Count - 1)
                    {
                        AddButton(272, 337, 4005, 4007, GumpButtonType.Page, i + 1);
                        AddPage(page);
                    }

                    if (page > 1)
                    {
                        AddButton(240, 337, 4005, 4007, GumpButtonType.Page, i - 1);
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
                BaseGump.SendGump(new ArenaStoneGump(User, Arena));
            }
            else
            {
                int id = info.ButtonID - 1;

                if (id >= 0 && id < PendingDuels.Count)
                {
                    var duel = PendingDuels[id];

                    if (!Arena.PendingDuels.ContainsKey(duel))
                    {
                        PVPArenaSystem.SendMessage(User, 1115957); // This session has expired. Please create a new session and try again.
                    }
                    else
                    {
                        BaseGump.SendGump(new OfferDuelGump(User, duel, Arena, false));
                    }
                }
            }
        }
    }

    public class OfferDuelGump : BaseGump
    {
        public ArenaDuel Duel { get; private set; }
        public PVPArena Arena { get; private set; }
        public bool FromPlayer { get; private set; }
        public bool DetailsOnly { get; private set; }

        public OfferDuelGump(PlayerMobile pm, ArenaDuel duel, PVPArena arena, bool fromPlayer, bool detailsOnly = false)
            : base(pm, 50, 50)
        {
            Duel = duel;
            Arena = arena;
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

            for(int i = 0; i < partList.Count; i++)
            {
                AddLabel(17, 80 + (i * 20), 0x481, partList[i].Name);
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
                AddHtmlLocalized(20, 415, 460, CenterLoc, "#1115874", C32216(0xFF6347), false, false);
                // The arena gate has opened near the arena stone. <br>You've ninety seconds to use the gate or you'll be removed from this duel.
            }
            else
            {
                AddButton(8, 433, 4014, 4016, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 433, 100, 20, FromPlayer ? 1150300 : 1150154, 0xFFFF, false, false); // CANCEL : BACK

                AddButton(407, 433, 4023, 4025, 50, GumpButtonType.Reply, 0);
                AddHtmlLocalized(442, 383, 60, 20, 1115873, 0xFFFF, false, false); // ENTRY
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch(info.ButtonID)
            {
                case 0:
                    if (!FromPlayer)
                    {
                        List<ArenaDuel> list = Arena.GetPendingPublic();

                        if (list != null && list.Count > 0)
                        {
                            BaseGump.SendGump(new JoinDuelGump(User, list, Arena));
                        }
                        else
                        {
                            User.SendLocalizedMessage(1115966); // There are no duel sessions available.
                        }
                    }
                    break;
                case 1:
                    Duel.TryAddPlayer(User);
                    break;
            }
        }
    }

    public class BookedDuelsGump : BaseGump
    {
        public PVPArena Arena { get; private set; }

        public BookedDuelsGump(PlayerMobile pm, PVPArena arena)
            : base(pm, 50, 50)
        {
            Arena = arena;
        }

        public override void AddGumpLayout()
        {
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                var duel = Arena.GetPendingDuel(User);

                if (duel == null)
                {
                    BaseGump.SendGump(new ArenaStoneGump(User, Arena));
                }
                else
                {
                    BaseGump.SendGump(new PendingDuelGump(User, duel, Arena));
                }
            }
            else
            {

            }
        }
    }

    public class IndividualStatsGump : BaseGump
    {
        public IndividualStatsGump(PlayerMobile pm, PlayerMobile stats)
            : base(pm, 50, 50)
        {
        }

        public override void AddGumpLayout()
        {
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                var duel = Arena.GetPendingDuel(User);

                if (duel == null)
                {
                    BaseGump.SendGump(new ArenaStoneGump(User, Arena));
                }
                else
                {
                    BaseGump.SendGump(new PendingDuelGump(User, duel, Arena));
                }
            }
            else
            {
            }
        }
    }

    public class ArenaRankingsGump : BaseGump
    {
        public PVPArena Arena { get; private set; }

        public ArenaRankingsGump(PlayerMobile pm, PVPArena arena)
            : base(pm, 50, 50)
        {
            Arena = arena;
        }

        public override void AddGumpLayout()
        {
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                var duel = Arena.GetPendingDuel(User);

                if (duel == null)
                {
                    BaseGump.SendGump(new ArenaStoneGump(User, Arena));
                }
                else
                {
                    BaseGump.SendGump(new PendingDuelGump(User, duel, Arena));
                }
            }
            else
            {
            }
        }
    }

    public class DuelResultsGump : BaseGump
    {
        public ArenaDuel Duel { get; private set; }
        public ArenaTeam Winners { get; private set; }

        public DuelResultsGump(PlayerMobile pm, ArenaDuel duel, ArenaTeam winners)
            : base(pm, 50, 50)
        {
            Duel = duel;
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

            AddImageTiled(8, 343, 355, 246, 2624);
            AddAlphaRegion(8, 343, 355, 246);

            string winner = "Draw";

            if (Winners != null)
            {
                if (Duel.BattleMode == BattleMode.Team)
                {
                    if (Winners == Duel.TeamOrder)
                        winner = "Team Order";
                    else if (Winners == Duel.TeamChaos)
                        winner = "Team Chaos";
                }
                else
                {
                    var pm = Winners.PlayerZero;
                    winner = pm == null ? "Someone" : pm.Name;
                }
            }

            AddHtmlLocalized(0, 10, 371, 20, CenterLoc, "#1115990", 0xFFFF, false, false); // Arena Menu - Results
            AddHtmlLocalized(145, 50, 371, 20, 1153207, winner, 0xFFFF, false, false); // Winner: ~1_VAL~

            int i = 0;

            foreach (var kvp in Dual.KillRecord)
            {
                AddLabel(15, 100 + (i * 25), 0x481, kvp.Key.Name);
                AddHtmlLocalized(158, 100 + (i * 25), 100, 20, 1115986, 0xFFFF, false, false); // KILLED
                AddLabel(231, 100 + (i * 25), 0x481, kvp.Value.Name);

                i++;

                if (i >= 10)
                    break;
            }

            AddHtmlLocalized(42, 343, 150, 20, 1150300, 0xFFFF, false, false); // CANCEL
            AddButton(8, 343, 4017, 4019, 0, GumpButtonType.Reply, 0);
        }
    }
}