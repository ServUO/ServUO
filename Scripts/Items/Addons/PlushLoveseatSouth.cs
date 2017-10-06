using System;
using Server;

namespace Server.Items
{
    public class PlushLoveseatSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new PlushLoveseatSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public PlushLoveseatSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C83), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C82), 1, 0, 0);
        }

        public PlushLoveseatSouthAddon(Serial serial)
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

    public class PlushLoveseatSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new PlushLoveseatSouthAddon(); } }
        public override int LabelNumber { get { return 1154135; } } // Plush Loveseat (South)

        [Constructable]
        public PlushLoveseatSouthDeed()
        {
        }

        public PlushLoveseatSouthDeed(Serial serial)
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