using System;

namespace Server.Items
{
    public class GrapeVine : Item
    {
        public override bool IsArtifact => true;
        private DateTime m_NextHarvest;
        private readonly double HarvestWait = 4;

        public override int LabelNumber => 1149954;

        [Constructable]
        public GrapeVine() : base(Utility.Random(3355, 10))
        {
            m_NextHarvest = DateTime.UtcNow;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if ((ItemID == 3358 || ItemID == 3363) && Movable == false && m_NextHarvest < DateTime.UtcNow)
            {
                from.AddToBackpack(new GrapeBunch());
                m_NextHarvest = DateTime.UtcNow + TimeSpan.FromHours(HarvestWait);
            }
        }

        public GrapeVine(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_NextHarvest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextHarvest = reader.ReadDateTime();
        }
    }
}