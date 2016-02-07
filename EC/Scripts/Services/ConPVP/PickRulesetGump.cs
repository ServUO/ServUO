using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class PickRulesetGump : Gump
    {
        private readonly Mobile m_From;
        private readonly DuelContext m_Context;
        private readonly Ruleset m_Ruleset;
        private readonly Ruleset[] m_Defaults;
        private readonly Ruleset[] m_Flavors;
        public PickRulesetGump(Mobile from, DuelContext context, Ruleset ruleset)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Context = context;
            this.m_Ruleset = ruleset;
            this.m_Defaults = ruleset.Layout.Defaults;
            this.m_Flavors = ruleset.Layout.Flavors;

            int height = 25 + 20 + ((this.m_Defaults.Length + 1) * 22) + 6 + 20 + (this.m_Flavors.Length * 22) + 25;

            this.AddPage(0);

            this.AddBackground(0, 0, 260, height, 9250);
            this.AddBackground(10, 10, 240, height - 20, 0xDAC);

            this.AddHtml(35, 25, 190, 20, this.Center("Rules"), false, false);

            int y = 25 + 20;

            for (int i = 0; i < this.m_Defaults.Length; ++i)
            {
                Ruleset cur = this.m_Defaults[i];

                this.AddHtml(35 + 14, y, 176, 20, cur.Title, false, false);

                if (ruleset.Base == cur && !ruleset.Changed)
                    this.AddImage(35, y + 4, 0x939);
                else if (ruleset.Base == cur)
                    this.AddButton(35, y + 4, 0x93A, 0x939, 2 + i, GumpButtonType.Reply, 0);
                else
                    this.AddButton(35, y + 4, 0x938, 0x939, 2 + i, GumpButtonType.Reply, 0);

                y += 22;
            }

            this.AddHtml(35 + 14, y, 176, 20, "Custom", false, false);
            this.AddButton(35, y + 4, ruleset.Changed ? 0x939 : 0x938, 0x939, 1, GumpButtonType.Reply, 0);

            y += 22;
            y += 6;

            this.AddHtml(35, y, 190, 20, this.Center("Flavors"), false, false);
            y += 20;

            for (int i = 0; i < this.m_Flavors.Length; ++i)
            {
                Ruleset cur = this.m_Flavors[i];

                this.AddHtml(35 + 14, y, 176, 20, cur.Title, false, false);

                if (ruleset.Flavors.Contains(cur))
                    this.AddButton(35, y + 4, 0x939, 0x938, 2 + this.m_Defaults.Length + i, GumpButtonType.Reply, 0);
                else
                    this.AddButton(35, y + 4, 0x938, 0x939, 2 + this.m_Defaults.Length + i, GumpButtonType.Reply, 0);

                y += 22;
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Context != null && !this.m_Context.Registered)
                return;

            switch ( info.ButtonID )
            {
                case 0: // closed
                    {
                        if (this.m_Context != null)
                            this.m_From.SendGump(new DuelContextGump(this.m_From, this.m_Context));

                        break;
                    }
                case 1: // customize
                    {
                        this.m_From.SendGump(new RulesetGump(this.m_From, this.m_Ruleset, this.m_Ruleset.Layout, this.m_Context));
                        break;
                    }
                default:
                    {
                        int idx = info.ButtonID - 2;

                        if (idx >= 0 && idx < this.m_Defaults.Length)
                        {
                            this.m_Ruleset.ApplyDefault(this.m_Defaults[idx]);
                            this.m_From.SendGump(new PickRulesetGump(this.m_From, this.m_Context, this.m_Ruleset));
                        }
                        else
                        {
                            idx -= this.m_Defaults.Length;

                            if (idx >= 0 && idx < this.m_Flavors.Length)
                            {
                                if (this.m_Ruleset.Flavors.Contains(this.m_Flavors[idx]))
                                    this.m_Ruleset.RemoveFlavor(this.m_Flavors[idx]);
                                else
                                    this.m_Ruleset.AddFlavor(this.m_Flavors[idx]);

                                this.m_From.SendGump(new PickRulesetGump(this.m_From, this.m_Context, this.m_Ruleset));
                            }
                        }

                        break;
                    }
            }
        }
    }
}