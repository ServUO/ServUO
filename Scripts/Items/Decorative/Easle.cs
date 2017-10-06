using System;

namespace Server.Items
{
    [Furniture]
    [TypeAlias("Server.Items.Easle")]
    public class EasleSouth : Item
    {
        [Constructable]
        public EasleSouth()
            : base(0xF65)
        {
            Weight = 25.0;
        }

        public EasleSouth(Serial serial)
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

            if (Weight == 10.0)
            {
                Weight = 25.0;
            }
        }
    }

    [Furniture]
    public class EasleEast : Item
    {
        [Constructable]
        public EasleEast()
            : base(0xF67)
        {
            Weight = 25.0;
        }

        public EasleEast(Serial serial)
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

    [Furniture]
    public class EasleNorth : Item
    {
        [Constructable]
        public EasleNorth()
            : base(0xF69)
        {
            Weight = 25.0;
        }

        public EasleNorth(Serial serial)
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