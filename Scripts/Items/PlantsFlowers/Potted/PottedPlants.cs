using System;

namespace Server.Items
{
    public class PottedPlant : Item
    {
        [Constructable]
        public PottedPlant()
            : base(0x11CA)
        {
            this.Weight = 100;
        }

        public PottedPlant(Serial serial)
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

    public class PottedPlant1 : Item
    {
        [Constructable]
        public PottedPlant1()
            : base(0x11CB)
        {
            this.Weight = 100;
        }

        public PottedPlant1(Serial serial)
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

    public class PottedPlant2 : Item
    {
        [Constructable]
        public PottedPlant2()
            : base(0x11CC)
        {
            this.Weight = 100;
        }

        public PottedPlant2(Serial serial)
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