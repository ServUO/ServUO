using System;
using Server.Items;

namespace Server.Gumps
{
    public class ClockworkStartGump : Gump
    {
        readonly ClockworkMechanism m_Item;
        public ClockworkStartGump(ClockworkMechanism item)
            : base(200, 200)
        {
            this.m_Item = item;

            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 297, 115, 9200);

            this.AddImageTiled(5, 10, 285, 25, 2624);
            this.AddHtmlLocalized(10, 15, 275, 25, 1112855, 0x7FFF, false, false);

            this.AddImageTiled(5, 40, 285, 40, 2624);
            this.AddHtmlLocalized(10, 40, 275, 40, 1112856, 0x7FFF, false, false);

            this.AddButton(5, 85, 4017, 4018, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(40, 87, 80, 25, 1011012, 0x7FFF, false, false);

            this.AddButton(215, 85, 4023, 4024, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(250, 87, 80, 25, 1006044, 0x7FFF, false, false);
        }

        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        { 
            if (0 != info.ButtonID && null != this.m_Item && !this.m_Item.Deleted)
                this.m_Item.BeginMechanismAssembly(state.Mobile);
        }
    }
}