using System;

namespace Server.Items
{
    public class EnergyVortexScroll : SpellScroll
    {
        [Constructable]
        public EnergyVortexScroll()
            : this(1)
        {
        }

        [Constructable]
        public EnergyVortexScroll(int amount)
            : base(57, 0x1F66, amount)
        {
        }

        public EnergyVortexScroll(Serial serial)
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