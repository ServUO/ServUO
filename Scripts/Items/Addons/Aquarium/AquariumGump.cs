using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class AquariumGump : Gump
    {
        private readonly Aquarium m_Aquarium;
        public AquariumGump(Aquarium aquarium, bool edit)
            : base(100, 100)
        {
            this.m_Aquarium = aquarium;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 350, 323, 0xE10);
            this.AddImage(0, 0, 0x2C96);

            if (this.m_Aquarium.Items.Count == 0)
                return;

            for (int i = 1; i <= this.m_Aquarium.Items.Count; i ++)
                this.DisplayPage(i, edit);
        }

        public virtual void DisplayPage(int page, bool edit)
        {
            this.AddPage(page);

            Item item = this.m_Aquarium.Items[page - 1];

            // item name
            if (item.LabelNumber != 0)
                this.AddHtmlLocalized(20, 217, 250, 20, item.LabelNumber, 0xFFFFFF, false, false); // Name

            // item details
            if (item is BaseFish)
                this.AddHtmlLocalized(20, 239, 315, 20, ((BaseFish)item).GetDescription(), 0xFFFFFF, false, false);
            else
                this.AddHtmlLocalized(20, 239, 315, 20, 1073634, 0xFFFFFF, false, false); // An aquarium decoration

            // item image
            this.AddItem(150, 80, item.ItemID, item.Hue);

            // item number / all items
            this.AddHtml(20, 195, 250, 20, String.Format("<BASEFONT COLOR=#FFFFFF>{0}/{1}</BASEFONT>", page, this.m_Aquarium.Items.Count), false, false);

            // remove item
            if (edit)
            {
                this.AddBackground(230, 195, 100, 26, 0x13BE);
                this.AddButton(235, 200, 0x845, 0x846, page, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(260, 198, 60, 26, 1073838, 0x0, false, false); // Remove
            }

            // next page
            if (page < this.m_Aquarium.Items.Count)
            {
                this.AddButton(195, 280, 0xFA5, 0xFA7, 0, GumpButtonType.Page, page + 1);
                this.AddHtmlLocalized(230, 283, 100, 18, 1044045, 0xFFFFFF, false, false); // NEXT PAGE
            }

            // previous page
            if (page > 1)
            {
                this.AddButton(45, 280, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page - 1);
                this.AddHtmlLocalized(80, 283, 100, 18, 1044044, 0xFFFFFF, false, false); // PREV PAGE
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Aquarium == null || this.m_Aquarium.Deleted)
                return;

            bool edit = this.m_Aquarium.HasAccess(sender.Mobile);

            if (info.ButtonID > 0 && info.ButtonID <= this.m_Aquarium.Items.Count && edit)
                this.m_Aquarium.RemoveItem(sender.Mobile, info.ButtonID - 1);

            if (info.ButtonID > 0)
                sender.Mobile.SendGump(new AquariumGump(this.m_Aquarium, edit));
        }
    }
}