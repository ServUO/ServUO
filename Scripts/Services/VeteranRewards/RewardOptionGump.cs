using Server.Network;
using System.Collections.Generic;

namespace Server.Gumps
{
    public interface IRewardOption
    {
        void GetOptions(RewardOptionList list);
        void OnOptionSelected(Mobile from, int choice);
    }

    public class RewardOptionGump : Gump
    {
        private readonly RewardOptionList m_Options = new RewardOptionList();
        private readonly IRewardOption m_Option;

        public RewardOptionGump(IRewardOption option)
            : this(option, 0)
        {
        }

        public RewardOptionGump(IRewardOption option, int title)
            : base(60, 36)
        {
            m_Option = option;

            if (m_Option != null)
                m_Option.GetOptions(m_Options);

            AddPage(0);

            AddBackground(0, 0, 273, 324, 0x13BE);
            AddImageTiled(10, 10, 253, 20, 0xA40);
            AddImageTiled(10, 40, 253, 244, 0xA40);
            AddImageTiled(10, 294, 253, 20, 0xA40);
            AddAlphaRegion(10, 10, 253, 304);

            AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

            if (title > 0)
                AddHtmlLocalized(14, 12, 273, 20, title, 0x7FFF, false, false);
            else
                AddHtmlLocalized(14, 12, 273, 20, 1080392, 0x7FFF, false, false); // Select your choice from the menu below.

            AddPage(1);

            for (int i = 0; i < m_Options.Count; i++)
            {
                AddButton(19, 49 + i * 24, 0x845, 0x846, m_Options[i].ID, GumpButtonType.Reply, 0);

                if (m_Options[i].Cliloc.Number > 0)
                    AddHtmlLocalized(44, 47 + i * 24, 213, 20, m_Options[i].Cliloc.Number, 0x7FFF, false, false);
                else
                    AddHtml(44, 47 + i * 24, 213, 20, string.Format("<basefont color=#FFFFFF>{0}", m_Options[i].Text), false, false);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Option != null && Contains(info.ButtonID))
                m_Option.OnOptionSelected(sender.Mobile, info.ButtonID);
        }

        private bool Contains(int chosen)
        {
            if (m_Options == null)
                return false;

            foreach (RewardOption option in m_Options)
            {
                if (option.ID == chosen)
                    return true;
            }

            return false;
        }
    }

    public class AddonOptionGump : Gump
    {
        private readonly RewardOptionList m_Options = new RewardOptionList();
        private readonly IRewardOption m_Option;

        public AddonOptionGump(IRewardOption option)
            : this(option, 0)
        {
        }

        public AddonOptionGump(IRewardOption option, int title)
            : this(option, title, 300, 180)
        {
        }

        public AddonOptionGump(IRewardOption option, int title, int bgw, int bgh)
            : base(50, 50)
        {
            m_Option = option;

            if (m_Option != null)
                m_Option.GetOptions(m_Options);

            AddPage(0);

            AddBackground(0, 0, bgw, bgh, 0xA28);

            if (title > 0)
                AddHtmlLocalized(30, 30, 240, 20, 1113302, string.Format("#{0}", title), 0x0, false, false); // <CENTER>~1_VAL~</CENTER>
            else
                AddHtmlLocalized(30, 30, 240, 20, 1113302, "#1080392", 0x0, false, false); // Select your choice from the menu below.

            for (int i = 0; i < m_Options.Count; i++)
            {
                AddButton(30, 70 + i * 20, 0xFA5, 0xFA7, m_Options[i].ID, GumpButtonType.Reply, 0);

                if (m_Options[i].Cliloc.Number > 0)
                    AddHtmlLocalized(70, 70 + i * 20, 150, 20, m_Options[i].Cliloc.Number, 0x0, false, false);
                else
                    AddHtml(70, 70 + i * 20, 150, 20, string.Format("<basefont color=#000000>{0}", m_Options[i].Text), false, false);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Option != null && Contains(info.ButtonID))
                m_Option.OnOptionSelected(sender.Mobile, info.ButtonID);
        }

        private bool Contains(int chosen)
        {
            if (m_Options == null)
                return false;

            foreach (RewardOption option in m_Options)
            {
                if (option.ID == chosen)
                    return true;
            }

            return false;
        }
    }

    public class RewardOption
    {
        public RewardOption(int id, TextDefinition cliloc, string text)
        {
            ID = id;
            Cliloc = cliloc;
            Text = text;
        }

        public int ID { get; set; }
        public TextDefinition Cliloc { get; set; }
        public string Text { get; set; }
    }

    public class RewardOptionList : List<RewardOption>
    {
        public void Add(int id, TextDefinition cliloc)
        {
            Add(new RewardOption(id, cliloc, null));
        }

        public void Add(int id, string text)
        {
            Add(new RewardOption(id, 0, text));
        }
    }
}
