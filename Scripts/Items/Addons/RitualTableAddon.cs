using System;

namespace Server.Items
{
    public class RitualTableAddon : BaseAddon
    {
        [Constructable]
        public RitualTableAddon()
            : this(0)
        {
        }

        [Constructable]
        public RitualTableAddon(int hue)
        {
            AddComponent(new AddonComponent(0x4985), 0, 0, 0);
            AddComponent(new AddonComponent(0x4984), 0, 1, 0);
            AddComponent(new AddonComponent(0x4983), 1, 0, 0);
            AddComponent(new AddonComponent(0x4982), 1, 1, 0);
            Hue = hue;
        }

        public RitualTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new RitualTableDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RitualTableDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1097690; } } // ritual table

        [Constructable]
        public RitualTableDeed()
        {
        }

        public RitualTableDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new RitualTableAddon(Hue); } }

        
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