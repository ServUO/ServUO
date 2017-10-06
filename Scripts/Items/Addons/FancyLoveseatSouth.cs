using System;
using Server;

namespace Server.Items
{
    public class FancyLoveseatSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new FancyLoveseatSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public FancyLoveseatSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C87), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C86), 1, 0, 0);
        }

        public FancyLoveseatSouthAddon(Serial serial)
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

    public class FancyLoveseatSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new FancyLoveseatSouthAddon(); } }
        public override int LabelNumber { get { return 1154137; } } // Fancy Loveseat (South)

        [Constructable]
        public FancyLoveseatSouthDeed()
        {
        }

        public FancyLoveseatSouthDeed(Serial serial)
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