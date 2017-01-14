using System;

namespace Server.Items
{
    public class EnergyBoltScroll : SpellScroll
    {
        [Constructable]
        public EnergyBoltScroll()
            : this(1)
        {
        }

        [Constructable]
        public EnergyBoltScroll(int amount)
            : base(41, 0x1F56, amount)
        {
        }

        public EnergyBoltScroll(Serial serial)
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