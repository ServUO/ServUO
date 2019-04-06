using System;
using Server;

namespace Server.Items
{
    public class SmallDisplayCaseSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallDisplayCaseSouthDeed(); } }

        [Constructable]
        public SmallDisplayCaseSouthAddon()
        {
            AddComponent(new AddonComponent(0x0B0A), 0, 0, 0);
            AddComponent(new AddonComponent(0x0B0C), 0, 0, 3);
        }

        public SmallDisplayCaseSouthAddon(Serial serial)
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

    public class SmallDisplayCaseSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallDisplayCaseSouthAddon(); } }
        public override int LabelNumber { get { return 1155842; } } // Small Display Case (South)

        [Constructable]
        public SmallDisplayCaseSouthDeed()
        {
        }

        public SmallDisplayCaseSouthDeed(Serial serial)
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