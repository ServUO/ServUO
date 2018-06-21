using System;

namespace Server.Items
{
    public class WillowTreeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new WillowTreeDeed(); } }

        [Constructable]
        public WillowTreeAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x224A, 1071105), 0, 0, 0);
        }

        public WillowTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class WillowTreeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new WillowTreeAddon(); } }
        public override int LabelNumber { get { return 1071105; } } // Willow Tree

        [Constructable]
        public WillowTreeDeed()
        {
            LootType = LootType.Blessed;
        }

        public WillowTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}