using System;
using System.Collections;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.ConPVP
{
    public class RulesetGump : Gump
    {
        private readonly Mobile m_From;
        private readonly Ruleset m_Ruleset;
        private readonly RulesetLayout m_Page;
        private readonly DuelContext m_DuelContext;
        private readonly bool m_ReadOnly;
        public RulesetGump(Mobile from, Ruleset ruleset, RulesetLayout page, DuelContext duelContext)
            : this(from, ruleset, page, duelContext, false)
        {
        }

        public RulesetGump(Mobile from, Ruleset ruleset, RulesetLayout page, DuelContext duelContext, bool readOnly)
            : base(readOnly ? 310 : 50, 50)
        {
            this.m_From = from;
            this.m_Ruleset = ruleset;
            this.m_Page = page;
            this.m_DuelContext = duelContext;
            this.m_ReadOnly = readOnly;

            this.Dragable = !readOnly;

            from.CloseGump(typeof(RulesetGump));
            from.CloseGump(typeof(DuelContextGump));
            from.CloseGump(typeof(ParticipantGump));

            RulesetLayout depthCounter = page;
            int depth = 0;

            while (depthCounter != null)
            {
                ++depth;
                depthCounter = depthCounter.Parent;
            }

            int count = page.Children.Length + page.Options.Length;

            this.AddPage(0);

            int height = 35 + 10 + 2 + (count * 22) + 2 + 30;

            this.AddBackground(0, 0, 260, height, 9250);
            this.AddBackground(10, 10, 240, height - 20, 0xDAC);

            this.AddHtml(35, 25, 190, 20, this.Center(page.Title), false, false);

            int x = 35;
            int y = 47;

            for (int i = 0; i < page.Children.Length; ++i)
            {
                this.AddGoldenButton(x, y, 1 + i);
                this.AddHtml(x + 25, y, 250, 22, page.Children[i].Title, false, false);

                y += 22;
            }

            for (int i = 0; i < page.Options.Length; ++i)
            {
                bool enabled = ruleset.Options[page.Offset + i];

                if (readOnly)
                    this.AddImage(x, y, enabled ? 0xD3 : 0xD2);
                else
                    this.AddCheck(x, y, 0xD2, 0xD3, enabled, i);

                this.AddHtml(x + 25, y, 250, 22, page.Options[i], false, false);

                y += 22;
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

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_DuelContext != null && !this.m_DuelContext.Registered)
                return;

            if (!this.m_ReadOnly)
            {
                BitArray opts = new BitArray(this.m_Page.Options.Length);

                for (int i = 0; i < info.Switches.Length; ++i)
                {
                    int sid = info.Switches[i];

                    if (sid >= 0 && sid < this.m_Page.Options.Length)
                        opts[sid] = true;
                }

                for (int i = 0; i < opts.Length; ++i)
                {
                    if (this.m_Ruleset.Options[this.m_Page.Offset + i] != opts[i])
                    {
                        this.m_Ruleset.Options[this.m_Page.Offset + i] = opts[i];
                        this.m_Ruleset.Changed = true;
                    }
                }
            }

            int bid = info.ButtonID;

            if (bid == 0)
            {
                if (this.m_Page.Parent != null)
                    this.m_From.SendGump(new RulesetGump(this.m_From, this.m_Ruleset, this.m_Page.Parent, this.m_DuelContext, this.m_ReadOnly));
                else if (!this.m_ReadOnly)
                    this.m_From.SendGump(new PickRulesetGump(this.m_From, this.m_DuelContext, this.m_Ruleset));
            }
            else
            {
                bid -= 1;

                if (bid >= 0 && bid < this.m_Page.Children.Length)
                    this.m_From.SendGump(new RulesetGump(this.m_From, this.m_Ruleset, this.m_Page.Children[bid], this.m_DuelContext, this.m_ReadOnly));
            }
        }
    }
}