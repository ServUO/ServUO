using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.BulkOrders
{
    public class SmallBODAcceptGump : Gump
    {
        public override int TypeID { get { return 0x1C7; } }
        private readonly SmallBOD m_Deed;
        private readonly Mobile m_From;

        public SmallBODAcceptGump(Mobile from, SmallBOD deed)
            : base(50, 50)
        {
            m_From = from;
            m_Deed = deed;

            m_From.CloseGump(typeof(LargeBODAcceptGump));
            m_From.CloseGump(typeof(SmallBODAcceptGump));

            bool enlarge = deed.RequireExceptional || deed.Material != BulkMaterialType.None;

            AddPage(0);

            AddBackground(25, 10, 430, enlarge ? 264 : 192, 5054);

            AddImageTiled(33, 20, 413, enlarge ? 245 : 173, 2624);
            AddAlphaRegion(33, 20, 413, enlarge ? 245 : 173);

            AddImage(20, 5, 10460);
            AddImage(430, 5, 10460);
            AddImage(20, enlarge ? 249 : 177, 10460);
            AddImage(430, enlarge ? 249 : 177, 10460);

            AddHtmlLocalized(40, 48, 350, 20, 1045135, 0xFFFFFF, false, false); // Ah!  Thanks for the goods!  Would you help me out?
            AddHtmlLocalized(40, 72, 210, 20, 1045138, 0xFFFFFF, false, false); // Amount to make:
            AddLabel(250, 72, 1152, deed.AmountMax.ToString());

            AddHtmlLocalized(40, enlarge ? 216 : 144, 350, 20, 1045139, 0xFFFFFF, false, false); // Do you want to accept this order?
            AddHtmlLocalized(135, enlarge ? 240 : 168, 120, 20, 1006044, 0xFFFFFF, false, false); // OK
            AddHtmlLocalized(310, enlarge ? 240 : 168, 120, 20, 1011012, 0xFFFFFF, false, false); // CANCEL

            AddButton(100, enlarge ? 240 : 168, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddButton(275, enlarge ? 240 : 168, 4005, 4007, 0, GumpButtonType.Reply, 0);

            AddHtmlLocalized(190, 25, 120, 20, 1045133, 0xFFFFFF, false, false); // A bulk order
            AddHtmlLocalized(40, 96, 120, 20, 1045136, 0xFFFFFF, false, false); // Item requested:
            AddItem(315, 96, deed.Graphic);

            if (enlarge)
            {
                int krobjects = 2;

                AddHtmlLocalized(40, 144, 210, 20, 1045140, 0xFFFFFF, false, false); // Special requirements to meet:

                if (deed.RequireExceptional)
                {
                    // All items must be exceptional.
                    AddHtmlLocalized(40, 168, 350, 20, 1045141, 0xFFFFFF, false, false);
                    krobjects--;
                }

                if (deed.Material != BulkMaterialType.None)
                {
                    // All items must be made with x material.
                    AddHtmlLocalized(40, deed.RequireExceptional ? 192 : 168, 350, 20, GetMaterialNumberFor(deed.Material), 0xFFFFFF, false, false);
                    krobjects--;
                }

                while (krobjects > 0)
                {
                    AddECHtmlLocalized(0, 0, 0, 0, -1, false, false);
                    krobjects--;
                }
            }
            else
            {
                AddECHtmlLocalized(0, 0, 0, 0, -1, false, false);
                AddECHtmlLocalized(0, 0, 0, 0, -1, false, false);
                AddECHtmlLocalized(0, 0, 0, 0, -1, false, false);
            }

            AddECHtmlLocalized(0, 0, 0, 0, -1, false, false);

            AddHtmlLocalized(40, 120, 210, 20, deed.Number, 0xFFFFFF, false, false);
        }

        public static int GetMaterialNumberFor(BulkMaterialType material)
        {
            if (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite)
            {
                return 1045142 + (int)(material - BulkMaterialType.DullCopper);
            }
            else if (material >= BulkMaterialType.Spined && material <= BulkMaterialType.Barbed)
            {
                return 1049348 + (int)(material - BulkMaterialType.Spined);
            }

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