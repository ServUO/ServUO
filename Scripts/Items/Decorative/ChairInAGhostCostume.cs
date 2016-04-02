using System;

namespace Server.Items
{
    public class ChairInAGhostCostume : Item
    {
        [Constructable]
        public ChairInAGhostCostume()
            : base(0x3F26)
        {
        }

        public ChairInAGhostCostume(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 5;
            }
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