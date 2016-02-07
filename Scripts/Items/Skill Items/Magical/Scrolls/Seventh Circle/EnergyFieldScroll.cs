using System;

namespace Server.Items
{
    public class EnergyFieldScroll : SpellScroll
    {
        [Constructable]
        public EnergyFieldScroll()
            : this(1)
        {
        }

        [Constructable]
        public EnergyFieldScroll(int amount)
            : base(49, 0x1F5E, amount)
        {
        }

        public EnergyFieldScroll(Serial serial)
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