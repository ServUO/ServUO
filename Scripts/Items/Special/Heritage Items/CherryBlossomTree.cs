using System;

namespace Server.Items
{
    public class CherryBlossomTreeAddon : BaseAddon
    {
        [Constructable]
        public CherryBlossomTreeAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0x26EE, 1076268), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x3122, 1076268), 0, 0, 0);
        }

        public CherryBlossomTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new CherryBlossomTreeDeed();
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

    public class CherryBlossomTreeDeed : BaseAddonDeed
    {
        [Constructable]
        public CherryBlossomTreeDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public CherryBlossomTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new CherryBlossomTreeAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076268;
            }
        }// Cherry Blossom Tree
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