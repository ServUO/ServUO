using System;

namespace Server.Items
{
    public class Vase : Item
    {
        [Constructable]
        public Vase()
            : base(0xB46)
        {
            this.Weight = 10;
        }

        public Vase(Serial serial)
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

    public class LargeVase : Item
    {
        [Constructable]
        public LargeVase()
            : base(0xB45)
        {
            this.Weight = 15;
        }

        public LargeVase(Serial serial)
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

    public class SmallUrn : Item
    {
        [Constructable]
        public SmallUrn()
            : base(0x241C)
        {
            this.Weight = 20.0;
        }

        public SmallUrn(Serial serial)
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