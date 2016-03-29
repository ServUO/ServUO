using System;

namespace Server.Items
{
    [FlipableAttribute(0x1f81, 0x1f82, 0x1f83, 0x1f84)]
    public class Glass : Item
    {
        [Constructable]
        public Glass()
            : base(0x1f81)
        {
            this.Weight = 0.1;
        }

        public Glass(Serial serial)
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

    public class GlassBottle : Item
    {
        [Constructable]
        public GlassBottle()
            : base(0xe2b)
        {
            this.Weight = 0.3;
        }

        public GlassBottle(Serial serial)
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