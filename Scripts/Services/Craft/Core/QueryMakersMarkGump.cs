using System;
using Server.Gumps;
using Server.Items;

namespace Server.Engines.Craft
{
    public class QueryMakersMarkGump : Gump
    {
        private readonly int m_Quality;
        private readonly Mobile m_From;
        private readonly CraftItem m_CraftItem;
        private readonly CraftSystem m_CraftSystem;
        private readonly Type m_TypeRes;
        private readonly ITool m_Tool;
        public QueryMakersMarkGump(int quality, Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, ITool tool)
            : base(100, 200)
        {
            from.CloseGump(typeof(QueryMakersMarkGump));

            this.m_Quality = quality;
            this.m_From = from;
            this.m_CraftItem = craftItem;
            this.m_CraftSystem = craftSystem;
            this.m_TypeRes = typeRes;
            this.m_Tool = tool;

            this.AddPage(0);

            this.AddBackground(0, 0, 220, 170, 5054);
            this.AddBackground(10, 10, 200, 150, 3000);

            this.AddHtmlLocalized(20, 20, 180, 80, 1018317, false, false); // Do you wish to place your maker's mark on this item?

            this.AddHtmlLocalized(55, 100, 140, 25, 1011011, false, false); // CONTINUE
            this.AddButton(20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 125, 140, 25, 1011012, false, false); // CANCEL
            this.AddButton(20, 125, 4005, 4007, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            bool makersMark = (info.ButtonID == 1);

            if (makersMark)
                this.m_From.SendLocalizedMessage(501808); // You mark the item.
            else
                this.m_From.SendLocalizedMessage(501809); // Cancelled mark.

            this.m_CraftItem.CompleteCraft(this.m_Quality, makersMark, this.m_From, this.m_CraftSystem, this.m_TypeRes, this.m_Tool, null);
        }
    }
}