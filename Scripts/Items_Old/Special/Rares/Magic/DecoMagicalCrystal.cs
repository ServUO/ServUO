using System;

namespace Server.Items
{
    public class DecoMagicalCrystal : Item
    {
        [Constructable]
        public DecoMagicalCrystal()
            : base(0x1F19)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoMagicalCrystal(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}