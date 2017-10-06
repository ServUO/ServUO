using System;
using Server;

namespace Server.Items
{
    public class PlushLoveseatEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new PlushLoveseatEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public PlushLoveseatEastAddon()
        {
            AddComponent(new AddonComponent(0x4C84), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C85), 0, 1, 0);
        }

        public PlushLoveseatEastAddon(Serial serial)
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

    public class PlushLoveseatEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new PlushLoveseatEastAddon(); } }
        public override int LabelNumber { get { return 1154136; } } // Plush Loveseat (East)

        [Constructable]
        public PlushLoveseatEastDeed()
        {
        }

        public PlushLoveseatEastDeed(Serial serial)
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