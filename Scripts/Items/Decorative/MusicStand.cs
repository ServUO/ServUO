using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0xEBB, 0xEBC)]
    public class TallMusicStand : Item
    {
        [Constructable]
        public TallMusicStand()
            : base(0xEBB)
        {
            this.Weight = 10.0;
        }

        public TallMusicStand(Serial serial)
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

            if (this.Weight == 8.0)
                this.Weight = 10.0;
        }
    }

    [Furniture]
    [Flipable(0xEB6, 0xEB8)]
    public class ShortMusicStand : Item
    {
        [Constructable]
        public ShortMusicStand()
            : base(0xEB6)
        {
            this.Weight = 10.0;
        }

        public ShortMusicStand(Serial serial)
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

            if (this.Weight == 6.0)
                this.Weight = 10.0;
        }
    }
}