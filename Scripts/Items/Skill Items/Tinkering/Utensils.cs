using System;

namespace Server.Items
{
    [Flipable(0x9F4, 0x9F5, 0x9A3, 0x9A4)]
    public class Fork : Item
    {
        [Constructable]
        public Fork()
            : base(0x9F4)
        {
            this.Weight = 1.0;
        }

        public Fork(Serial serial)
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

    public class ForkLeft : Item
    {
        [Constructable]
        public ForkLeft()
            : base(0x9F4)
        {
            this.Weight = 1.0;
        }

        public ForkLeft(Serial serial)
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

    public class ForkRight : Item
    {
        [Constructable]
        public ForkRight()
            : base(0x9F5)
        {
            this.Weight = 1.0;
        }

        public ForkRight(Serial serial)
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

    [Flipable(0x9F8, 0x9F9, 0x9C2, 0x9C3)]
    public class Spoon : Item
    {
        [Constructable]
        public Spoon()
            : base(0x9F8)
        {
            this.Weight = 1.0;
        }

        public Spoon(Serial serial)
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

    public class SpoonLeft : Item
    {
        [Constructable]
        public SpoonLeft()
            : base(0x9F8)
        {
            this.Weight = 1.0;
        }

        public SpoonLeft(Serial serial)
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

    public class SpoonRight : Item
    {
        [Constructable]
        public SpoonRight()
            : base(0x9F9)
        {
            this.Weight = 1.0;
        }

        public SpoonRight(Serial serial)
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

    [Flipable(0x9F6, 0x9F7, 0x9A5, 0x9A6)]
    public class Knife : Item
    {
        [Constructable]
        public Knife()
            : base(0x9F6)
        {
            this.Weight = 1.0;
        }

        public Knife(Serial serial)
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

    public class KnifeLeft : Item
    {
        [Constructable]
        public KnifeLeft()
            : base(0x9F6)
        {
            this.Weight = 1.0;
        }

        public KnifeLeft(Serial serial)
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

    public class KnifeRight : Item
    {
        [Constructable]
        public KnifeRight()
            : base(0x9F7)
        {
            this.Weight = 1.0;
        }

        public KnifeRight(Serial serial)
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

    public class Plate : Item
    {
        [Constructable]
        public Plate()
            : base(0x9D7)
        {
            this.Weight = 1.0;
        }

        public Plate(Serial serial)
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