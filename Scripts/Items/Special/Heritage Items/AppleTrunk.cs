using System;

namespace Server.Items
{
    public class AppleTrunkAddon : BaseAddon
    {
        [Constructable]
        public AppleTrunkAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xD98, 1076785), 0, 0, 0);
        }

        public AppleTrunkAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AppleTrunkDeed();
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

    public class AppleTrunkDeed : BaseAddonDeed
    {
        [Constructable]
        public AppleTrunkDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public AppleTrunkDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AppleTrunkAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076785;
            }
        }// Apple Trunk
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