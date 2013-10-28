using System;

namespace Server.Items
{
    public class CherryBlossomTrunkAddon : BaseAddon
    {
        [Constructable]
        public CherryBlossomTrunkAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0x26EE, 1076784), 0, 0, 0);
        }

        public CherryBlossomTrunkAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new CherryBlossomTrunkDeed();
            }
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

    public class CherryBlossomTrunkDeed : BaseAddonDeed
    {
        [Constructable]
        public CherryBlossomTrunkDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public CherryBlossomTrunkDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new CherryBlossomTrunkAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076784;
            }
        }// Cherry Blossom Trunk
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