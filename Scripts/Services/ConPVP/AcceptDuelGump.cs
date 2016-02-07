using System;
using System.Collections;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class AcceptDuelGump : Gump
    {
        private static readonly Hashtable m_IgnoreLists = new Hashtable();
        private const int LabelColor32 = 0xFFFFFF;
        private const int BlackColor32 = 0x000008;
        private readonly Mobile m_Challenger;
        private readonly Mobile m_Challenged;
        private readonly DuelContext m_Context;
        private readonly Participant m_Participant;
        private readonly int m_Slot;
        private bool m_Active = true;
        public AcceptDuelGump(Mobile challenger, Mobile challenged, DuelContext context, Participant p, int slot)
            : base(50, 50)
        {
            this.m_Challenger = challenger;
            this.m_Challenged = challenged;
            this.m_Context = context;
            this.m_Participant = p;
            this.m_Slot = slot;

            challenged.CloseGump(typeof(AcceptDuelGump));

            this.Closable = false;

            this.AddPage(0);

            //AddBackground( 0, 0, 400, 220, 9150 );
            this.AddBackground(1, 1, 398, 218, 3600);
            //AddBackground( 16, 15, 369, 189, 9100 );

            this.AddImageTiled(16, 15, 369, 189, 3604);
            this.AddAlphaRegion(16, 15, 369, 189);

            this.AddImage(215, -43, 0xEE40);
            //AddImage( 330, 141, 0x8BA );

            this.AddHtml(22 - 1, 22, 294, 20, this.Color(this.Center("Duel Challenge"), BlackColor32), false, false);
            this.AddHtml(22 + 1, 22, 294, 20, this.Color(this.Center("Duel Challenge"), BlackColor32), false, false);
            this.AddHtml(22, 22 - 1, 294, 20, this.Color(this.Center("Duel Challenge"), BlackColor32), false, false);
            this.AddHtml(22, 22 + 1, 294, 20, this.Color(this.Center("Duel Challenge"), BlackColor32), false, false);
            this.AddHtml(22, 22, 294, 20, this.Color(this.Center("Duel Challenge"), LabelColor32), false, false);

            string fmt;

            if (p.Contains(challenger))
                fmt = "You have been asked to join sides with {0} in a duel. Do you accept?";
            else
                fmt = "You have been challenged to a duel from {0}. Do you accept?";

            this.AddHtml(22 - 1, 50, 294, 40, this.Color(String.Format(fmt, challenger.Name), BlackColor32), false, false);
            this.AddHtml(22 + 1, 50, 294, 40, this.Color(String.Format(fmt, challenger.Name), BlackColor32), false, false);
            this.AddHtml(22, 50 - 1, 294, 40, this.Color(String.Format(fmt, challenger.Name), BlackColor32), false, false);
            this.AddHtml(22, 50 + 1, 294, 40, this.Color(String.Format(fmt, challenger.Name), BlackColor32), false, false);
            this.AddHtml(22, 50, 294, 40, this.Color(String.Format(fmt, challenger.Name), 0xB0C868), false, false);

            this.AddImageTiled(32, 88, 264, 1, 9107);
            this.AddImageTiled(42, 90, 264, 1, 9157);

            this.AddRadio(24, 100, 9727, 9730, true, 1);
            this.AddHtml(60 - 1, 105, 250, 20, this.Color("Yes, I will fight this duel.", BlackColor32), false, false);
            this.AddHtml(60 + 1, 105, 250, 20, this.Color("Yes, I will fight this duel.", BlackColor32), false, false);
            this.AddHtml(60, 105 - 1, 250, 20, this.Color("Yes, I will fight this duel.", BlackColor32), false, false);
            this.AddHtml(60, 105 + 1, 250, 20, this.Color("Yes, I will fight this duel.", BlackColor32), false, false);
            this.AddHtml(60, 105, 250, 20, this.Color("Yes, I will fight this duel.", LabelColor32), false, false);

            this.AddRadio(24, 135, 9727, 9730, false, 2);
            this.AddHtml(60 - 1, 140, 250, 20, this.Color("No, I do not wish to fight.", BlackColor32), false, false);
            this.AddHtml(60 + 1, 140, 250, 20, this.Color("No, I do not wish to fight.", BlackColor32), false, false);
            this.AddHtml(60, 140 - 1, 250, 20, this.Color("No, I do not wish to fight.", BlackColor32), false, false);
            this.AddHtml(60, 140 + 1, 250, 20, this.Color("No, I do not wish to fight.", BlackColor32), false, false);
            this.AddHtml(60, 140, 250, 20, this.Color("No, I do not wish to fight.", LabelColor32), false, false);

            this.AddRadio(24, 170, 9727, 9730, false, 3);
            this.AddHtml(60 - 1, 175, 250, 20, this.Color("No, knave. Do not ask again.", BlackColor32), false, false);
            this.AddHtml(60 + 1, 175, 250, 20, this.Color("No, knave. Do not ask again.", BlackColor32), false, false);
            this.AddHtml(60, 175 - 1, 250, 20, this.Color("No, knave. Do not ask again.", BlackColor32), false, false);
            this.AddHtml(60, 175 + 1, 250, 20, this.Color("No, knave. Do not ask again.", BlackColor32), false, false);
            this.AddHtml(60, 175, 250, 20, this.Color("No, knave. Do not ask again.", LabelColor32), false, false);

            this.AddButton(314, 173, 247, 248, 1, GumpButtonType.Reply, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(15.0), new TimerCallback(AutoReject));
        }

        public static void BeginIgnore(Mobile source, Mobile toIgnore)
        {
            ArrayList list = (ArrayList)m_IgnoreLists[source];

            if (list == null)
                m_IgnoreLists[source] = list = new ArrayList();

            for (int i = 0; i < list.Count; ++i)
            {
                IgnoreEntry ie = (IgnoreEntry)list[i];

                if (ie.Ignored == toIgnore)
                {
                    ie.Refresh();
                    return;
                }
                else if (ie.Expired)
                {
                    list.RemoveAt(i--);
                }
            }

            list.Add(new IgnoreEntry(toIgnore));
        }

        public static bool IsIgnored(Mobile source, Mobile check)
        {
            ArrayList list = (ArrayList)m_IgnoreLists[source];

            if (list == null)
                return false;

            for (int i = 0; i < list.Count; ++i)
            {
                IgnoreEntry ie = (IgnoreEntry)list[i];

                if (ie.Expired)
                    list.RemoveAt(i--);
                else if (ie.Ignored == check)
                    return true;
            }

            return false;
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AutoReject()
        {
            if (!this.m_Active)
                return;

            this.m_Active = false;

            this.m_Challenged.CloseGump(typeof(AcceptDuelGump));

            this.m_Challenger.SendMessage("{0} seems unresponsive.", this.m_Challenged.Name);
            this.m_Challenged.SendMessage("You decline the challenge.");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID != 1 || !this.m_Active || !this.m_Context.Registered)
                return;

            this.m_Active = false;

            if (!this.m_Context.Participants.Contains(this.m_Participant))
                return;

            if (info.IsSwitched(1))
            {
                PlayerMobile pm = this.m_Challenged as PlayerMobile;

                if (pm == null)
                    return;

                if (pm.DuelContext != null)
                {
                    if (pm.DuelContext.Initiator == pm)
                        pm.SendMessage(0x22, "You have already started a duel.");
                    else
                        pm.SendMessage(0x22, "You have already been challenged in a duel.");

                    this.m_Challenger.SendMessage("{0} cannot fight because they are already assigned to another duel.", pm.Name);
                }
                else if (DuelContext.CheckCombat(pm))
                {
                    pm.SendMessage(0x22, "You have recently been in combat with another player and must wait before starting a duel.");
                    this.m_Challenger.SendMessage("{0} cannot fight because they have recently been in combat with another player.", pm.Name);
                }
                else if (TournamentController.IsActive)
                {
                    pm.SendMessage(0x22, "A tournament is currently active and you may not duel.");
                    this.m_Challenger.SendMessage(0x22, "A tournament is currently active and you may not duel.");
                }
                else
                {
                    bool added = false;

                    if (this.m_Slot >= 0 && this.m_Slot < this.m_Participant.Players.Length && this.m_Participant.Players[this.m_Slot] == null)
                    {
                        added = true;
                        this.m_Participant.Players[this.m_Slot] = new DuelPlayer(this.m_Challenged, this.m_Participant);
                    }
                    else
                    {
                        for (int i = 0; i < this.m_Participant.Players.Length; ++i)
                        {
                            if (this.m_Participant.Players[i] == null)
                            {
                                added = true;
                                this.m_Participant.Players[i] = new DuelPlayer(this.m_Challenged, this.m_Participant);
                                break;
                            }
                        }
                    }

                    if (added)
                    {
                        this.m_Challenger.SendMessage("{0} has accepted the request.", this.m_Challenged.Name);
                        this.m_Challenged.SendMessage("You have accepted the request from {0}.", this.m_Challenger.Name);

                        NetState ns = this.m_Challenger.NetState;

                        if (ns != null)
                        {
                            foreach (Gump g in ns.Gumps)
                            {
                                if (g is ParticipantGump)
                                {
                                    ParticipantGump pg = (ParticipantGump)g;

                                    if (pg.Participant == this.m_Participant)
                                    {
                                        this.m_Challenger.SendGump(new ParticipantGump(this.m_Challenger, this.m_Context, this.m_Participant));
                                        break;
                                    }
                                }
                                else if (g is DuelContextGump)
                                {
                                    DuelContextGump dcg = (DuelContextGump)g;

                                    if (dcg.Context == this.m_Context)
                                    {
                                        this.m_Challenger.SendGump(new DuelContextGump(this.m_Challenger, this.m_Context));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this.m_Challenger.SendMessage("The participant list was full and so {0} could not join.", this.m_Challenged.Name);
                        this.m_Challenged.SendMessage("The participant list was full and so you could not join the fight {1} {0}.", this.m_Challenger.Name, this.m_Participant.Contains(this.m_Challenger) ? "with" : "against");
                    }
                }
            }
            else
            {
                if (info.IsSwitched(3))
                    BeginIgnore(this.m_Challenged, this.m_Challenger);

                this.m_Challenger.SendMessage("{0} does not wish to fight.", this.m_Challenged.Name);
                this.m_Challenged.SendMessage("You chose not to fight {1} {0}.", this.m_Challenger.Name, this.m_Participant.Contains(this.m_Challenger) ? "with" : "against");
            }
        }

        private class IgnoreEntry
        {
            public readonly Mobile m_Ignored;
            public DateTime m_Expire;
            private static readonly TimeSpan ExpireDelay = TimeSpan.FromMinutes(15.0);
            public IgnoreEntry(Mobile ignored)
            {
                this.m_Ignored = ignored;
                this.Refresh();
            }

            public Mobile Ignored
            {
                get
                {
                    return this.m_Ignored;
                }
            }
            public bool Expired
            {
                get
                {
                    return (DateTime.UtcNow >= this.m_Expire);
                }
            }
            public void Refresh()
            {
                this.m_Expire = DateTime.UtcNow + ExpireDelay;
            }
        }
    }
}