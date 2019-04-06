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
            m_From = from;
            m_Deed = deed;

            m_From.CloseGump(typeof(LargeBODAcceptGump));
            m_From.CloseGump(typeof(SmallBODAcceptGump));

            LargeBulkEntry[] entries = deed.Entries;

            AddPage(0);

            AddBackground(25, 10, 430, 240 + (entries.Length * 24), 5054);

            AddImageTiled(33, 20, 413, 221 + (entries.Length * 24), 2624);
            AddAlphaRegion(33, 20, 413, 221 + (entries.Length * 24));

            AddImage(20, 5, 10460);
            AddImage(430, 5, 10460);
            AddImage(20, 225 + (entries.Length * 24), 10460);
            AddImage(430, 225 + (entries.Length * 24), 10460);

            AddHtmlLocalized(180, 25, 120, 20, 1045134, 0x7FFF, false, false); // A large bulk order

            AddHtmlLocalized(40, 48, 350, 20, 1045135, 0x7FFF, false, false); // Ah!  Thanks for the goods!  Would you help me out?

            AddHtmlLocalized(40, 72, 210, 20, 1045138, 0x7FFF, false, false); // Amount to make:
            AddLabel(250, 72, 1152, deed.AmountMax.ToString());

            AddHtmlLocalized(40, 96, 120, 20, 1045137, 0x7FFF, false, false); // Items requested:

            int y = 120;

            for (int i = 0; i < entries.Length; ++i, y += 24)
                AddHtmlLocalized(40, y, 210, 20, entries[i].Details.Number, 0x7FFF, false, false);

            if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
            {
                AddHtmlLocalized(40, y, 210, 20, 1045140, 0x7FFF, false, false); // Special requirements to meet:
                y += 24;

                if (deed.RequireExceptional)
                {
                    AddHtmlLocalized(40, y, 350, 20, 1045141, 0x7FFF, false, false); // All items must be exceptional.
                    y += 24;
                }

                if (deed.Material != BulkMaterialType.None)
                {
                    AddHtmlLocalized(40, y, 350, 20, SmallBODGump.GetMaterialNumberFor(deed.Material), 0x7FFF, false, false); // All items must be made with x material.
                    y += 24;
                }
            }

            AddHtmlLocalized(40, 192 + (entries.Length * 24), 350, 20, 1045139, 0x7FFF, false, false); // Do you want to accept this order?

            AddButton(100, 216 + (entries.Length * 24), 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(135, 216 + (entries.Length * 24), 120, 20, 1006044, 0x7FFF, false, false); // Ok

            AddButton(275, 216 + (entries.Length * 24), 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(310, 216 + (entries.Length * 24), 120, 20, 1011012, 0x7FFF, false, false); // CANCEL
        }

        public override void OnServerClose(NetState owner)
        {
            Timer.DelayCall(() =>
                {
                    if (m_Deed.Map == null || m_Deed.Map == Map.Internal)
                    {
                        m_Deed.Delete();
                    }
                });
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1) // Ok
            {
                if (m_From.PlaceInBackpack(m_Deed))
                {
                    m_From.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.
                }
                else
                {
                    m_From.SendLocalizedMessage(1045150); // There is not enough room in your backpack for the deed.
                    m_Deed.Delete();
                }
            }
            else
            {
                m_Deed.Delete();
            }
        }
    }
}