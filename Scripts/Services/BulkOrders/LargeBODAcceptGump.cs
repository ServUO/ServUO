using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.BulkOrders
{
    public class LargeBODAcceptGump : Gump
    {
        private readonly LargeBOD m_Deed;
        private readonly Mobile m_From;
        public LargeBODAcceptGump(Mobile from, LargeBOD deed)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Deed = deed;

            this.m_From.CloseGump(typeof(LargeBODAcceptGump));
            this.m_From.CloseGump(typeof(SmallBODAcceptGump));

            LargeBulkEntry[] entries = deed.Entries;

            this.AddPage(0);

            this.AddBackground(25, 10, 430, 240 + (entries.Length * 24), 5054);

            this.AddImageTiled(33, 20, 413, 221 + (entries.Length * 24), 2624);
            this.AddAlphaRegion(33, 20, 413, 221 + (entries.Length * 24));

            this.AddImage(20, 5, 10460);
            this.AddImage(430, 5, 10460);
            this.AddImage(20, 225 + (entries.Length * 24), 10460);
            this.AddImage(430, 225 + (entries.Length * 24), 10460);

            this.AddHtmlLocalized(180, 25, 120, 20, 1045134, 0x7FFF, false, false); // A large bulk order

            this.AddHtmlLocalized(40, 48, 350, 20, 1045135, 0x7FFF, false, false); // Ah!  Thanks for the goods!  Would you help me out?

            this.AddHtmlLocalized(40, 72, 210, 20, 1045138, 0x7FFF, false, false); // Amount to make:
            this.AddLabel(250, 72, 1152, deed.AmountMax.ToString());

            this.AddHtmlLocalized(40, 96, 120, 20, 1045137, 0x7FFF, false, false); // Items requested:

            int y = 120;

            for (int i = 0; i < entries.Length; ++i, y += 24)
                this.AddHtmlLocalized(40, y, 210, 20, entries[i].Details.Number, 0x7FFF, false, false);

            if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
            {
                this.AddHtmlLocalized(40, y, 210, 20, 1045140, 0x7FFF, false, false); // Special requirements to meet:
                y += 24;

                if (deed.RequireExceptional)
                {
                    this.AddHtmlLocalized(40, y, 350, 20, 1045141, 0x7FFF, false, false); // All items must be exceptional.
                    y += 24;
                }

                if (deed.Material != BulkMaterialType.None)
                {
                    this.AddHtmlLocalized(40, y, 350, 20, GetMaterialNumberFor(deed.Material), 0x7FFF, false, false); // All items must be made with x material.
                    y += 24;
                }
            }

            this.AddHtmlLocalized(40, 192 + (entries.Length * 24), 350, 20, 1045139, 0x7FFF, false, false); // Do you want to accept this order?

            this.AddButton(100, 216 + (entries.Length * 24), 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(135, 216 + (entries.Length * 24), 120, 20, 1006044, 0x7FFF, false, false); // Ok

            this.AddButton(275, 216 + (entries.Length * 24), 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(310, 216 + (entries.Length * 24), 120, 20, 1011012, 0x7FFF, false, false); // CANCEL
        }

        public static int GetMaterialNumberFor(BulkMaterialType material)
        {
            if (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite)
                return 1045142 + (int)(material - BulkMaterialType.DullCopper);
            else if (material >= BulkMaterialType.Spined && material <= BulkMaterialType.Barbed)
                return 1049348 + (int)(material - BulkMaterialType.Spined);

            return 0;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1) // Ok
            {
                if (this.m_From.PlaceInBackpack(this.m_Deed))
                {
                    this.m_From.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.
                }
                else
                {
                    this.m_From.SendLocalizedMessage(1045150); // There is not enough room in your backpack for the deed.
                    this.m_Deed.Delete();
                }
            }
            else
            {
                this.m_Deed.Delete();
            }
        }
    }
}