using System;

namespace Server.Items
{
    [Furniture]
    [TypeAlias("Server.Items.TallMusicStand")]
    public class TallMusicStandLeft : Item
    {
        [Constructable]
        public TallMusicStandLeft()
            : base(0xEBB)
        {
            Weight = 10.0;
        }

        public TallMusicStandLeft(Serial serial)
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

            if (Weight == 8.0)
                Weight = 10.0;
        }
    }

    [Furniture]
    public class TallMusicStandRight : Item
    {
        [Constructable]
        public TallMusicStandRight()
            : base(0xEBC)
        {
            Weight = 10.0;
        }

        public TallMusicStandRight(Serial serial)
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
    [TypeAlias("Server.Items.ShortMusicStand")]
    public class ShortMusicStandLeft : Item
    {
        [Constructable]
        public ShortMusicStandLeft()
            : base(0xEB6)
        {
            Weight = 10.0;
        }

        public ShortMusicStandLeft(Serial serial)
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

            if (Weight == 6.0)
                Weight = 10.0;
        }
    }

    [Furniture]
    public class ShortMusicStandRight : Item
    {
        [Constructable]
        public ShortMusicStandRight()
            : base(0xEB8)
        {
            Weight = 10.0;
        }

        public ShortMusicStandRight(Serial serial)
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