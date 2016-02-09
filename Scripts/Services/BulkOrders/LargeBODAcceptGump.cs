using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.BulkOrders
{
    public class LargeBODAcceptGump : Gump
    {
        public override int TypeID { get { return 0x1C7; } }
        private readonly LargeBOD m_Deed;
        private readonly Mobile m_From;

        public LargeBODAcceptGump(Mobile from, LargeBOD deed)
            : base(50, 50)
        {
            m_From = from;
            m_Deed = deed;

            m_From.CloseGump(typeof(LargeBODAcceptGump));
            m_From.CloseGump(typeof(SmallBODAcceptGump));

            LargeBulkEntry[] entries = deed.Entries;

            bool enlarge = deed.RequireExceptional || deed.Material != BulkMaterialType.None;

            AddPage(0);

            AddBackground(25, 10, 430, (enlarge ? 240 : 168) + (entries.Length * 24), 5054);

            AddImageTiled(33, 20, 413, (enlarge ? 221 : 149) + (entries.Length * 24), 2624);
            AddAlphaRegion(33, 20, 413, (enlarge ? 221 : 149) + (entries.Length * 24));

            AddImage(20, 5, 10460);
            AddImage(430, 5, 10460);
            AddImage(20, (enlarge ? 225 : 153) + (entries.Length * 24), 10460);
            AddImage(430, (enlarge ? 225 : 153) + (entries.Length * 24), 10460);

            AddHtmlLocalized(40, 48, 350, 20, 1045135, 0xFFFFFF, false, false); // Ah!  Thanks for the goods!  Would you help me out?
            AddHtmlLocalized(40, 72, 210, 20, 1045138, 0xFFFFFF, false, false); // Amount to make:
            AddLabel(250, 72, 1152, deed.AmountMax.ToString());

            AddHtmlLocalized(40, (enlarge ? 192 : 120) + (entries.Length * 24), 350, 20, 1045139, 0xFFFFFF, false, false); // Do you want to accept this order?

            AddHtmlLocalized(135, (enlarge ? 216 : 144) + (entries.Length * 24), 120, 20, 1006044, 0xFFFFFF, false, false); // OK
            AddHtmlLocalized(310, (enlarge ? 216 : 144) + (entries.Length * 24), 120, 20, 1011012, 0xFFFFFF, false, false); // CANCEL

            AddButton(100, (enlarge ? 216 : 144) + (entries.Length * 24), 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddButton(275, (enlarge ? 216 : 144) + (entries.Length * 24), 4005, 4007, 0, GumpButtonType.Reply, 0);

            AddHtmlLocalized(180, 25, 120, 20, 1045134, 0xFFFFFF, false, false); // A large bulk order
            AddHtmlLocalized(40, 96, 120, 20, 1045137, 0xFFFFFF, false, false); // Items requested:

            int y = 120 + entries.Length * 24;

            if (enlarge)
            {
                int krobjects = 2;

                AddHtmlLocalized(40, y, 210, 20, 1045140, 0xFFFFFF, false, false); // Special requirements to meet:
                y += 24;

                if (deed.RequireExceptional)
                {
                    AddHtmlLocalized(40, y, 350, 20, 1045141, 0xFFFFFF, false, false); // All items must be exceptional.
                    y += 24;
                    krobjects--;
                }

                if (deed.Material != BulkMaterialType.None)
                {
                    AddHtmlLocalized(40, y, 350, 20, GetMaterialNumberFor(deed.Material), 0xFFFFFF, false, false); // All items must be made with x material.
                    y += 24;
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

            y = 120;

            for (int i = 0; i < entries.Length; ++i, y += 24)
            {
                AddHtmlLocalized(40, y, 210, 20, entries[i].Details.Number, 0xFFFFFF, false, false);
            }
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