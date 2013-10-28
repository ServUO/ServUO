using System;

namespace Server.Items
{
    public class StoneAnvilEastAddon : BaseAddon
    {
        [Constructable]
        public StoneAnvilEastAddon()
        {
            this.AddComponent(new AnvilComponent(0x2DD6), 0, 0, 0);
        }

        public StoneAnvilEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new StoneAnvilEastDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
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

    public class StoneAnvilEastDeed : BaseAddonDeed
    {
        [Constructable]
        public StoneAnvilEastDeed()
        {
        }

        public StoneAnvilEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new StoneAnvilEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073392;
            }
        }// stone anvil (east)
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