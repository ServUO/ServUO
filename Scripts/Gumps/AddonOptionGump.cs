using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Gumps
{
    public interface IAddonOption
    {
        void GetOptions(AddonOptionList list);
        void OnOptionSelected(Mobile from, int choice);
    }

    public class AddonOptionGump : Gump
    {
        private readonly AddonOptionList m_Options = new AddonOptionList();
        private readonly IAddonOption m_Option;

        public AddonOptionGump(IAddonOption option)
            : this(option, 0)
        {
        }

        public AddonOptionGump(IAddonOption option, int title)
            : base(50, 50)
        {
            m_Option = option;

            if (m_Option != null)
                m_Option.GetOptions(m_Options);

            AddPage(0);

            AddBackground(0, 0, 300, 180, 0xA28);

            if (title > 0)
                AddHtmlLocalized(30, 30, 240, 20, 1113302, String.Format("#{0}", title), 0x0, false, false); // <CENTER>~1_VAL~</CENTER>
            else
                AddHtmlLocalized(30, 30, 240, 20, 1113302, "#1080392", 0x0, false, false); // Select your choice from the menu below.

            for (int i = 0; i < m_Options.Count; i++)
            {
                AddButton(30, 70 + i * 20, 0xFA5, 0xFA7, m_Options[i].ID, GumpButtonType.Reply, 0);

                if(m_Options[i].Cliloc.Number > 0)
                    AddHtmlLocalized(70, 70 + i * 20, 150, 20, m_Options[i].Cliloc.Number, 0x0, false, false);
                else
                    AddHtml(70, 70 + i * 20, 150, 20, String.Format("<basefont color=#000000>{0}", m_Options[i].Cliloc.String), false, false);
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

            foreach (AddonOption option in m_Options)
            {
                if (option.ID == chosen)
                    return true;
            }

            return false;
        }
    }

    public class AddonOption
    {
        public AddonOption(int id, TextDefinition cliloc)
        {
            ID = id;
            Cliloc = cliloc;
        }

        public int ID { get; private set; }
        public TextDefinition Cliloc { get; private set; }
    }

    public class AddonOptionList : List<AddonOption>
    {
        public AddonOptionList()
            : base()
        {
        }

        public void Add(int id, TextDefinition cliloc)
        {
            Add(new AddonOption(id, cliloc));
        }
    }
}
