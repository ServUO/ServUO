using System;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class SpikePostAddon : BaseAddon
    {
        [Constructable]
        public SpikePostAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x364D, 1076676), 0, 0, 0);
        }

        public SpikePostAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new SpikePostDeed(); } }

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    AddComponent(new LocalizedAddonComponent(0x369C, 1076676), 0, 0, 0);
                    break;
                case Direction.South:
                    AddComponent(new LocalizedAddonComponent(0x364D, 1076676), 0, 0, 0);
                    break;
            }
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

    public class SpikePostDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1076676; } } // Spike Post

        public override bool ExcludeDeedHue { get { return true; } }

        [Constructable]
        public SpikePostDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public SpikePostDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new SpikePostAddon(); } }

        
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
