using System;
using System.Collections.Generic;
using Server.Network;

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

                if(m_Options[i].Cliloc.Number > 0)
                    AddHtmlLocalized(44, 47 + i * 24, 213, 20, m_Options[i].Cliloc.Number, 0x7FFF, false, false);
                else
                    AddHtml(44, 47 + i * 24, 213, 20, String.Format("<basefont color=#FFFFFF>{0}", m_Options[i].Cliloc.String), false, false);
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
        private readonly int m_ID;
        private readonly TextDefinition m_Cliloc;

        public RewardOption(int id, TextDefinition cliloc)
        {
            m_ID = id;
            m_Cliloc = cliloc;
        }

        public int ID
        {
            get
            {
                return m_ID;
            }
        }
        public TextDefinition Cliloc
        {
            get
            {
                return m_Cliloc;
            }
        }
    }

    public class RewardOptionList : List<RewardOption>
    {
        public RewardOptionList()
            : base()
        {
        }

        public void Add(int id, TextDefinition cliloc)
        {
            Add(new RewardOption(id, cliloc));
        }
    }
}