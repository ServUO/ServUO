using System;
using System.Collections.Generic;
using Server.Guilds;

namespace Server.Gumps
{
    public abstract class GuildMobileListGump : Gump
    {
        protected Mobile m_Mobile;
        protected Guild m_Guild;
        protected List<Mobile> m_List;
        public GuildMobileListGump(Mobile from, Guild guild, bool radio, List<Mobile> list)
            : base(20, 30)
        {
            this.m_Mobile = from;
            this.m_Guild = guild;

            this.Dragable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 550, 440, 5054);
            this.AddBackground(10, 10, 530, 420, 3000);

            this.Design();

            this.m_List = new List<Mobile>(list);

            for (int i = 0; i < this.m_List.Count; ++i)
            {
                if ((i % 11) == 0)
                {
                    if (i != 0)
                    {
                        this.AddButton(300, 370, 4005, 4007, 0, GumpButtonType.Page, (i / 11) + 1);
                        this.AddHtmlLocalized(335, 370, 300, 35, 1011066, false, false); // Next page
                    }

                    this.AddPage((i / 11) + 1);

                    if (i != 0)
                    {
                        this.AddButton(20, 370, 4014, 4016, 0, GumpButtonType.Page, (i / 11));
                        this.AddHtmlLocalized(55, 370, 300, 35, 1011067, false, false); // Previous page
                    }
                }

                if (radio)
                    this.AddRadio(20, 35 + ((i % 11) * 30), 208, 209, false, i);

                Mobile m = this.m_List[i];

                string name;

                if ((name = m.Name) != null && (name = name.Trim()).Length <= 0)
                    name = "(empty)";

                this.AddLabel((radio ? 55 : 20), 35 + ((i % 11) * 30), 0, name);
            }
        }

        protected virtual void Design()
        {
        }
    }
}