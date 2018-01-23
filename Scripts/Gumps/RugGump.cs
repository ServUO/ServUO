using System;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class RugGump : Gump
    {
        private BaseRugAddonDeed Deed { get; set; }

        public RugGump(Mobile owner, BaseRugAddonDeed deed)
            : base(10, 10)
        {
            Deed = deed;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 313, 206, 2600);
            AddHtmlLocalized(83, 26, 200 , 20, deed.LabelNumber, false ,false);

            AddHtmlLocalized(80, 65, 200, 20, 1150125, false, false);
            AddHtmlLocalized(180, 65, 200, 20, 1150124, false, false);
            AddButton(45, 65, 4007, 4006, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(80, 90, 200, 20, 1150125, false, false);
            AddHtmlLocalized(180, 90, 200, 20, 1150123, false, false);
            AddButton(45, 90, 4007, 4006, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(80, 115, 200, 20, 1150126, false, false);
            AddHtmlLocalized(180, 115, 200, 20, 1150124, false, false);
            AddButton(45, 115, 4007, 4006, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(80, 140, 200, 20, 1150126, false, false);
            AddHtmlLocalized(180, 140, 200, 20, 1150123, false, false);
            AddButton(45, 140, 4007, 4006, 4, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (Deed.Deleted)
                return;

            Mobile from = state.Mobile;

            if (!Deed.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); //You must have the objectin your backpack to use it.
                return;
            }

            switch (info.ButtonID)
            {
                case 1:
                    Deed.RugType = RugType.SouthLarge;
                    from.Target = new BaseAddonDeed.InternalTarget(Deed);
                    break;
                case 2:
                    Deed.RugType = RugType.EastLarge;
                    from.Target = new BaseAddonDeed.InternalTarget(Deed);
                    break;
                case 3:
                    Deed.RugType = RugType.SouthSmall;
                    from.Target = new BaseAddonDeed.InternalTarget(Deed);
                    break;
                case 4:
                    Deed.RugType = RugType.EastSmall;
                    from.Target = new BaseAddonDeed.InternalTarget(Deed);
                    break;
            }
        }
    }
}
