using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class DuelContextGump : Gump
    {
        private readonly Mobile m_From;
        private readonly DuelContext m_Context;
        public DuelContextGump(Mobile from, DuelContext context)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Context = context;

            from.CloseGump(typeof(RulesetGump));
            from.CloseGump(typeof(DuelContextGump));
            from.CloseGump(typeof(ParticipantGump));

            int count = context.Participants.Count;

            if (count < 3)
                count = 3;

            int height = 35 + 10 + 22 + 30 + 22 + 22 + 2 + (count * 22) + 2 + 30;

            this.AddPage(0);

            this.AddBackground(0, 0, 300, height, 9250);
            this.AddBackground(10, 10, 280, height - 20, 0xDAC);

            this.AddHtml(35, 25, 230, 20, this.Center("Duel Setup"), false, false);

            int x = 35;
            int y = 47;

            this.AddGoldenButtonLabeled(x, y, 1, "Rules");
            y += 22;
            this.AddGoldenButtonLabeled(x, y, 2, "Start");
            y += 22;
            this.AddGoldenButtonLabeled(x, y, 3, "Add Participant");
            y += 30;

            this.AddHtml(35, y, 230, 20, this.Center("Participants"), false, false);
            y += 22;

            for (int i = 0; i < context.Participants.Count; ++i)
            {
                Participant p = (Participant)context.Participants[i];

                this.AddGoldenButtonLabeled(x, y, 4 + i, String.Format(p.Count == 1 ? "Player {0}: {3}" : "Team {0}: {1}/{2}: {3}", 1 + i, p.FilledSlots, p.Count, p.NameList));
                y += 22;
            }
        }

        public Mobile From
        {
            get
            {
                return this.m_From;
            }
        }
        public DuelContext Context
        {
            get
            {
                return this.m_Context;
            }
        }
        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public void AddGoldenButton(int x, int y, int bid)
        {
            this.AddButton(x, y, 0xD2, 0xD2, bid, GumpButtonType.Reply, 0);
            this.AddButton(x + 3, y + 3, 0xD8, 0xD8, bid, GumpButtonType.Reply, 0);
        }

        public void AddGoldenButtonLabeled(int x, int y, int bid, string text)
        {
            this.AddGoldenButton(x, y, bid);
            this.AddHtml(x + 25, y, 200, 20, text, false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!this.m_Context.Registered)
                return;

            int index = info.ButtonID;

            switch ( index )
            {
                case -1: // CloseGump
                    {
                        break;
                    }
                case 0: // closed
                    {
                        this.m_Context.Unregister();
                        break;
                    }
                case 1: // Rules
                    {
                        //m_From.SendGump( new RulesetGump( m_From, m_Context.Ruleset, m_Context.Ruleset.Layout, m_Context ) );
                        this.m_From.SendGump(new PickRulesetGump(this.m_From, this.m_Context, this.m_Context.Ruleset));
                        break;
                    }
                case 2: // Start
                    {
                        if (this.m_Context.CheckFull())
                        {
                            this.m_Context.CloseAllGumps();
                            this.m_Context.SendReadyUpGump();
                            //m_Context.SendReadyGump();
                        }
                        else
                        {
                            this.m_From.SendMessage("You cannot start the duel before all participating players have been assigned.");
                            this.m_From.SendGump(new DuelContextGump(this.m_From, this.m_Context));
                        }

                        break;
                    }
                case 3: // New Participant
                    {
                        if (this.m_Context.Participants.Count < 10)
                            this.m_Context.Participants.Add(new Participant(this.m_Context, 1));
                        else
                            this.m_From.SendMessage("The number of participating parties may not be increased further.");

                        this.m_From.SendGump(new DuelContextGump(this.m_From, this.m_Context));

                        break;
                    }
                default: // Participant
                    {
                        index -= 4;

                        if (index >= 0 && index < this.m_Context.Participants.Count)
                            this.m_From.SendGump(new ParticipantGump(this.m_From, this.m_Context, (Participant)this.m_Context.Participants[index]));

                        break;
                    }
            }
        }
    }
}