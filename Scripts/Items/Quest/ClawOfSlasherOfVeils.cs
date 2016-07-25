using System;

namespace Server.Items
{
    public class ClawOfSlasherOfVeils : Item
    {
        public override int LabelNumber { get { return 1031704; } } // Claw of Slasher of Veils

        [Constructable]
        public ClawOfSlasherOfVeils()
            : base(0x2DB8)
        {
            this.Weight = 10.0;
            this.Stackable = false;
        }

        public ClawOfSlasherOfVeils(Serial serial)
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