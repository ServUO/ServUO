using System;
using Server;

namespace Server.Items
{
    public class SmallDisplayCaseEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallDisplayCaseEastDeed(); } }

        [Constructable]
        public SmallDisplayCaseEastAddon()
        {
            AddComponent(new AddonComponent(0x0B09), 0, 0, 0);
            AddComponent(new AddonComponent(0x0B0B), 0, 0, 3);
        }

        public SmallDisplayCaseEastAddon(Serial serial)
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

    public class SmallDisplayCaseEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallDisplayCaseEastAddon(); } }
        public override int LabelNumber { get { return 1155843; } } // Small Display Case (East)

        [Constructable]
        public SmallDisplayCaseEastDeed()
        {
        }

        public SmallDisplayCaseEastDeed(Serial serial)
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