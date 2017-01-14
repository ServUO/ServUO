using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.NewMagincia
{
    public class MaginciaPlotStone : Item
    {
        public override bool ForceShowProperties { get { return true; } }

        private MaginciaHousingPlot m_Plot;

        [CommandProperty(AccessLevel.GameMaster)]
        public MaginciaHousingPlot Plot 
        { 
            get { return m_Plot; } 
            set { m_Plot = value; } 
        }

        [Constructable]
        public MaginciaPlotStone() : base(3805)
        {
            Movable = false;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1150494, m_Plot != null ? m_Plot.Identifier : "Unknown"); // lot ~1_PLOTID~
        }

        public override void OnDoubleClick(Mobile from)
        {
            MaginciaLottoSystem system = MaginciaLottoSystem.Instance;

            if (system == null || !system.Enabled || m_Plot == null)
                return;

            if (from.InRange(this.Location, 4))
            {
                if (m_Plot.LottoOngoing)
                {
                    from.CloseGump(typeof(MaginciaLottoGump));
                    from.SendGump(new MaginciaLottoGump(from, m_Plot));
                }
                else if (!m_Plot.IsAvailable)
                    from.SendMessage("The lottory for this lot has ended.");
                else
                    from.SendMessage("The lottory for this lot has expired.  Check back soon!");
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Plot != null)
                MaginciaLottoSystem.UnregisterPlot(m_Plot);

            base.OnAfterDelete();
        }

        public MaginciaPlotStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}