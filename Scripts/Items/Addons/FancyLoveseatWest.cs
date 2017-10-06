using System;
using Server;

namespace Server.Items
{
    public class FancyLoveseatWestAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new FancyLoveseatWestDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public FancyLoveseatWestAddon()
        {
            AddComponent(new AddonComponent(0x9C58), 0, 0, 0);
            AddComponent(new AddonComponent(0x9C57), 0, 1, 0);
        }

        public FancyLoveseatWestAddon(Serial serial)
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

    public class FancyLoveseatWestDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new FancyLoveseatWestAddon(); } }
        public override int LabelNumber { get { return 1156561; } } // Fancy Loveseat (West)

        [Constructable]
        public FancyLoveseatWestDeed()
        {
        }

        public FancyLoveseatWestDeed(Serial serial)
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