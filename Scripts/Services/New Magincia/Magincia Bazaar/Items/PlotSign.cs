using Server.ContextMenus;
using System;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class PlotSign : Item
    {
        public static readonly int RuneCost = 100;

        private MaginciaBazaarPlot m_Plot;

        [CommandProperty(AccessLevel.GameMaster)]
        public MaginciaBazaarPlot Plot
        {
            get { return m_Plot; }
            set { m_Plot = value; InvalidateProperties(); }
        }

        public override bool DisplayWeight => false;

        public PlotSign(MaginciaBazaarPlot plot)
            : base(3025)
        {
            Movable = false;
            m_Plot = plot;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Plot == null || !m_Plot.Active)
            {
                from.SendMessage("New Magincia Bazaar Plot {0} is inactive at this time.", m_Plot.PlotDef.ID);
            }
            else if (from.InRange(Location, 3))
            {
                from.CloseGump(typeof(BaseBazaarGump));
                from.SendGump(new StallLeasingGump(from, m_Plot));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Plot == null)
                return;

            if (m_Plot.ShopName != null)
                list.Add(1062449, m_Plot.ShopName); // Shop Name: ~1_NAME~

            if (m_Plot.Merchant != null)
                list.Add(1150529, m_Plot.Merchant.Name); // Proprietor: ~1_NAME~

            if (m_Plot.Auction != null)
            {
                int left = 1;
                if (m_Plot.Auction.AuctionEnd > DateTime.UtcNow)
                {
                    TimeSpan ts = m_Plot.Auction.AuctionEnd - DateTime.UtcNow;
                    left = (int)(ts.TotalHours + 1);
                }

                list.Add(1150533, left.ToString()); // Auction for Lease Ends Within ~1_HOURS~ Hours
            }

            if (!m_Plot.Active)
                list.Add(1153036); // Inactive
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Plot == null)
                list.Add(1150530, "unknown"); // Stall ~1_NAME~
            else
                list.Add(1150530, m_Plot.PlotDef != null ? m_Plot.PlotDef.ID : "unknown"); // Stall ~1_NAME~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_Plot != null && m_Plot.Active)
                list.Add(new RecallRuneEntry(from, this));
        }

        private class RecallRuneEntry : ContextMenuEntry
        {
            private readonly PlotSign m_Sign;
            private readonly Mobile m_From;

            public RecallRuneEntry(Mobile from, PlotSign sign)
                : base(1151508, -1)
            {
                m_Sign = sign;
                m_From = from;

                Enabled = from.InRange(sign.Location, 2);
            }

            public override void OnClick()
            {
                m_From.SendGump(new ShopRecallRuneGump(m_From, m_Sign));
            }
        }

        public PlotSign(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}