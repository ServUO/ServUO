using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.ConPVP
{
    public enum TournamentStage
    {
        Inactive,
        Signup,
        Fighting
    }

    public enum GroupingType
    {
        HighVsLow,
        Nearest,
        Random
    }

    public enum TieType
    {
        Random,
        Highest,
        Lowest,
        FullElimination,
        FullAdvancement
    }

    public class TournamentRegistrar : Banker
    {
        private TournamentController m_Tournament;

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentController Tournament
        {
            get
            {
                return this.m_Tournament;
            }
            set
            {
                this.m_Tournament = value;
            }
        }

        [Constructable]
        public TournamentRegistrar()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(30.0), new TimerCallback(Announce_Callback));
        }

        private void Announce_Callback()
        {
            Tournament tourny = null;

            if (this.m_Tournament != null)
                tourny = this.m_Tournament.Tournament;

            if (tourny != null && tourny.Stage == TournamentStage.Signup)
                this.PublicOverheadMessage(MessageType.Regular, 0x35, false, "Come one, come all! Do you aspire to be a fighter of great renown? Join this tournament and show the world your abilities.");
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            Tournament tourny = null;

            if (this.m_Tournament != null)
                tourny = this.m_Tournament.Tournament;

            if (this.InRange(m, 4) && !this.InRange(oldLocation, 4) && tourny != null && tourny.Stage == TournamentStage.Signup && m.CanBeginAction(this))
            {
                Ladder ladder = Ladder.Instance;

                if (ladder != null)
                {
                    LadderEntry entry = ladder.Find(m);

                    if (entry != null && Ladder.GetLevel(entry.Experience) < tourny.LevelRequirement)
                        return;
                }

                if (tourny.HasParticipant(m))
                    return;

                this.PrivateOverheadMessage(MessageType.Regular, 0x35, false, String.Format("Hello m'{0}. Dost thou wish to enter this tournament? You need only to write your name in this book.", m.Female ? "Lady" : "Lord"), m.NetState);
                m.BeginAction(this);
                Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerStateCallback(ReleaseLock_Callback), m);
            }
        }

        private void ReleaseLock_Callback(object obj)
        {
            ((Mobile)obj).EndAction(this);
        }

        public TournamentRegistrar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)this.m_Tournament);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Tournament = reader.ReadItem() as TournamentController;
                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(30.0), new TimerCallback(Announce_Callback));
        }
    }

    public class TournamentSignupItem : Item
    {
        private TournamentController m_Tournament;
        private Mobile m_Registrar;

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentController Tournament
        {
            get
            {
                return this.m_Tournament;
            }
            set
            {
                this.m_Tournament = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Registrar
        {
            get
            {
                return this.m_Registrar;
            }
            set
            {
                this.m_Registrar = value;
            }
        }

        public override string DefaultName
        {
            get
            {
                return "tournament signup book";
            }
        }

        [Constructable]
        public TournamentSignupItem()
            : base(4029)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that
            }
            else if (this.m_Tournament != null)
            {
                Tournament tourny = this.m_Tournament.Tournament;

                if (tourny != null)
                {
                    if (this.m_Registrar != null)
                        this.m_Registrar.Direction = this.m_Registrar.GetDirectionTo(this);

                    switch ( tourny.Stage )
                    {
                        case TournamentStage.Fighting:
                            {
                                if (this.m_Registrar != null)
                                {
                                    if (tourny.HasParticipant(from))
                                    {
                                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                            0x35, false, "Excuse me? You are already signed up.", from.NetState);
                                    }
                                    else
                                    {
                                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                            0x22, false, "The tournament has already begun. You are too late to signup now.", from.NetState);
                                    }
                                }

                                break;
                            }
                        case TournamentStage.Inactive:
                            {
                                if (this.m_Registrar != null)
                                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                        0x35, false, "The tournament is closed.", from.NetState);

                                break;
                            }
                        case TournamentStage.Signup:
                            {
                                Ladder ladder = Ladder.Instance;

                                if (ladder != null)
                                {
                                    LadderEntry entry = ladder.Find(from);

                                    if (entry != null && Ladder.GetLevel(entry.Experience) < tourny.LevelRequirement)
                                    {
                                        if (this.m_Registrar != null)
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, "You have not yet proven yourself a worthy dueler.", from.NetState);
                                        }

                                        break;
                                    }
                                }

                                if (from.HasGump(typeof(AcceptTeamGump)))
                                {
                                    if (this.m_Registrar != null)
                                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                            0x22, false, "You must first respond to the offer I've given you.", from.NetState);
                                }
                                else if (from.HasGump(typeof(AcceptDuelGump)))
                                {
                                    if (this.m_Registrar != null)
                                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                            0x22, false, "You must first cancel your duel offer.", from.NetState);
                                }
                                else if (from is PlayerMobile && ((PlayerMobile)from).DuelContext != null)
                                {
                                    if (this.m_Registrar != null)
                                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                            0x22, false, "You are already participating in a duel.", from.NetState);
                                }
                                else if (!tourny.HasParticipant(from))
                                {
                                    ArrayList players = new ArrayList();
                                    players.Add(from);
                                    from.CloseGump(typeof(ConfirmSignupGump));
                                    from.SendGump(new ConfirmSignupGump(from, this.m_Registrar, tourny, players));
                                }
                                else if (this.m_Registrar != null)
                                {
                                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                        0x35, false, "You have already entered this tournament.", from.NetState);
                                }

                                break;
                            }
                    }
                }
            }
        }

        public TournamentSignupItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)this.m_Tournament);
            writer.Write((Mobile)this.m_Registrar);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Tournament = reader.ReadItem() as TournamentController;
                        this.m_Registrar = reader.ReadMobile();
                        break;
                    }
            }
        }
    }

    public class ConfirmSignupGump : Gump
    {
        private readonly Mobile m_From;
        private readonly Tournament m_Tournament;
        private readonly ArrayList m_Players;
        private readonly Mobile m_Registrar;

        private const int BlackColor32 = 0x000008;
        private const int LabelColor32 = 0xFFFFFF;

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void AddBorderedText(int x, int y, int width, int height, string text, int color, int borderColor)
        {
            this.AddColoredText(x - 1, y - 1, width, height, text, borderColor);
            this.AddColoredText(x - 1, y + 1, width, height, text, borderColor);
            this.AddColoredText(x + 1, y - 1, width, height, text, borderColor);
            this.AddColoredText(x + 1, y + 1, width, height, text, borderColor);
            this.AddColoredText(x, y, width, height, text, color);
        }

        private void AddColoredText(int x, int y, int width, int height, string text, int color)
        {
            if (color == 0)
                this.AddHtml(x, y, width, height, text, false, false);
            else
                this.AddHtml(x, y, width, height, this.Color(text, color), false, false);
        }

        public void AddGoldenButton(int x, int y, int bid)
        {
            this.AddButton(x, y, 0xD2, 0xD2, bid, GumpButtonType.Reply, 0);
            this.AddButton(x + 3, y + 3, 0xD8, 0xD8, bid, GumpButtonType.Reply, 0);
        }

        public ConfirmSignupGump(Mobile from, Mobile registrar, Tournament tourny, ArrayList players)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Registrar = registrar;
            this.m_Tournament = tourny;
            this.m_Players = players;

            this.m_From.CloseGump(typeof(AcceptTeamGump));
            this.m_From.CloseGump(typeof(AcceptDuelGump));
            this.m_From.CloseGump(typeof(DuelContextGump));
            this.m_From.CloseGump(typeof(ConfirmSignupGump));

            #region Rules
            Ruleset ruleset = tourny.Ruleset;
            Ruleset basedef = ruleset.Base;

            int height = 185 + 60 + 12;

            int changes = 0;

            BitArray defs;

            if (ruleset.Flavors.Count > 0)
            {
                defs = new BitArray(basedef.Options);

                for (int i = 0; i < ruleset.Flavors.Count; ++i)
                    defs.Or(((Ruleset)ruleset.Flavors[i]).Options);

                height += ruleset.Flavors.Count * 18;
            }
            else
            {
                defs = basedef.Options;
            }

            BitArray opts = ruleset.Options;

            for (int i = 0; i < opts.Length; ++i)
            {
                if (defs[i] != opts[i])
                    ++changes;
            }

            height += (changes * 22);

            height += 10 + 22 + 25 + 25;

            if (tourny.PlayersPerParticipant > 1)
                height += 36 + (tourny.PlayersPerParticipant * 20);
            #endregion

            this.Closable = false;

            this.AddPage(0);

            //AddBackground( 0, 0, 400, 220, 9150 );
            this.AddBackground(1, 1, 398, height, 3600);
            //AddBackground( 16, 15, 369, 189, 9100 );

            this.AddImageTiled(16, 15, 369, height - 29, 3604);
            this.AddAlphaRegion(16, 15, 369, height - 29);

            this.AddImage(215, -43, 0xEE40);
            //AddImage( 330, 141, 0x8BA );

            StringBuilder sb = new StringBuilder();

            if (tourny.TournyType == TournyType.FreeForAll)
            {
                sb.Append("FFA");
            }
            else if (tourny.TournyType == TournyType.RandomTeam)
            {
                sb.Append("Team");
            }
            else if (tourny.TournyType == TournyType.RedVsBlue)
            {
                sb.Append("Red v Blue");
            }
            else
            {
                for (int i = 0; i < tourny.ParticipantsPerMatch; ++i)
                {
                    if (sb.Length > 0)
                        sb.Append('v');

                    sb.Append(tourny.PlayersPerParticipant);
                }
            }

            if (tourny.EventController != null)
                sb.Append(' ').Append(tourny.EventController.Title);

            sb.Append(" Tournament Signup");

            this.AddBorderedText(22, 22, 294, 20, this.Center(sb.ToString()), LabelColor32, BlackColor32);
            this.AddBorderedText(22, 50, 294, 40, "You have requested to join the tournament. Do you accept the rules?", 0xB0C868, BlackColor32);

            this.AddImageTiled(32, 88, 264, 1, 9107);
            this.AddImageTiled(42, 90, 264, 1, 9157);

            #region Rules
            int y = 100;

            string groupText = null;

            switch ( tourny.GroupType )
            {
                case GroupingType.HighVsLow:
                    groupText = "High vs Low";
                    break;
                case GroupingType.Nearest:
                    groupText = "Closest opponent";
                    break;
                case GroupingType.Random:
                    groupText = "Random";
                    break;
            }

            this.AddBorderedText(35, y, 190, 20, String.Format("Grouping: {0}", groupText), LabelColor32, BlackColor32);
            y += 20;

            string tieText = null;

            switch ( tourny.TieType )
            {
                case TieType.Random:
                    tieText = "Random";
                    break;
                case TieType.Highest:
                    tieText = "Highest advances";
                    break;
                case TieType.Lowest:
                    tieText = "Lowest advances";
                    break;
                case TieType.FullAdvancement:
                    tieText = (tourny.ParticipantsPerMatch == 2 ? "Both advance" : "Everyone advances");
                    break;
                case TieType.FullElimination:
                    tieText = (tourny.ParticipantsPerMatch == 2 ? "Both eliminated" : "Everyone eliminated");
                    break;
            }

            this.AddBorderedText(35, y, 190, 20, String.Format("Tiebreaker: {0}", tieText), LabelColor32, BlackColor32);
            y += 20;

            string sdText = "Off";

            if (tourny.SuddenDeath > TimeSpan.Zero)
            {
                sdText = String.Format("{0}:{1:D2}", (int)tourny.SuddenDeath.TotalMinutes, tourny.SuddenDeath.Seconds);

                if (tourny.SuddenDeathRounds > 0)
                    sdText = String.Format("{0} (first {1} rounds)", sdText, tourny.SuddenDeathRounds);
                else
                    sdText = String.Format("{0} (all rounds)", sdText);
            }

            this.AddBorderedText(35, y, 240, 20, String.Format("Sudden Death: {0}", sdText), LabelColor32, BlackColor32);
            y += 20;

            y += 6;
            this.AddImageTiled(32, y - 1, 264, 1, 9107);
            this.AddImageTiled(42, y + 1, 264, 1, 9157);
            y += 6;

            this.AddBorderedText(35, y, 190, 20, String.Format("Ruleset: {0}", basedef.Title), LabelColor32, BlackColor32);
            y += 20;

            for (int i = 0; i < ruleset.Flavors.Count; ++i, y += 18)
                this.AddBorderedText(35, y, 190, 20, String.Format(" + {0}", ((Ruleset)ruleset.Flavors[i]).Title), LabelColor32, BlackColor32);

            y += 4;

            if (changes > 0)
            {
                this.AddBorderedText(35, y, 190, 20, "Modifications:", LabelColor32, BlackColor32);
                y += 20;

                for (int i = 0; i < opts.Length; ++i)
                {
                    if (defs[i] != opts[i])
                    {
                        string name = ruleset.Layout.FindByIndex(i);

                        if (name != null) // sanity
                        {
                            this.AddImage(35, y, opts[i] ? 0xD3 : 0xD2);
                            this.AddBorderedText(60, y, 165, 22, name, LabelColor32, BlackColor32);
                        }

                        y += 22;
                    }
                }
            }
            else
            {
                this.AddBorderedText(35, y, 190, 20, "Modifications: None", LabelColor32, BlackColor32);
                y += 20;
            }
            #endregion

            #region Team
            if (tourny.PlayersPerParticipant > 1)
            {
                y += 8;
                this.AddImageTiled(32, y - 1, 264, 1, 9107);
                this.AddImageTiled(42, y + 1, 264, 1, 9157);
                y += 8;

                this.AddBorderedText(35, y, 190, 20, "Your Team", LabelColor32, BlackColor32);
                y += 20;

                for (int i = 0; i < players.Count; ++i, y += 20)
                {
                    if (i == 0)
                        this.AddImage(35, y, 0xD2);
                    else
                        this.AddGoldenButton(35, y, 1 + i);

                    this.AddBorderedText(60, y, 200, 20, ((Mobile)players[i]).Name, LabelColor32, BlackColor32);
                }

                for (int i = players.Count; i < tourny.PlayersPerParticipant; ++i, y += 20)
                {
                    if (i == 0)
                        this.AddImage(35, y, 0xD2);
                    else
                        this.AddGoldenButton(35, y, 1 + i);

                    this.AddBorderedText(60, y, 200, 20, "(Empty)", LabelColor32, BlackColor32);
                }
            }
            #endregion

            y += 8;
            this.AddImageTiled(32, y - 1, 264, 1, 9107);
            this.AddImageTiled(42, y + 1, 264, 1, 9157);
            y += 8;

            this.AddRadio(24, y, 9727, 9730, true, 1);
            this.AddBorderedText(60, y + 5, 250, 20, "Yes, I wish to join the tournament.", LabelColor32, BlackColor32);
            y += 35;

            this.AddRadio(24, y, 9727, 9730, false, 2);
            this.AddBorderedText(60, y + 5, 250, 20, "No, I do not wish to join.", LabelColor32, BlackColor32);
            y += 35;

            y -= 3;
            this.AddButton(314, y, 247, 248, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1 && info.IsSwitched(1))
            {
                Tournament tourny = this.m_Tournament;
                Mobile from = this.m_From;

                switch ( tourny.Stage )
                {
                    case TournamentStage.Fighting:
                        {
                            if (this.m_Registrar != null)
                            {
                                if (this.m_Tournament.HasParticipant(from))
                                {
                                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                        0x35, false, "Excuse me? You are already signed up.", from.NetState);
                                }
                                else
                                {
                                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                        0x22, false, "The tournament has already begun. You are too late to signup now.", from.NetState);
                                }
                            }

                            break;
                        }
                    case TournamentStage.Inactive:
                        {
                            if (this.m_Registrar != null)
                                this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                    0x35, false, "The tournament is closed.", from.NetState);

                            break;
                        }
                    case TournamentStage.Signup:
                        {
                            if (this.m_Players.Count != tourny.PlayersPerParticipant)
                            {
                                if (this.m_Registrar != null)
                                {
                                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                        0x35, false, "You have not yet chosen your team.", from.NetState);
                                }

                                this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                                break;
                            }

                            Ladder ladder = Ladder.Instance;

                            for (int i = 0; i < this.m_Players.Count; ++i)
                            {
                                Mobile mob = (Mobile)this.m_Players[i];

                                LadderEntry entry = (ladder == null ? null : ladder.Find(mob));

                                if (entry != null && Ladder.GetLevel(entry.Experience) < tourny.LevelRequirement)
                                {
                                    if (this.m_Registrar != null)
                                    {
                                        if (mob == from)
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, "You have not yet proven yourself a worthy dueler.", from.NetState);
                                        }
                                        else
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, String.Format("{0} has not yet proven themselves a worthy dueler.", mob.Name), from.NetState);
                                        }
                                    }

                                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                                    return;
                                }
                                else if (tourny.HasParticipant(mob))
                                {
                                    if (this.m_Registrar != null)
                                    {
                                        if (mob == from)
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, "You have already entered this tournament.", from.NetState);
                                        }
                                        else
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, String.Format("{0} has already entered this tournament.", mob.Name), from.NetState);
                                        }
                                    }

                                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                                    return;
                                }
                                else if (mob is PlayerMobile && ((PlayerMobile)mob).DuelContext != null)
                                {
                                    if (this.m_Registrar != null)
                                    {
                                        if (mob == from)
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, "You are already assigned to a duel. You must yield it before joining this tournament.", from.NetState);
                                        }
                                        else
                                        {
                                            this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                                0x35, false, String.Format("{0} is already assigned to a duel. They must yield it before joining this tournament.", mob.Name), from.NetState);
                                        }
                                    }

                                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                                    return;
                                }
                            }

                            if (this.m_Registrar != null)
                            {
                                string fmt;

                                if (tourny.PlayersPerParticipant == 1)
                                    fmt = "As you say m'{0}. I've written your name to the bracket. The tournament will begin {1}.";
                                else if (tourny.PlayersPerParticipant == 2)
                                    fmt = "As you wish m'{0}. The tournament will begin {1}, but first you must name your partner.";
                                else
                                    fmt = "As you wish m'{0}. The tournament will begin {1}, but first you must name your team.";

                                string timeUntil;
                                int minutesUntil = (int)Math.Round(((tourny.SignupStart + tourny.SignupPeriod) - DateTime.UtcNow).TotalMinutes);

                                if (minutesUntil == 0)
                                    timeUntil = "momentarily";
                                else
                                    timeUntil = String.Format("in {0} minute{1}", minutesUntil, minutesUntil == 1 ? "" : "s");

                                this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                                    0x35, false, String.Format(fmt, from.Female ? "Lady" : "Lord", timeUntil), from.NetState);
                            }

                            TournyParticipant part = new TournyParticipant(from);
                            part.Players.Clear();
                            part.Players.AddRange(this.m_Players);

                            tourny.Participants.Add(part);

                            break;
                        }
                }
            }
            else if (info.ButtonID > 1)
            {
                int index = info.ButtonID - 1;

                if (index > 0 && index < this.m_Players.Count)
                {
                    this.m_Players.RemoveAt(index);
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                }
                else if (this.m_Players.Count < this.m_Tournament.PlayersPerParticipant)
                {
                    this.m_From.BeginTarget(12, false, TargetFlags.None, new TargetCallback(AddPlayer_OnTarget));
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                }
            }
        }

        private void AddPlayer_OnTarget(Mobile from, object obj)
        {
            Mobile mob = obj as Mobile;

            if (mob == null || mob == from)
            {
                this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                if (this.m_Registrar != null)
                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                        0x22, false, "Excuse me?", from.NetState);
            }
            else if (!mob.Player)
            {
                this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                if (mob.Body.IsHuman)
                    mob.SayTo(from, 1005443); // Nay, I would rather stay here and watch a nail rust.
                else
                    mob.SayTo(from, 1005444); // The creature ignores your offer.
            }
            else if (AcceptDuelGump.IsIgnored(mob, from) || mob.Blessed)
            {
                this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                if (this.m_Registrar != null)
                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                        0x22, false, "They ignore your invitation.", from.NetState);
            }
            else
            {
                PlayerMobile pm = mob as PlayerMobile;

                if (pm == null)
                    return;

                if (pm.DuelContext != null)
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They are already assigned to another duel.", from.NetState);
                }
                else if (mob.HasGump(typeof(AcceptTeamGump)))
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They have already been offered a partnership.", from.NetState);
                }
                else if (mob.HasGump(typeof(ConfirmSignupGump)))
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They are already trying to join this tournament.", from.NetState);
                }
                else if (this.m_Players.Contains(mob))
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "You have already named them as a team member.", from.NetState);
                }
                else if (this.m_Tournament.HasParticipant(mob))
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They have already entered this tournament.", from.NetState);
                }
                else if (this.m_Players.Count >= this.m_Tournament.PlayersPerParticipant)
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "Your team is full.", from.NetState);
                }
                else
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));
                    mob.SendGump(new AcceptTeamGump(from, mob, this.m_Tournament, this.m_Registrar, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x59, false, String.Format("As you command m'{0}. I've given your offer to {1}.", from.Female ? "Lady" : "Lord", mob.Name), from.NetState);
                }
            }
        }
    }

    public class AcceptTeamGump : Gump
    {
        private bool m_Active;

        private readonly Mobile m_From;
        private readonly Mobile m_Requested;
        private readonly Tournament m_Tournament;
        private readonly Mobile m_Registrar;
        private readonly ArrayList m_Players;

        private const int BlackColor32 = 0x000008;
        private const int LabelColor32 = 0xFFFFFF;

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void AddBorderedText(int x, int y, int width, int height, string text, int color, int borderColor)
        {
            this.AddColoredText(x - 1, y - 1, width, height, text, borderColor);
            this.AddColoredText(x - 1, y + 1, width, height, text, borderColor);
            this.AddColoredText(x + 1, y - 1, width, height, text, borderColor);
            this.AddColoredText(x + 1, y + 1, width, height, text, borderColor);
            this.AddColoredText(x, y, width, height, text, color);
        }

        private void AddColoredText(int x, int y, int width, int height, string text, int color)
        {
            if (color == 0)
                this.AddHtml(x, y, width, height, text, false, false);
            else
                this.AddHtml(x, y, width, height, this.Color(text, color), false, false);
        }

        public AcceptTeamGump(Mobile from, Mobile requested, Tournament tourny, Mobile registrar, ArrayList players)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Requested = requested;
            this.m_Tournament = tourny;
            this.m_Registrar = registrar;
            this.m_Players = players;

            this.m_Active = true;

            #region Rules
            Ruleset ruleset = tourny.Ruleset;
            Ruleset basedef = ruleset.Base;

            int height = 185 + 35 + 60 + 12;

            int changes = 0;

            BitArray defs;

            if (ruleset.Flavors.Count > 0)
            {
                defs = new BitArray(basedef.Options);

                for (int i = 0; i < ruleset.Flavors.Count; ++i)
                    defs.Or(((Ruleset)ruleset.Flavors[i]).Options);

                height += ruleset.Flavors.Count * 18;
            }
            else
            {
                defs = basedef.Options;
            }

            BitArray opts = ruleset.Options;

            for (int i = 0; i < opts.Length; ++i)
            {
                if (defs[i] != opts[i])
                    ++changes;
            }

            height += (changes * 22);

            height += 10 + 22 + 25 + 25;
            #endregion

            this.Closable = false;

            this.AddPage(0);

            this.AddBackground(1, 1, 398, height, 3600);

            this.AddImageTiled(16, 15, 369, height - 29, 3604);
            this.AddAlphaRegion(16, 15, 369, height - 29);

            this.AddImage(215, -43, 0xEE40);

            StringBuilder sb = new StringBuilder();

            if (tourny.TournyType == TournyType.FreeForAll)
            {
                sb.Append("FFA");
            }
            else if (tourny.TournyType == TournyType.RandomTeam)
            {
                sb.Append(tourny.ParticipantsPerMatch);
                sb.Append("-Team");
            }
            else if (tourny.TournyType == TournyType.RedVsBlue)
            {
                sb.Append("Red v Blue");
            }
            else
            {
                for (int i = 0; i < tourny.ParticipantsPerMatch; ++i)
                {
                    if (sb.Length > 0)
                        sb.Append('v');

                    sb.Append(tourny.PlayersPerParticipant);
                }
            }

            if (tourny.EventController != null)
                sb.Append(' ').Append(tourny.EventController.Title);

            sb.Append(" Tournament Invitation");

            this.AddBorderedText(22, 22, 294, 20, this.Center(sb.ToString()), LabelColor32, BlackColor32);

            this.AddBorderedText(22, 50, 294, 40,
                String.Format("You have been asked to partner with {0} in a tournament. Do you accept?", from.Name),
                0xB0C868, BlackColor32);

            this.AddImageTiled(32, 88, 264, 1, 9107);
            this.AddImageTiled(42, 90, 264, 1, 9157);

            #region Rules
            int y = 100;

            string groupText = null;

            switch ( tourny.GroupType )
            {
                case GroupingType.HighVsLow:
                    groupText = "High vs Low";
                    break;
                case GroupingType.Nearest:
                    groupText = "Closest opponent";
                    break;
                case GroupingType.Random:
                    groupText = "Random";
                    break;
            }

            this.AddBorderedText(35, y, 190, 20, String.Format("Grouping: {0}", groupText), LabelColor32, BlackColor32);
            y += 20;

            string tieText = null;

            switch ( tourny.TieType )
            {
                case TieType.Random:
                    tieText = "Random";
                    break;
                case TieType.Highest:
                    tieText = "Highest advances";
                    break;
                case TieType.Lowest:
                    tieText = "Lowest advances";
                    break;
                case TieType.FullAdvancement:
                    tieText = (tourny.ParticipantsPerMatch == 2 ? "Both advance" : "Everyone advances");
                    break;
                case TieType.FullElimination:
                    tieText = (tourny.ParticipantsPerMatch == 2 ? "Both eliminated" : "Everyone eliminated");
                    break;
            }

            this.AddBorderedText(35, y, 190, 20, String.Format("Tiebreaker: {0}", tieText), LabelColor32, BlackColor32);
            y += 20;

            string sdText = "Off";

            if (tourny.SuddenDeath > TimeSpan.Zero)
            {
                sdText = String.Format("{0}:{1:D2}", (int)tourny.SuddenDeath.TotalMinutes, tourny.SuddenDeath.Seconds);

                if (tourny.SuddenDeathRounds > 0)
                    sdText = String.Format("{0} (first {1} rounds)", sdText, tourny.SuddenDeathRounds);
                else
                    sdText = String.Format("{0} (all rounds)", sdText);
            }

            this.AddBorderedText(35, y, 240, 20, String.Format("Sudden Death: {0}", sdText), LabelColor32, BlackColor32);
            y += 20;

            y += 6;
            this.AddImageTiled(32, y - 1, 264, 1, 9107);
            this.AddImageTiled(42, y + 1, 264, 1, 9157);
            y += 6;

            this.AddBorderedText(35, y, 190, 20, String.Format("Ruleset: {0}", basedef.Title), LabelColor32, BlackColor32);
            y += 20;

            for (int i = 0; i < ruleset.Flavors.Count; ++i, y += 18)
                this.AddBorderedText(35, y, 190, 20, String.Format(" + {0}", ((Ruleset)ruleset.Flavors[i]).Title), LabelColor32, BlackColor32);

            y += 4;

            if (changes > 0)
            {
                this.AddBorderedText(35, y, 190, 20, "Modifications:", LabelColor32, BlackColor32);
                y += 20;

                for (int i = 0; i < opts.Length; ++i)
                {
                    if (defs[i] != opts[i])
                    {
                        string name = ruleset.Layout.FindByIndex(i);

                        if (name != null) // sanity
                        {
                            this.AddImage(35, y, opts[i] ? 0xD3 : 0xD2);
                            this.AddBorderedText(60, y, 165, 22, name, LabelColor32, BlackColor32);
                        }

                        y += 22;
                    }
                }
            }
            else
            {
                this.AddBorderedText(35, y, 190, 20, "Modifications: None", LabelColor32, BlackColor32);
                y += 20;
            }
            #endregion

            y += 8;
            this.AddImageTiled(32, y - 1, 264, 1, 9107);
            this.AddImageTiled(42, y + 1, 264, 1, 9157);
            y += 8;

            this.AddRadio(24, y, 9727, 9730, true, 1);
            this.AddBorderedText(60, y + 5, 250, 20, "Yes, I will join them.", LabelColor32, BlackColor32);
            y += 35;

            this.AddRadio(24, y, 9727, 9730, false, 2);
            this.AddBorderedText(60, y + 5, 250, 20, "No, I do not wish to fight.", LabelColor32, BlackColor32);
            y += 35;

            this.AddRadio(24, y, 9727, 9730, false, 3);
            this.AddBorderedText(60, y + 5, 270, 20, "No, most certainly not. Do not ask again.", LabelColor32, BlackColor32);
            y += 35;

            y -= 3;
            this.AddButton(314, y, 247, 248, 1, GumpButtonType.Reply, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(15.0), new TimerCallback(AutoReject));
        }

        public void AutoReject()
        {
            if (!this.m_Active)
                return;

            this.m_Active = false;

            this.m_Requested.CloseGump(typeof(AcceptTeamGump));
            this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

            if (this.m_Registrar != null)
            {
                this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                    0x22, false, String.Format("{0} seems unresponsive.", this.m_Requested.Name), this.m_From.NetState);

                this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                    0x22, false, String.Format("You have declined the partnership with {0}.", this.m_From.Name), this.m_Requested.NetState);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = this.m_From;
            Mobile mob = this.m_Requested;

            if (info.ButtonID != 1 || !this.m_Active)
                return;

            this.m_Active = false;

            if (info.IsSwitched(1))
            {
                PlayerMobile pm = mob as PlayerMobile;

                if (pm == null)
                    return;

                if (AcceptDuelGump.IsIgnored(mob, from) || mob.Blessed)
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They ignore your invitation.", from.NetState);
                }
                else if (pm.DuelContext != null)
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They are already assigned to another duel.", from.NetState);
                }
                else if (this.m_Players.Contains(mob))
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "You have already named them as a team member.", from.NetState);
                }
                else if (this.m_Tournament.HasParticipant(mob))
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "They have already entered this tournament.", from.NetState);
                }
                else if (this.m_Players.Count >= this.m_Tournament.PlayersPerParticipant)
                {
                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x22, false, "Your team is full.", from.NetState);
                }
                else
                {
                    this.m_Players.Add(mob);

                    this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                    if (this.m_Registrar != null)
                    {
                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x59, false, String.Format("{0} has accepted your offer of partnership.", mob.Name), from.NetState);

                        this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                            0x59, false, String.Format("You have accepted the partnership with {0}.", from.Name), mob.NetState);
                    }
                }
            }
            else
            {
                if (info.IsSwitched(3))
                    AcceptDuelGump.BeginIgnore(this.m_Requested, this.m_From);

                this.m_From.SendGump(new ConfirmSignupGump(this.m_From, this.m_Registrar, this.m_Tournament, this.m_Players));

                if (this.m_Registrar != null)
                {
                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                        0x22, false, String.Format("{0} has declined your offer of partnership.", mob.Name), from.NetState);

                    this.m_Registrar.PrivateOverheadMessage(MessageType.Regular,
                        0x22, false, String.Format("You have declined the partnership with {0}.", from.Name), mob.NetState);
                }
            }
        }
    }

    public class TournamentController : Item
    {
        private Tournament m_Tournament;

        [CommandProperty(AccessLevel.GameMaster)]
        public Tournament Tournament
        {
            get
            {
                return this.m_Tournament;
            }
            set
            {
            }
        }

        private static readonly ArrayList m_Instances = new ArrayList();

        public static bool IsActive
        {
            get
            {
                for (int i = 0; i < m_Instances.Count; ++i)
                {
                    TournamentController controller = (TournamentController)m_Instances[i];

                    if (controller != null && !controller.Deleted && controller.Tournament != null && controller.Tournament.Stage != TournamentStage.Inactive)
                        return true;
                }

                return false;
            }
        }

        public override string DefaultName
        {
            get
            {
                return "tournament controller";
            }
        }

        [Constructable]
        public TournamentController()
            : base(0x1B7A)
        {
            this.Visible = false;
            this.Movable = false;

            this.m_Tournament = new Tournament();
            m_Instances.Add(this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.AccessLevel >= AccessLevel.GameMaster && this.m_Tournament != null)
            {
                list.Add(new EditEntry(this.m_Tournament));

                if (this.m_Tournament.CurrentStage == TournamentStage.Inactive)
                    list.Add(new StartEntry(this.m_Tournament));
            }
        }

        private class EditEntry : ContextMenuEntry
        {
            private readonly Tournament m_Tournament;

            public EditEntry(Tournament tourny)
                : base(5101)
            {
                this.m_Tournament = tourny;
            }

            public override void OnClick()
            {
                this.Owner.From.SendGump(new PropertiesGump(this.Owner.From, this.m_Tournament));
            }
        }

        private class StartEntry : ContextMenuEntry
        {
            private readonly Tournament m_Tournament;

            public StartEntry(Tournament tourny)
                : base(5113)
            {
                this.m_Tournament = tourny;
            }

            public override void OnClick()
            {
                if (this.m_Tournament.Stage == TournamentStage.Inactive)
                {
                    this.m_Tournament.SignupStart = DateTime.UtcNow;
                    this.m_Tournament.Stage = TournamentStage.Signup;
                    this.m_Tournament.Participants.Clear();
                    this.m_Tournament.Pyramid.Levels.Clear();
                    this.m_Tournament.Alert("Hear ye! Hear ye!", "Tournament signup has opened. You can enter by signing up with the registrar.");
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster && this.m_Tournament != null)
            {
                from.CloseGump(typeof(PickRulesetGump));
                from.CloseGump(typeof(RulesetGump));
                from.SendGump(new PickRulesetGump(from, null, this.m_Tournament.Ruleset));
            }
        }

        public TournamentController(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            this.m_Tournament.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Tournament = new Tournament(reader);
                        break;
                    }
            }

            m_Instances.Add(this);
        }

        public override void OnDelete()
        {
            base.OnDelete();

            m_Instances.Remove(this);
        }
    }

    public enum TournyType
    {
        Standard,
        FreeForAll,
        RandomTeam,
        RedVsBlue
    }

    [PropertyObject]
    public class Tournament
    {
        private int m_ParticipantsPerMatch;
        private int m_PlayersPerParticipant;
        private int m_LevelRequirement;
        private TournyPyramid m_Pyramid;
        private Ruleset m_Ruleset;

        private ArrayList m_Arenas;
        private ArrayList m_Participants;
        private ArrayList m_Undefeated;

        private TimeSpan m_SignupPeriod;
        private DateTime m_SignupStart;

        private TournamentStage m_Stage;

        private GroupingType m_GroupType;
        private TieType m_TieType;
        private TimeSpan m_SuddenDeath;

        private TournyType m_TournyType;

        private int m_SuddenDeathRounds;

        private EventController m_EventController;

        public bool IsNotoRestricted
        {
            get
            {
                return (this.m_TournyType != TournyType.Standard);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public EventController EventController
        {
            get
            {
                return this.m_EventController;
            }
            set
            {
                this.m_EventController = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SuddenDeathRounds
        {
            get
            {
                return this.m_SuddenDeathRounds;
            }
            set
            {
                this.m_SuddenDeathRounds = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TournyType TournyType
        {
            get
            {
                return this.m_TournyType;
            }
            set
            {
                this.m_TournyType = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GroupingType GroupType
        {
            get
            {
                return this.m_GroupType;
            }
            set
            {
                this.m_GroupType = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TieType TieType
        {
            get
            {
                return this.m_TieType;
            }
            set
            {
                this.m_TieType = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SuddenDeath
        {
            get
            {
                return this.m_SuddenDeath;
            }
            set
            {
                this.m_SuddenDeath = value;
            }
        }

        public Ruleset Ruleset
        {
            get
            {
                return this.m_Ruleset;
            }
            set
            {
                this.m_Ruleset = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ParticipantsPerMatch
        {
            get
            {
                return this.m_ParticipantsPerMatch;
            }
            set
            {
                if (value < 2)
                    value = 2;
                else if (value > 10)
                    value = 10;
                this.m_ParticipantsPerMatch = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PlayersPerParticipant
        {
            get
            {
                return this.m_PlayersPerParticipant;
            }
            set
            {
                if (value < 1)
                    value = 1;
                else if (value > 10)
                    value = 10;
                this.m_PlayersPerParticipant = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LevelRequirement
        {
            get
            {
                return this.m_LevelRequirement;
            }
            set
            {
                this.m_LevelRequirement = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SignupPeriod
        {
            get
            {
                return this.m_SignupPeriod;
            }
            set
            {
                this.m_SignupPeriod = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SignupStart
        {
            get
            {
                return this.m_SignupStart;
            }
            set
            {
                this.m_SignupStart = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentStage CurrentStage
        {
            get
            {
                return this.m_Stage;
            }
        }

        public TournamentStage Stage
        {
            get
            {
                return this.m_Stage;
            }
            set
            {
                this.m_Stage = value;
            }
        }

        public TournyPyramid Pyramid
        {
            get
            {
                return this.m_Pyramid;
            }
            set
            {
                this.m_Pyramid = value;
            }
        }

        public ArrayList Arenas
        {
            get
            {
                return this.m_Arenas;
            }
            set
            {
                this.m_Arenas = value;
            }
        }

        public ArrayList Participants
        {
            get
            {
                return this.m_Participants;
            }
            set
            {
                this.m_Participants = value;
            }
        }

        public ArrayList Undefeated
        {
            get
            {
                return this.m_Undefeated;
            }
            set
            {
                this.m_Undefeated = value;
            }
        }

        public bool HasParticipant(Mobile mob)
        {
            for (int i = 0; i < this.m_Participants.Count; ++i)
            {
                TournyParticipant part = (TournyParticipant)this.m_Participants[i];

                if (part.Players.Contains(mob))
                    return true;
            }

            return false;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)4); // version

            writer.Write((Item)this.m_EventController);

            writer.WriteEncodedInt((int)this.m_SuddenDeathRounds);

            writer.WriteEncodedInt((int)this.m_TournyType);

            writer.WriteEncodedInt((int)this.m_GroupType);
            writer.WriteEncodedInt((int)this.m_TieType);
            writer.Write((TimeSpan)this.m_SuddenDeath);

            writer.WriteEncodedInt((int)this.m_ParticipantsPerMatch);
            writer.WriteEncodedInt((int)this.m_PlayersPerParticipant);
            writer.Write((TimeSpan)this.m_SignupPeriod);
        }

        public Tournament(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 4:
                    {
                        this.m_EventController = reader.ReadItem() as EventController;

                        goto case 3;
                    }
                case 3:
                    {
                        this.m_SuddenDeathRounds = reader.ReadEncodedInt();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_TournyType = (TournyType)reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_GroupType = (GroupingType)reader.ReadEncodedInt();
                        this.m_TieType = (TieType)reader.ReadEncodedInt();
                        this.m_SignupPeriod = reader.ReadTimeSpan();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 3)
                            this.m_SuddenDeathRounds = 3;

                        this.m_ParticipantsPerMatch = reader.ReadEncodedInt();
                        this.m_PlayersPerParticipant = reader.ReadEncodedInt();
                        this.m_SignupPeriod = reader.ReadTimeSpan();
                        this.m_Stage = TournamentStage.Inactive;
                        this.m_Pyramid = new TournyPyramid();
                        this.m_Ruleset = new Ruleset(RulesetLayout.Root);
                        this.m_Ruleset.ApplyDefault(this.m_Ruleset.Layout.Defaults[0]);
                        this.m_Participants = new ArrayList();
                        this.m_Undefeated = new ArrayList();
                        this.m_Arenas = new ArrayList();

                        break;
                    }
            }

            Timer.DelayCall(SliceInterval, SliceInterval, new TimerCallback(Slice));
        }

        public Tournament()
        {
            this.m_ParticipantsPerMatch = 2;
            this.m_PlayersPerParticipant = 1;
            this.m_Pyramid = new TournyPyramid();
            this.m_Ruleset = new Ruleset(RulesetLayout.Root);
            this.m_Ruleset.ApplyDefault(this.m_Ruleset.Layout.Defaults[0]);
            this.m_Participants = new ArrayList();
            this.m_Undefeated = new ArrayList();
            this.m_Arenas = new ArrayList();
            this.m_SignupPeriod = TimeSpan.FromMinutes(10.0);

            Timer.DelayCall(SliceInterval, SliceInterval, new TimerCallback(Slice));
        }

        public void HandleTie(Arena arena, TournyMatch match, ArrayList remaining)
        {
            if (remaining.Count == 1)
                this.HandleWon(arena, match, (TournyParticipant)remaining[0]);

            if (remaining.Count < 2)
                return;

            StringBuilder sb = new StringBuilder();

            sb.Append("The match has ended in a tie ");

            if (remaining.Count == 2)
                sb.Append("between ");
            else
                sb.Append("among ");

            sb.Append(remaining.Count);

            if (((TournyParticipant)remaining[0]).Players.Count == 1)
                sb.Append(" players: ");
            else
                sb.Append(" teams: ");

            bool hasAppended = false;

            for (int j = 0; j < match.Participants.Count; ++j)
            {
                TournyParticipant part = (TournyParticipant)match.Participants[j];

                if (remaining.Contains(part))
                {
                    if (hasAppended)
                        sb.Append(", ");

                    sb.Append(part.NameList);
                    hasAppended = true;
                }
                else
                {
                    this.m_Undefeated.Remove(part);
                }
            }

            sb.Append(". ");

            string whole = (remaining.Count == 2 ? "both" : "all");

            TieType tieType = this.m_TieType;

            if (tieType == TieType.FullElimination && remaining.Count >= this.m_Undefeated.Count)
                tieType = TieType.FullAdvancement;

            switch ( this.m_TieType )
            {
                case TieType.FullAdvancement:
                    {
                        sb.AppendFormat("In accordance with the rules, {0} parties are advanced.", whole);
                        break;
                    }
                case TieType.FullElimination:
                    {
                        for (int j = 0; j < remaining.Count; ++j)
                            this.m_Undefeated.Remove(remaining[j]);

                        sb.AppendFormat("In accordance with the rules, {0} parties are eliminated.", whole);
                        break;
                    }
                case TieType.Random:
                    {
                        TournyParticipant advanced = (TournyParticipant)remaining[Utility.Random(remaining.Count)];

                        for (int i = 0; i < remaining.Count; ++i)
                        {
                            if (remaining[i] != advanced)
                                this.m_Undefeated.Remove(remaining[i]);
                        }

                        if (advanced != null)
                            sb.AppendFormat("In accordance with the rules, {0} {1} advanced.", advanced.NameList, advanced.Players.Count == 1 ? "is" : "are");

                        break;
                    }
                case TieType.Highest:
                    {
                        TournyParticipant advanced = null;

                        for (int i = 0; i < remaining.Count; ++i)
                        {
                            TournyParticipant part = (TournyParticipant)remaining[i];

                            if (advanced == null || part.TotalLadderXP > advanced.TotalLadderXP)
                                advanced = part;
                        }

                        for (int i = 0; i < remaining.Count; ++i)
                        {
                            if (remaining[i] != advanced)
                                this.m_Undefeated.Remove(remaining[i]);
                        }

                        if (advanced != null)
                            sb.AppendFormat("In accordance with the rules, {0} {1} advanced.", advanced.NameList, advanced.Players.Count == 1 ? "is" : "are");

                        break;
                    }
                case TieType.Lowest:
                    {
                        TournyParticipant advanced = null;

                        for (int i = 0; i < remaining.Count; ++i)
                        {
                            TournyParticipant part = (TournyParticipant)remaining[i];

                            if (advanced == null || part.TotalLadderXP < advanced.TotalLadderXP)
                                advanced = part;
                        }

                        for (int i = 0; i < remaining.Count; ++i)
                        {
                            if (remaining[i] != advanced)
                                this.m_Undefeated.Remove(remaining[i]);
                        }

                        if (advanced != null)
                            sb.AppendFormat("In accordance with the rules, {0} {1} advanced.", advanced.NameList, advanced.Players.Count == 1 ? "is" : "are");

                        break;
                    }
            }

            this.Alert(arena, sb.ToString());
        }

        public void OnEliminated(DuelPlayer player)
        {
            Participant part = player.Participant;

            if (!part.Eliminated)
                return;

            if (this.m_TournyType == TournyType.FreeForAll)
            {
                int rem = 0;

                for (int i = 0; i < part.Context.Participants.Count; ++i)
                {
                    Participant check = (Participant)part.Context.Participants[i];

                    if (check != null && !check.Eliminated)
                        ++rem;
                }

                TournyParticipant tp = part.TournyPart;

                if (tp == null)
                    return;

                if (rem == 1)
                    this.GiveAwards(tp.Players, TrophyRank.Silver, this.ComputeCashAward() / 2);
                else if (rem == 2)
                    this.GiveAwards(tp.Players, TrophyRank.Bronze, this.ComputeCashAward() / 4);
            }
        }

        public void HandleWon(Arena arena, TournyMatch match, TournyParticipant winner)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("The match is complete. ");
            sb.Append(winner.NameList);

            if (winner.Players.Count > 1)
                sb.Append(" have bested ");
            else
                sb.Append(" has bested ");

            if (match.Participants.Count > 2)
                sb.AppendFormat("{0} other {1}: ", match.Participants.Count - 1, winner.Players.Count == 1 ? "players" : "teams");

            bool hasAppended = false;

            for (int j = 0; j < match.Participants.Count; ++j)
            {
                TournyParticipant part = (TournyParticipant)match.Participants[j];

                if (part == winner)
                    continue;

                this.m_Undefeated.Remove(part);

                if (hasAppended)
                    sb.Append(", ");

                sb.Append(part.NameList);
                hasAppended = true;
            }

            sb.Append(".");

            if (this.m_TournyType == TournyType.Standard)
                this.Alert(arena, sb.ToString());
        }

        private static readonly TimeSpan SliceInterval = TimeSpan.FromSeconds(12.0);

        private int ComputeCashAward()
        {
            return this.m_Participants.Count * this.m_PlayersPerParticipant * 2500;
        }

        private void GiveAwards()
        {
            switch ( this.m_TournyType )
            {
                case TournyType.FreeForAll:
                    {
                        if (this.m_Pyramid.Levels.Count < 1)
                            break;

                        PyramidLevel top = this.m_Pyramid.Levels[this.m_Pyramid.Levels.Count - 1] as PyramidLevel;

                        if (top.FreeAdvance != null || top.Matches.Count != 1)
                            break;

                        TournyMatch match = top.Matches[0] as TournyMatch;
                        TournyParticipant winner = match.Winner;

                        if (winner != null)
                            this.GiveAwards(winner.Players, TrophyRank.Gold, this.ComputeCashAward());

                        break;
                    }
                case TournyType.Standard:
                    {
                        if (this.m_Pyramid.Levels.Count < 2)
                            break;

                        PyramidLevel top = this.m_Pyramid.Levels[this.m_Pyramid.Levels.Count - 1] as PyramidLevel;

                        if (top.FreeAdvance != null || top.Matches.Count != 1)
                            break;

                        int cash = this.ComputeCashAward();

                        TournyMatch match = top.Matches[0] as TournyMatch;
                        TournyParticipant winner = match.Winner;

                        for (int i = 0; i < match.Participants.Count; ++i)
                        {
                            TournyParticipant part = (TournyParticipant)match.Participants[i];

                            if (part == winner)
                                this.GiveAwards(part.Players, TrophyRank.Gold, cash);
                            else
                                this.GiveAwards(part.Players, TrophyRank.Silver, cash / 2);
                        }

                        PyramidLevel next = this.m_Pyramid.Levels[this.m_Pyramid.Levels.Count - 2] as PyramidLevel;

                        if (next.Matches.Count > 2)
                            break;

                        for (int i = 0; i < next.Matches.Count; ++i)
                        {
                            match = (TournyMatch)next.Matches[i];
                            winner = match.Winner;

                            for (int j = 0; j < match.Participants.Count; ++j)
                            {
                                TournyParticipant part = (TournyParticipant)match.Participants[j];

                                if (part != winner)
                                    this.GiveAwards(part.Players, TrophyRank.Bronze, cash / 4);
                            }
                        }

                        break;
                    }
            }
        }

        private void GiveAwards(ArrayList players, TrophyRank rank, int cash)
        {
            if (players.Count == 0)
                return;

            if (players.Count > 1)
                cash /= (players.Count - 1);

            cash += 500;
            cash /= 1000;
            cash *= 1000;

            StringBuilder sb = new StringBuilder();

            if (this.m_TournyType == TournyType.FreeForAll)
            {
                sb.Append(this.m_Participants.Count * this.m_PlayersPerParticipant);
                sb.Append("-man FFA");
            }
            else if (this.m_TournyType == TournyType.RandomTeam)
            {
                sb.Append(this.m_ParticipantsPerMatch);
                sb.Append("-team");
            }
            else if (this.m_TournyType == TournyType.RedVsBlue)
            {
                sb.Append("Red v Blue");
            }
            else
            {
                for (int i = 0; i < this.m_ParticipantsPerMatch; ++i)
                {
                    if (sb.Length > 0)
                        sb.Append('v');

                    sb.Append(this.m_PlayersPerParticipant);
                }
            }

            if (this.m_EventController != null)
                sb.Append(' ').Append(this.m_EventController.Title);

            sb.Append(" Champion");

            string title = sb.ToString();

            for (int i = 0; i < players.Count; ++i)
            {
                Mobile mob = (Mobile)players[i];

                if (mob == null || mob.Deleted)
                    continue;

                Item item = new Trophy(title, rank);

                if (!mob.PlaceInBackpack(item))
                    mob.BankBox.DropItem(item);

                if (cash > 0)
                {
                    item = new BankCheck(cash);

                    if (!mob.PlaceInBackpack(item))
                        mob.BankBox.DropItem(item);

                    mob.SendMessage("You have been awarded a {0} trophy and {1:N0}gp for your participation in this tournament.", rank.ToString().ToLower(), cash);
                }
                else
                {
                    mob.SendMessage("You have been awarded a {0} trophy for your participation in this tournament.", rank.ToString().ToLower());
                }
            }
        }

        public void Slice()
        {
            if (this.m_Stage == TournamentStage.Signup)
            {
                TimeSpan until = (this.m_SignupStart + this.m_SignupPeriod) - DateTime.UtcNow;

                if (until <= TimeSpan.Zero)
                {
                    for (int i = this.m_Participants.Count - 1; i >= 0; --i)
                    {
                        TournyParticipant part = (TournyParticipant)this.m_Participants[i];
                        bool bad = false;

                        for (int j = 0; j < part.Players.Count; ++j)
                        {
                            Mobile check = (Mobile)part.Players[j];

                            if (check.Deleted || check.Map == null || check.Map == Map.Internal || !check.Alive || Factions.Sigil.ExistsOn(check) || check.Region.IsPartOf(typeof(Regions.Jail)))
                            {
                                bad = true;
                                break;
                            }
                        }

                        if (bad)
                        {
                            for (int j = 0; j < part.Players.Count; ++j)
                                ((Mobile)part.Players[j]).SendMessage("You have been disqualified from the tournament.");

                            this.m_Participants.RemoveAt(i);
                        }
                    }

                    if (this.m_Participants.Count >= 2)
                    {
                        this.m_Stage = TournamentStage.Fighting;

                        this.m_Undefeated.Clear();

                        this.m_Pyramid.Levels.Clear();
                        this.m_Pyramid.AddLevel(this.m_ParticipantsPerMatch, this.m_Participants, this.m_GroupType, this.m_TournyType);

                        PyramidLevel level = (PyramidLevel)this.m_Pyramid.Levels[0];

                        if (level.FreeAdvance != null)
                            this.m_Undefeated.Add(level.FreeAdvance);

                        for (int i = 0; i < level.Matches.Count; ++i)
                        {
                            TournyMatch match = (TournyMatch)level.Matches[i];

                            this.m_Undefeated.AddRange(match.Participants);
                        }

                        this.Alert("Hear ye! Hear ye!", "The tournament will begin shortly.");
                    }
                    else
                    {
                        this.Alert("Is this all?", "Pitiful. Signup extended.");
                        this.m_SignupStart = DateTime.UtcNow;
                    }
                }
                else if (Math.Abs(until.TotalSeconds - TimeSpan.FromMinutes(1.0).TotalSeconds) < (SliceInterval.TotalSeconds / 2))
                {
                    this.Alert("Last call!", "If you wish to enter the tournament, sign up with the registrar now.");
                }
                else if (Math.Abs(until.TotalSeconds - TimeSpan.FromMinutes(5.0).TotalSeconds) < (SliceInterval.TotalSeconds / 2))
                {
                    this.Alert("The tournament will begin in 5 minutes.", "Sign up now before it's too late.");
                }
            }
            else if (this.m_Stage == TournamentStage.Fighting)
            {
                if (this.m_Undefeated.Count == 1)
                {
                    TournyParticipant winner = (TournyParticipant)this.m_Undefeated[0];

                    try
                    {
                        if (this.m_EventController != null)
                            this.Alert("The tournament has completed!", String.Format("Team {0} has won!", this.m_EventController.GetTeamName(((TournyMatch)((PyramidLevel)this.m_Pyramid.Levels[0]).Matches[0]).Participants.IndexOf(winner))));
                        else if (this.m_TournyType == TournyType.RandomTeam)
                            this.Alert("The tournament has completed!", String.Format("Team {0} has won!", ((TournyMatch)((PyramidLevel)this.m_Pyramid.Levels[0]).Matches[0]).Participants.IndexOf(winner) + 1));
                        else if (this.m_TournyType == TournyType.RedVsBlue)
                            this.Alert("The tournament has completed!", String.Format("Team {0} has won!", ((TournyMatch)((PyramidLevel)this.m_Pyramid.Levels[0]).Matches[0]).Participants.IndexOf(winner) == 0 ? "Red" : "Blue"));
                        else
                            this.Alert("The tournament has completed!", String.Format("{0} {1} the champion{2}.", winner.NameList, winner.Players.Count > 1 ? "are" : "is", winner.Players.Count == 1 ? "" : "s"));
                    }
                    catch
                    {
                    }

                    this.GiveAwards();

                    this.m_Stage = TournamentStage.Inactive;
                    this.m_Undefeated.Clear();
                }
                else if (this.m_Pyramid.Levels.Count > 0)
                {
                    PyramidLevel activeLevel = (PyramidLevel)this.m_Pyramid.Levels[this.m_Pyramid.Levels.Count - 1];
                    bool stillGoing = false;

                    for (int i = 0; i < activeLevel.Matches.Count; ++i)
                    {
                        TournyMatch match = (TournyMatch)activeLevel.Matches[i];

                        if (match.Winner == null)
                        {
                            stillGoing = true;

                            if (!match.InProgress)
                            {
                                for (int j = 0; j < this.m_Arenas.Count; ++j)
                                {
                                    Arena arena = (Arena)this.m_Arenas[j];

                                    if (!arena.IsOccupied)
                                    {
                                        match.Start(arena, this);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (!stillGoing)
                    {
                        for (int i = this.m_Undefeated.Count - 1; i >= 0; --i)
                        {
                            TournyParticipant part = (TournyParticipant)this.m_Undefeated[i];
                            bool bad = false;

                            for (int j = 0; j < part.Players.Count; ++j)
                            {
                                Mobile check = (Mobile)part.Players[j];

                                if (check.Deleted || check.Map == null || check.Map == Map.Internal || !check.Alive || Factions.Sigil.ExistsOn(check) || check.Region.IsPartOf(typeof(Regions.Jail)))
                                {
                                    bad = true;
                                    break;
                                }
                            }

                            if (bad)
                            {
                                for (int j = 0; j < part.Players.Count; ++j)
                                    ((Mobile)part.Players[j]).SendMessage("You have been disqualified from the tournament.");

                                this.m_Undefeated.RemoveAt(i);

                                if (this.m_Undefeated.Count == 1)
                                {
                                    TournyParticipant winner = (TournyParticipant)this.m_Undefeated[0];

                                    try
                                    {
                                        if (this.m_EventController != null)
                                            this.Alert("The tournament has completed!", String.Format("Team {0} has won", this.m_EventController.GetTeamName(((TournyMatch)((PyramidLevel)this.m_Pyramid.Levels[0]).Matches[0]).Participants.IndexOf(winner))));
                                        else if (this.m_TournyType == TournyType.RandomTeam)
                                            this.Alert("The tournament has completed!", String.Format("Team {0} has won!", ((TournyMatch)((PyramidLevel)this.m_Pyramid.Levels[0]).Matches[0]).Participants.IndexOf(winner) + 1));
                                        else if (this.m_TournyType == TournyType.RedVsBlue)
                                            this.Alert("The tournament has completed!", String.Format("Team {0} has won!", ((TournyMatch)((PyramidLevel)this.m_Pyramid.Levels[0]).Matches[0]).Participants.IndexOf(winner) == 0 ? "Red" : "Blue"));
                                        else
                                            this.Alert("The tournament has completed!", String.Format("{0} {1} the champion{2}.", winner.NameList, winner.Players.Count > 1 ? "are" : "is", winner.Players.Count == 1 ? "" : "s"));
                                    }
                                    catch
                                    {
                                    }

                                    this.GiveAwards();

                                    this.m_Stage = TournamentStage.Inactive;
                                    this.m_Undefeated.Clear();
                                    break;
                                }
                            }
                        }

                        if (this.m_Undefeated.Count > 1)
                            this.m_Pyramid.AddLevel(this.m_ParticipantsPerMatch, this.m_Undefeated, this.m_GroupType, this.m_TournyType);
                    }
                }
            }
        }

        public void Alert(params string[] alerts)
        {
            for (int i = 0; i < this.m_Arenas.Count; ++i)
                this.Alert((Arena)this.m_Arenas[i], alerts);
        }

        public void Alert(Arena arena, params string[] alerts)
        {
            if (arena != null && arena.Announcer != null)
            {
                for (int j = 0; j < alerts.Length; ++j)
                    Timer.DelayCall(TimeSpan.FromSeconds(Math.Max(j - 0.5, 0.0)), new TimerStateCallback(Alert_Callback), new object[] { arena.Announcer, alerts[j] });
            }
        }

        private void Alert_Callback(object state)
        {
            object[] states = (object[])state;

            if (states[0] != null)
                ((Mobile)states[0]).PublicOverheadMessage(MessageType.Regular, 0x35, false, (string)states[1]);
        }
    }

    public class TournyPyramid
    {
        private ArrayList m_Levels;

        public ArrayList Levels
        {
            get
            {
                return this.m_Levels;
            }
            set
            {
                this.m_Levels = value;
            }
        }

        public TournyPyramid()
        {
            this.m_Levels = new ArrayList();
        }

        public void AddLevel(int partsPerMatch, ArrayList participants, GroupingType groupType, TournyType tournyType)
        {
            ArrayList copy = new ArrayList(participants);

            if (groupType == GroupingType.Nearest || groupType == GroupingType.HighVsLow)
                copy.Sort();

            PyramidLevel level = new PyramidLevel();

            switch ( tournyType )
            {
                case TournyType.RedVsBlue:
                    {
                        TournyParticipant[] parts = new TournyParticipant[2];

                        for (int i = 0; i < parts.Length; ++i)
                            parts[i] = new TournyParticipant(new ArrayList());

                        for (int i = 0; i < copy.Count; ++i)
                        {
                            ArrayList players = ((TournyParticipant)copy[i]).Players;

                            for (int j = 0; j < players.Count; ++j)
                            {
                                Mobile mob = (Mobile)players[j];

                                if (mob.Kills >= 5)
                                    parts[0].Players.Add(mob);
                                else
                                    parts[1].Players.Add(mob);
                            }
                        }

                        level.Matches.Add(new TournyMatch(new ArrayList(parts)));
                        break;
                    }
                case TournyType.RandomTeam:
                    {
                        TournyParticipant[] parts = new TournyParticipant[partsPerMatch];

                        for (int i = 0; i < partsPerMatch; ++i)
                            parts[i] = new TournyParticipant(new ArrayList());

                        for (int i = 0; i < copy.Count; ++i)
                            parts[i % parts.Length].Players.AddRange(((TournyParticipant)copy[i]).Players);

                        level.Matches.Add(new TournyMatch(new ArrayList(parts)));
                        break;
                    }
                case TournyType.FreeForAll:
                    {
                        level.Matches.Add(new TournyMatch(copy));
                        break;
                    }
                case TournyType.Standard:
                    {
                        if (partsPerMatch == 2 && (participants.Count % 2) == 1)
                        {
                            int lowAdvances = int.MaxValue;

                            for (int i = 0; i < participants.Count; ++i)
                            {
                                TournyParticipant p = (TournyParticipant)participants[i];

                                if (p.FreeAdvances < lowAdvances)
                                    lowAdvances = p.FreeAdvances;
                            }

                            ArrayList toAdvance = new ArrayList();

                            for (int i = 0; i < participants.Count; ++i)
                            {
                                TournyParticipant p = (TournyParticipant)participants[i];

                                if (p.FreeAdvances == lowAdvances)
                                    toAdvance.Add(p);
                            }

                            if (toAdvance.Count == 0)
                                toAdvance = copy; // sanity

                            int idx = Utility.Random(toAdvance.Count);

                            ((TournyParticipant)toAdvance[idx]).AddLog("Advanced automatically due to an odd number of challengers.");
                            level.FreeAdvance = (TournyParticipant)toAdvance[idx];
                            ++level.FreeAdvance.FreeAdvances;
                            copy.Remove(toAdvance[idx]);
                        }

                        while (copy.Count >= partsPerMatch)
                        {
                            ArrayList thisMatch = new ArrayList();

                            for (int i = 0; i < partsPerMatch; ++i)
                            {
                                int idx = 0;

                                switch ( groupType )
                                {
                                    case GroupingType.HighVsLow:
                                        idx = (i * (copy.Count - 1)) / (partsPerMatch - 1);
                                        break;
                                    case GroupingType.Nearest:
                                        idx = 0;
                                        break;
                                    case GroupingType.Random:
                                        idx = Utility.Random(copy.Count);
                                        break;
                                }

                                thisMatch.Add(copy[idx]);
                                copy.RemoveAt(idx);
                            }

                            level.Matches.Add(new TournyMatch(thisMatch));
                        }

                        if (copy.Count > 1)
                            level.Matches.Add(new TournyMatch(copy));

                        break;
                    }
            }

            this.m_Levels.Add(level);
        }
    }

    public class PyramidLevel
    {
        private ArrayList m_Matches;
        private TournyParticipant m_FreeAdvance;

        public ArrayList Matches
        {
            get
            {
                return this.m_Matches;
            }
            set
            {
                this.m_Matches = value;
            }
        }

        public TournyParticipant FreeAdvance
        {
            get
            {
                return this.m_FreeAdvance;
            }
            set
            {
                this.m_FreeAdvance = value;
            }
        }

        public PyramidLevel()
        {
            this.m_Matches = new ArrayList();
        }
    }

    public class TournyMatch
    {
        private ArrayList m_Participants;
        private TournyParticipant m_Winner;
        private DuelContext m_Context;

        public ArrayList Participants
        {
            get
            {
                return this.m_Participants;
            }
            set
            {
                this.m_Participants = value;
            }
        }

        public TournyParticipant Winner
        {
            get
            {
                return this.m_Winner;
            }
            set
            {
                this.m_Winner = value;
            }
        }

        public DuelContext Context
        {
            get
            {
                return this.m_Context;
            }
            set
            {
                this.m_Context = value;
            }
        }

        public bool InProgress
        {
            get
            {
                return (this.m_Context != null && this.m_Context.Registered);
            }
        }

        public void Start(Arena arena, Tournament tourny)
        {
            TournyParticipant first = (TournyParticipant)this.m_Participants[0];

            DuelContext dc = new DuelContext((Mobile)first.Players[0], tourny.Ruleset.Layout, false);
            dc.Ruleset.Options.SetAll(false);
            dc.Ruleset.Options.Or(tourny.Ruleset.Options);

            for (int i = 0; i < this.m_Participants.Count; ++i)
            {
                TournyParticipant tournyPart = (TournyParticipant)this.m_Participants[i];
                Participant duelPart = new Participant(dc, tournyPart.Players.Count);

                duelPart.TournyPart = tournyPart;

                for (int j = 0; j < tournyPart.Players.Count; ++j)
                    duelPart.Add((Mobile)tournyPart.Players[j]);

                for (int j = 0; j < duelPart.Players.Length; ++j)
                {
                    if (duelPart.Players[j] != null)
                        duelPart.Players[j].Ready = true;
                }

                dc.Participants.Add(duelPart);
            }

            if (tourny.EventController != null)
                dc.m_EventGame = tourny.EventController.Construct(dc);

            dc.m_Tournament = tourny;
            dc.m_Match = this;

            dc.m_OverrideArena = arena;

            if (tourny.SuddenDeath > TimeSpan.Zero && (tourny.SuddenDeathRounds == 0 || tourny.Pyramid.Levels.Count <= tourny.SuddenDeathRounds))
                dc.StartSuddenDeath(tourny.SuddenDeath);

            dc.SendReadyGump(0);

            if (dc.StartedBeginCountdown)
            {
                this.m_Context = dc;

                for (int i = 0; i < this.m_Participants.Count; ++i)
                {
                    TournyParticipant p = (TournyParticipant)this.m_Participants[i];

                    for (int j = 0; j < p.Players.Count; ++j)
                    {
                        Mobile mob = (Mobile)p.Players[j];

                        foreach (Mobile view in mob.GetMobilesInRange(18))
                        {
                            if (!mob.CanSee(view))
                                mob.Send(view.RemovePacket);
                        }

                        mob.LocalOverheadMessage(MessageType.Emote, 0x3B2, false, "* Your mind focuses intently on the fight and all other distractions fade away *");
                    }
                }
            }
            else
            {
                dc.Unregister();
                dc.StopCountdown();
            }
        }

        public TournyMatch(ArrayList participants)
        {
            this.m_Participants = participants;

            for (int i = 0; i < participants.Count; ++i)
            {
                TournyParticipant part = (TournyParticipant)participants[i];

                StringBuilder sb = new StringBuilder();

                sb.Append("Matched in a duel against ");

                if (participants.Count > 2)
                    sb.AppendFormat("{0} other {1}: ", participants.Count - 1, part.Players.Count == 1 ? "players" : "teams");

                bool hasAppended = false;

                for (int j = 0; j < participants.Count; ++j)
                {
                    if (i == j)
                        continue;

                    if (hasAppended)
                        sb.Append(", ");

                    sb.Append(((TournyParticipant)participants[j]).NameList);
                    hasAppended = true;
                }

                sb.Append(".");

                part.AddLog(sb.ToString());
            }
        }
    }

    public class TournyParticipant : IComparable
    {
        private ArrayList m_Players;
        private ArrayList m_Log;
        private int m_FreeAdvances;

        public ArrayList Players
        {
            get
            {
                return this.m_Players;
            }
            set
            {
                this.m_Players = value;
            }
        }

        public ArrayList Log
        {
            get
            {
                return this.m_Log;
            }
            set
            {
                this.m_Log = value;
            }
        }

        public int FreeAdvances
        {
            get
            {
                return this.m_FreeAdvances;
            }
            set
            {
                this.m_FreeAdvances = value;
            }
        }

        public int TotalLadderXP
        {
            get
            {
                Ladder ladder = Ladder.Instance;

                if (ladder == null)
                    return 0;

                int total = 0;

                for (int i = 0; i < this.m_Players.Count; ++i)
                {
                    Mobile mob = (Mobile)this.m_Players[i];
                    LadderEntry entry = ladder.Find(mob);

                    if (entry != null)
                        total += entry.Experience;
                }

                return total;
            }
        }

        public string NameList
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < this.m_Players.Count; ++i)
                {
                    if (this.m_Players[i] == null)
                        continue;

                    Mobile mob = (Mobile)this.m_Players[i];

                    if (sb.Length > 0)
                    {
                        if (this.m_Players.Count == 2)
                            sb.Append(" and ");
                        else if ((i + 1) < this.m_Players.Count)
                            sb.Append(", ");
                        else
                            sb.Append(", and ");
                    }

                    sb.Append(mob.Name);
                }

                if (sb.Length == 0)
                    return "Empty";

                return sb.ToString();
            }
        }

        public void AddLog(string text)
        {
            this.m_Log.Add(text);
        }

        public void AddLog(string format, params object[] args)
        {
            this.AddLog(String.Format(format, args));
        }

        public void WonMatch(TournyMatch match)
        {
            this.AddLog("Match won.");
        }

        public void LostMatch(TournyMatch match)
        {
            this.AddLog("Match lost.");
        }

        public TournyParticipant(Mobile owner)
        {
            this.m_Log = new ArrayList();
            this.m_Players = new ArrayList();
            this.m_Players.Add(owner);
        }

        public TournyParticipant(ArrayList players)
        {
            this.m_Log = new ArrayList();
            this.m_Players = players;
        }

        public int CompareTo(object obj)
        {
            TournyParticipant p = (TournyParticipant)obj;

            return p.TotalLadderXP - this.TotalLadderXP;
        }
    }

    public enum TournyBracketGumpType
    {
        Index,
        Rules_Info,
        Participant_List,
        Participant_Info,
        Round_List,
        Round_Info,
        Match_Info,
        Player_Info
    }

    public class TournamentBracketGump : Gump
    {
        private readonly Mobile m_From;
        private readonly Tournament m_Tournament;
        private readonly TournyBracketGumpType m_Type;
        private readonly ArrayList m_List;
        private readonly int m_Page;
        private int m_PerPage;
        private readonly object m_Object;

        private const int BlackColor32 = 0x000008;
        private const int LabelColor32 = 0xFFFFFF;

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void AddBorderedText(int x, int y, int width, int height, string text, int color, int borderColor)
        {
            this.AddColoredText(x - 1, y - 1, width, height, text, borderColor);
            this.AddColoredText(x - 1, y + 1, width, height, text, borderColor);
            this.AddColoredText(x + 1, y - 1, width, height, text, borderColor);
            this.AddColoredText(x + 1, y + 1, width, height, text, borderColor);
            this.AddColoredText(x, y, width, height, text, color);
        }

        private void AddColoredText(int x, int y, int width, int height, string text, int color)
        {
            if (color == 0)
                this.AddHtml(x, y, width, height, text, false, false);
            else
                this.AddHtml(x, y, width, height, this.Color(text, color), false, false);
        }

        public void AddRightArrow(int x, int y, int bid, string text)
        {
            this.AddButton(x, y, 0x15E1, 0x15E5, bid, GumpButtonType.Reply, 0);

            if (text != null)
                this.AddHtml(x + 20, y - 1, 230, 20, text, false, false);
        }

        public void AddRightArrow(int x, int y, int bid)
        {
            this.AddRightArrow(x, y, bid, null);
        }

        public void AddLeftArrow(int x, int y, int bid, string text)
        {
            this.AddButton(x, y, 0x15E3, 0x15E7, bid, GumpButtonType.Reply, 0);

            if (text != null)
                this.AddHtml(x + 20, y - 1, 230, 20, text, false, false);
        }

        public void AddLeftArrow(int x, int y, int bid)
        {
            this.AddLeftArrow(x, y, bid, null);
        }

        public int ToButtonID(int type, int index)
        {
            return 1 + (index * 7) + type;
        }

        public bool FromButtonID(int bid, out int type, out int index)
        {
            type = (bid - 1) % 7;
            index = (bid - 1) / 7;
            return (bid >= 1);
        }

        public void StartPage(out int index, out int count, out int y, int perPage)
        {
            this.m_PerPage = perPage;

            index = Math.Max(this.m_Page * perPage, 0);
            count = Math.Max(Math.Min(this.m_List.Count - index, perPage), 0);

            y = 53 + ((12 - perPage) * 18);

            if (this.m_Page > 0)
                this.AddLeftArrow(242, 35, this.ToButtonID(1, 0));

            if ((this.m_Page + 1) * perPage < this.m_List.Count)
                this.AddRightArrow(260, 35, this.ToButtonID(1, 1));
        }

        public TournamentBracketGump(Mobile from, Tournament tourny, TournyBracketGumpType type, ArrayList list, int page, object obj)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Tournament = tourny;
            this.m_Type = type;
            this.m_List = list;
            this.m_Page = page;
            this.m_Object = obj;
            this.m_PerPage = 12;

            switch ( type )
            {
                case TournyBracketGumpType.Index:
                    {
                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 300, 9380);

                        StringBuilder sb = new StringBuilder();

                        if (tourny.TournyType == TournyType.FreeForAll)
                        {
                            sb.Append("FFA");
                        }
                        else if (tourny.TournyType == TournyType.RandomTeam)
                        {
                            sb.Append("Team");
                        }
                        else if (tourny.TournyType == TournyType.RedVsBlue)
                        {
                            sb.Append("Red v Blue");
                        }
                        else
                        {
                            for (int i = 0; i < tourny.ParticipantsPerMatch; ++i)
                            {
                                if (sb.Length > 0)
                                    sb.Append('v');

                                sb.Append(tourny.PlayersPerParticipant);
                            }
                        }

                        if (tourny.EventController != null)
                            sb.Append(' ').Append(tourny.EventController.Title);

                        sb.Append(" Tournament Bracket");

                        this.AddHtml(25, 35, 250, 20, this.Center(sb.ToString()), false, false);

                        this.AddRightArrow(25, 53, this.ToButtonID(0, 4), "Rules");
                        this.AddRightArrow(25, 71, this.ToButtonID(0, 1), "Participants");

                        if (this.m_Tournament.Stage == TournamentStage.Signup)
                        {
                            TimeSpan until = (this.m_Tournament.SignupStart + this.m_Tournament.SignupPeriod) - DateTime.UtcNow;
                            string text;
                            int secs = (int)until.TotalSeconds;

                            if (secs > 0)
                            {
                                int mins = secs / 60;
                                secs %= 60;

                                if (mins > 0 && secs > 0)
                                    text = String.Format("The tournament will begin in {0} minute{1} and {2} second{3}.", mins, mins == 1 ? "" : "s", secs, secs == 1 ? "" : "s");
                                else if (mins > 0)
                                    text = String.Format("The tournament will begin in {0} minute{1}.", mins, mins == 1 ? "" : "s");
                                else if (secs > 0)
                                    text = String.Format("The tournament will begin in {0} second{1}.", secs, secs == 1 ? "" : "s");
                                else
                                    text = "The tournament will begin shortly.";
                            }
                            else
                            {
                                text = "The tournament will begin shortly.";
                            }

                            this.AddHtml(25, 92, 250, 40, text, false, false);
                        }
                        else
                        {
                            this.AddRightArrow(25, 89, this.ToButtonID(0, 2), "Rounds");
                        }

                        break;
                    }
                case TournyBracketGumpType.Rules_Info:
                    {
                        Ruleset ruleset = tourny.Ruleset;
                        Ruleset basedef = ruleset.Base;

                        BitArray defs;

                        if (ruleset.Flavors.Count > 0)
                        {
                            defs = new BitArray(basedef.Options);

                            for (int i = 0; i < ruleset.Flavors.Count; ++i)
                                defs.Or(((Ruleset)ruleset.Flavors[i]).Options);
                        }
                        else
                        {
                            defs = basedef.Options;
                        }

                        int changes = 0;

                        BitArray opts = ruleset.Options;

                        for (int i = 0; i < opts.Length; ++i)
                        {
                            if (defs[i] != opts[i])
                                ++changes;
                        }

                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 60 + 18 + 20 + 20 + 20 + 8 + 20 + (ruleset.Flavors.Count * 18) + 4 + 20 + (changes * 22) + 6, 9380);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 0));
                        this.AddHtml(25, 35, 250, 20, this.Center("Rules"), false, false);

                        int y = 53;

                        string groupText = null;

                        switch ( tourny.GroupType )
                        {
                            case GroupingType.HighVsLow:
                                groupText = "High vs Low";
                                break;
                            case GroupingType.Nearest:
                                groupText = "Closest opponent";
                                break;
                            case GroupingType.Random:
                                groupText = "Random";
                                break;
                        }

                        this.AddHtml(35, y, 190, 20, String.Format("Grouping: {0}", groupText), false, false);
                        y += 20;

                        string tieText = null;

                        switch ( tourny.TieType )
                        {
                            case TieType.Random:
                                tieText = "Random";
                                break;
                            case TieType.Highest:
                                tieText = "Highest advances";
                                break;
                            case TieType.Lowest:
                                tieText = "Lowest advances";
                                break;
                            case TieType.FullAdvancement:
                                tieText = (tourny.ParticipantsPerMatch == 2 ? "Both advance" : "Everyone advances");
                                break;
                            case TieType.FullElimination:
                                tieText = (tourny.ParticipantsPerMatch == 2 ? "Both eliminated" : "Everyone eliminated");
                                break;
                        }

                        this.AddHtml(35, y, 190, 20, String.Format("Tiebreaker: {0}", tieText), false, false);
                        y += 20;

                        string sdText = "Off";

                        if (tourny.SuddenDeath > TimeSpan.Zero)
                        {
                            sdText = String.Format("{0}:{1:D2}", (int)tourny.SuddenDeath.TotalMinutes, tourny.SuddenDeath.Seconds);

                            if (tourny.SuddenDeathRounds > 0)
                                sdText = String.Format("{0} (first {1} rounds)", sdText, tourny.SuddenDeathRounds);
                            else
                                sdText = String.Format("{0} (all rounds)", sdText);
                        }

                        this.AddHtml(35, y, 240, 20, String.Format("Sudden Death: {0}", sdText), false, false);
                        y += 20;

                        y += 8;

                        this.AddHtml(35, y, 190, 20, String.Format("Ruleset: {0}", basedef.Title), false, false);
                        y += 20;

                        for (int i = 0; i < ruleset.Flavors.Count; ++i, y += 18)
                            this.AddHtml(35, y, 190, 20, String.Format(" + {0}", ((Ruleset)ruleset.Flavors[i]).Title), false, false);

                        y += 4;

                        if (changes > 0)
                        {
                            this.AddHtml(35, y, 190, 20, "Modifications:", false, false);
                            y += 20;

                            for (int i = 0; i < opts.Length; ++i)
                            {
                                if (defs[i] != opts[i])
                                {
                                    string name = ruleset.Layout.FindByIndex(i);

                                    if (name != null) // sanity
                                    {
                                        this.AddImage(35, y, opts[i] ? 0xD3 : 0xD2);
                                        this.AddHtml(60, y, 165, 22, name, false, false);
                                    }

                                    y += 22;
                                }
                            }
                        }
                        else
                        {
                            this.AddHtml(35, y, 190, 20, "Modifications: None", false, false);
                            y += 20;
                        }

                        break;
                    }
                case TournyBracketGumpType.Participant_List:
                    {
                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 300, 9380);

                        if (this.m_List == null)
                            this.m_List = new ArrayList(tourny.Participants);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 0));
                        this.AddHtml(25, 35, 250, 20, this.Center(String.Format("{0} Participant{1}", this.m_List.Count, this.m_List.Count == 1 ? "" : "s")), false, false);

                        int index, count, y;
                        this.StartPage(out index, out count, out y, 12);

                        for (int i = 0; i < count; ++i, y += 18)
                        {
                            TournyParticipant part = (TournyParticipant)this.m_List[index + i];
                            string name = part.NameList;

                            if (this.m_Tournament.TournyType != TournyType.Standard && part.Players.Count == 1)
                            {
                                PlayerMobile pm = part.Players[0] as PlayerMobile;

                                if (pm != null && pm.DuelPlayer != null)
                                    name = this.Color(name, pm.DuelPlayer.Eliminated ? 0x6633333 : 0x336666);
                            }

                            this.AddRightArrow(25, y, this.ToButtonID(2, index + i), name);
                        }

                        break;
                    }
                case TournyBracketGumpType.Participant_Info:
                    {
                        TournyParticipant part = obj as TournyParticipant;

                        if (part == null)
                            break;

                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 60 + 18 + 20 + (part.Players.Count * 18) + 20 + 20 + 160, 9380);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 1));
                        this.AddHtml(25, 35, 250, 20, this.Center("Participants"), false, false);

                        int y = 53;

                        this.AddHtml(25, y, 200, 20, part.Players.Count == 1 ? "Players" : "Team", false, false);
                        y += 20;

                        for (int i = 0; i < part.Players.Count; ++i)
                        {
                            Mobile mob = (Mobile)part.Players[i];
                            string name = mob.Name;

                            if (this.m_Tournament.TournyType != TournyType.Standard)
                            {
                                PlayerMobile pm = mob as PlayerMobile;

                                if (pm != null && pm.DuelPlayer != null)
                                    name = this.Color(name, pm.DuelPlayer.Eliminated ? 0x6633333 : 0x336666);
                            }

                            this.AddRightArrow(35, y, this.ToButtonID(4, i), name);
                            y += 18;
                        }

                        this.AddHtml(25, y, 200, 20, String.Format("Free Advances: {0}", part.FreeAdvances == 0 ? "None" : part.FreeAdvances.ToString()), false, false);
                        y += 20;

                        this.AddHtml(25, y, 200, 20, "Log:", false, false);
                        y += 20;

                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < part.Log.Count; ++i)
                        {
                            if (sb.Length > 0)
                                sb.Append("<br>");

                            sb.Append(part.Log[i]);
                        }

                        if (sb.Length == 0)
                            sb.Append("Nothing logged yet.");

                        this.AddHtml(25, y, 250, 150, this.Color(sb.ToString(), BlackColor32), false, true);

                        break;
                    }
                case TournyBracketGumpType.Player_Info:
                    {
                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 300, 9380);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 3));
                        this.AddHtml(25, 35, 250, 20, this.Center("Participants"), false, false);

                        Mobile mob = obj as Mobile;

                        if (mob == null)
                            break;

                        Ladder ladder = Ladder.Instance;
                        LadderEntry entry = (ladder == null ? null : ladder.Find(mob));

                        this.AddHtml(25, 53, 250, 20, String.Format("Name: {0}", mob.Name), false, false);
                        this.AddHtml(25, 73, 250, 20, String.Format("Guild: {0}", mob.Guild == null ? "None" : mob.Guild.Name + " [" + mob.Guild.Abbreviation + "]"), false, false);
                        this.AddHtml(25, 93, 250, 20, String.Format("Rank: {0}", entry == null ? "N/A" : LadderGump.Rank(entry.Index + 1)), false, false);
                        this.AddHtml(25, 113, 250, 20, String.Format("Level: {0}", entry == null ? 0 : Ladder.GetLevel(entry.Experience)), false, false);
                        this.AddHtml(25, 133, 250, 20, String.Format("Wins: {0:N0}", entry == null ? 0 : entry.Wins), false, false);
                        this.AddHtml(25, 153, 250, 20, String.Format("Losses: {0:N0}", entry == null ? 0 : entry.Losses), false, false);

                        break;
                    }
                case TournyBracketGumpType.Round_List:
                    {
                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 300, 9380);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 0));
                        this.AddHtml(25, 35, 250, 20, this.Center("Rounds"), false, false);

                        if (this.m_List == null)
                            this.m_List = new ArrayList(tourny.Pyramid.Levels);

                        int index, count, y;
                        this.StartPage(out index, out count, out y, 12);

                        for (int i = 0; i < count; ++i, y += 18)
                        {
                            PyramidLevel level = (PyramidLevel)this.m_List[index + i];

                            this.AddRightArrow(25, y, this.ToButtonID(3, index + i), "Round #" + (index + i + 1));
                        }

                        break;
                    }
                case TournyBracketGumpType.Round_Info:
                    {
                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 300, 9380);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 2));
                        this.AddHtml(25, 35, 250, 20, this.Center("Rounds"), false, false);

                        PyramidLevel level = this.m_Object as PyramidLevel;

                        if (level == null)
                            break;

                        if (this.m_List == null)
                            this.m_List = new ArrayList(level.Matches);

                        this.AddRightArrow(25, 53, this.ToButtonID(5, 0), String.Format("Free Advance: {0}", level.FreeAdvance == null ? "None" : level.FreeAdvance.NameList));

                        this.AddHtml(25, 73, 200, 20, String.Format("{0} Match{1}", this.m_List.Count, this.m_List.Count == 1 ? "" : "es"), false, false);

                        int index, count, y;
                        this.StartPage(out index, out count, out y, 10);

                        for (int i = 0; i < count; ++i, y += 18)
                        {
                            TournyMatch match = (TournyMatch)this.m_List[index + i];

                            int color = -1;

                            if (match.InProgress)
                                color = 0x336666;
                            else if (match.Context != null && match.Winner == null)
                                color = 0x666666;

                            StringBuilder sb = new StringBuilder();

                            if (this.m_Tournament.TournyType == TournyType.Standard)
                            {
                                for (int j = 0; j < match.Participants.Count; ++j)
                                {
                                    if (sb.Length > 0)
                                        sb.Append(" vs ");

                                    TournyParticipant part = (TournyParticipant)match.Participants[j];
                                    string txt = part.NameList;

                                    if (color == -1 && match.Context != null && match.Winner == part)
                                        txt = this.Color(txt, 0x336633);
                                    else if (color == -1 && match.Context != null)
                                        txt = this.Color(txt, 0x663333);

                                    sb.Append(txt);
                                }
                            }
                            else if (this.m_Tournament.EventController != null || this.m_Tournament.TournyType == TournyType.RandomTeam || this.m_Tournament.TournyType == TournyType.RedVsBlue)
                            {
                                for (int j = 0; j < match.Participants.Count; ++j)
                                {
                                    if (sb.Length > 0)
                                        sb.Append(" vs ");

                                    TournyParticipant part = (TournyParticipant)match.Participants[j];
                                    string txt;

                                    if (this.m_Tournament.EventController != null)
                                        txt = String.Format("Team {0} ({1})", this.m_Tournament.EventController.GetTeamName(j), part.Players.Count);
                                    else if (this.m_Tournament.TournyType == TournyType.RandomTeam)
                                        txt = String.Format("Team {0} ({1})", j + 1, part.Players.Count);
                                    else
                                        txt = String.Format("Team {0} ({1})", j == 0 ? "Red" : "Blue", part.Players.Count);

                                    if (color == -1 && match.Context != null && match.Winner == part)
                                        txt = this.Color(txt, 0x336633);
                                    else if (color == -1 && match.Context != null)
                                        txt = this.Color(txt, 0x663333);

                                    sb.Append(txt);
                                }
                            }
                            else if (this.m_Tournament.TournyType == TournyType.FreeForAll)
                            {
                                sb.Append("Free For All");
                            }

                            string str = sb.ToString();

                            if (color >= 0)
                                str = this.Color(str, color);

                            this.AddRightArrow(25, y, this.ToButtonID(5, index + i + 1), str);
                        }

                        break;
                    }
                case TournyBracketGumpType.Match_Info:
                    {
                        TournyMatch match = obj as TournyMatch;

                        if (match == null)
                            break;

                        int ct = (this.m_Tournament.TournyType == TournyType.FreeForAll ? 2 : match.Participants.Count);

                        this.AddPage(0);
                        this.AddBackground(0, 0, 300, 60 + 18 + 20 + 20 + 20 + (ct * 18) + 6, 9380);

                        this.AddLeftArrow(25, 11, this.ToButtonID(0, 5));
                        this.AddHtml(25, 35, 250, 20, this.Center("Rounds"), false, false);

                        this.AddHtml(25, 53, 250, 20, String.Format("Winner: {0}", match.Winner == null ? "N/A" : match.Winner.NameList), false, false);
                        this.AddHtml(25, 73, 250, 20, String.Format("State: {0}", match.InProgress ? "In progress" : match.Context != null ? "Complete" : "Waiting"), false, false);
                        this.AddHtml(25, 93, 250, 20, String.Format("Participants:"), false, false);

                        if (this.m_Tournament.TournyType == TournyType.Standard)
                        {
                            for (int i = 0; i < match.Participants.Count; ++i)
                            {
                                TournyParticipant part = (TournyParticipant)match.Participants[i];

                                this.AddRightArrow(25, 113 + (i * 18), this.ToButtonID(6, i), part.NameList);
                            }
                        }
                        else if (this.m_Tournament.EventController != null || this.m_Tournament.TournyType == TournyType.RandomTeam || this.m_Tournament.TournyType == TournyType.RedVsBlue)
                        {
                            for (int i = 0; i < match.Participants.Count; ++i)
                            {
                                TournyParticipant part = (TournyParticipant)match.Participants[i];

                                if (this.m_Tournament.EventController != null)
                                    this.AddRightArrow(25, 113 + (i * 18), this.ToButtonID(6, i), String.Format("Team {0} ({1})", this.m_Tournament.EventController.GetTeamName(i), part.Players.Count));
                                else if (this.m_Tournament.TournyType == TournyType.RandomTeam)
                                    this.AddRightArrow(25, 113 + (i * 18), this.ToButtonID(6, i), String.Format("Team {0} ({1})", i + 1, part.Players.Count));
                                else
                                    this.AddRightArrow(25, 113 + (i * 18), this.ToButtonID(6, i), String.Format("Team {0} ({1})", i == 0 ? "Red" : "Blue", part.Players.Count));
                            }
                        }
                        else if (this.m_Tournament.TournyType == TournyType.FreeForAll)
                        {
                            this.AddHtml(25, 113, 250, 20, "Free For All", false, false);
                        }

                        break;
                    }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int type, index;

            if (!this.FromButtonID(info.ButtonID, out type, out index))
                return;

            switch ( type )
            {
                case 0:
                    {
                        switch ( index )
                        {
                            case 0:
                                this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Index, null, 0, null));
                                break;
                            case 1:
                                this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Participant_List, null, 0, null));
                                break;
                            case 2:
                                this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Round_List, null, 0, null));
                                break;
                            case 4:
                                this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Rules_Info, null, 0, null));
                                break;
                            case 3:
                                {
                                    Mobile mob = this.m_Object as Mobile;

                                    for (int i = 0; i < this.m_Tournament.Participants.Count; ++i)
                                    {
                                        TournyParticipant part = (TournyParticipant)this.m_Tournament.Participants[i];

                                        if (part.Players.Contains(mob))
                                        {
                                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Participant_Info, null, 0, part));
                                            break;
                                        }
                                    }

                                    break;
                                }
                            case 5:
                                {
                                    TournyMatch match = this.m_Object as TournyMatch;

                                    if (match == null)
                                        break;

                                    for (int i = 0; i < this.m_Tournament.Pyramid.Levels.Count; ++i)
                                    {
                                        PyramidLevel level = (PyramidLevel)this.m_Tournament.Pyramid.Levels[i];

                                        if (level.Matches.Contains(match))
                                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Round_Info, null, 0, level));
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case 1:
                    {
                        switch ( index )
                        {
                            case 0:
                                {
                                    if (this.m_List != null && this.m_Page > 0)
                                        this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, this.m_Type, this.m_List, this.m_Page - 1, this.m_Object));

                                    break;
                                }
                            case 1:
                                {
                                    if (this.m_List != null && ((this.m_Page + 1) * this.m_PerPage) < this.m_List.Count)
                                        this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, this.m_Type, this.m_List, this.m_Page + 1, this.m_Object));

                                    break;
                                }
                        }

                        break;
                    }
                case 2:
                    {
                        if (this.m_Type != TournyBracketGumpType.Participant_List)
                            break;

                        if (index >= 0 && index < this.m_List.Count)
                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Participant_Info, null, 0, this.m_List[index]));

                        break;
                    }
                case 3:
                    {
                        if (this.m_Type != TournyBracketGumpType.Round_List)
                            break;

                        if (index >= 0 && index < this.m_List.Count)
                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Round_Info, null, 0, this.m_List[index]));

                        break;
                    }
                case 4:
                    {
                        if (this.m_Type != TournyBracketGumpType.Participant_Info)
                            break;

                        TournyParticipant part = this.m_Object as TournyParticipant;

                        if (part != null && index >= 0 && index < part.Players.Count)
                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Player_Info, null, 0, part.Players[index]));

                        break;
                    }
                case 5:
                    {
                        if (this.m_Type != TournyBracketGumpType.Round_Info)
                            break;

                        PyramidLevel level = this.m_Object as PyramidLevel;

                        if (level == null)
                            break;

                        if (index == 0)
                        {
                            if (level.FreeAdvance != null)
                                this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Participant_Info, null, 0, level.FreeAdvance));
                            else
                                this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, this.m_Type, this.m_List, this.m_Page, this.m_Object));
                        }
                        else if (index >= 1 && index <= level.Matches.Count)
                        {
                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Match_Info, null, 0, level.Matches[index - 1]));
                        }

                        break;
                    }
                case 6:
                    {
                        if (this.m_Type != TournyBracketGumpType.Match_Info)
                            break;

                        TournyMatch match = this.m_Object as TournyMatch;

                        if (match != null && index >= 0 && index < match.Participants.Count)
                            this.m_From.SendGump(new TournamentBracketGump(this.m_From, this.m_Tournament, TournyBracketGumpType.Participant_Info, null, 0, match.Participants[index]));

                        break;
                    }
            }
        }
    }

    public class TournamentBracketItem : Item
    {
        private TournamentController m_Tournament;

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentController Tournament
        {
            get
            {
                return this.m_Tournament;
            }
            set
            {
                this.m_Tournament = value;
            }
        }

        public override string DefaultName
        {
            get
            {
                return "tournament bracket";
            }
        }

        [Constructable]
        public TournamentBracketItem()
            : base(3774)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that
            }
            else if (this.m_Tournament != null)
            {
                Tournament tourny = this.m_Tournament.Tournament;

                if (tourny != null)
                {
                    from.CloseGump(typeof(TournamentBracketGump));
                    from.SendGump(new TournamentBracketGump(from, tourny, TournyBracketGumpType.Index, null, 0, null));
                    /*if ( tourny.Stage == TournamentStage.Fighting && tourny.Pyramid.Levels.Count > 0 )
                    from.SendGump( new TournamentBracketGump( tourny, (PyramidLevel)tourny.Pyramid.Levels[tourny.Pyramid.Levels.Count - 1] ) );
                    else
                    from.SendGump( new TournamentBracketGump( tourny, 0 ) );*/
                }
            }
        }

        public TournamentBracketItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)this.m_Tournament);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Tournament = reader.ReadItem() as TournamentController;
                        break;
                    }
            }
        }
    }
}