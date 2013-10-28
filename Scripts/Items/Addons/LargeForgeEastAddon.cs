using System;

namespace Server.Items
{
    public class LargeForgeEastAddon : BaseAddon
    {
        [Constructable]
        public LargeForgeEastAddon()
        {
            this.AddComponent(new ForgeComponent(0x1986), 0, 0, 0);
            this.AddComponent(new ForgeComponent(0x198A), 0, 1, 0);
            this.AddComponent(new ForgeComponent(0x1996), 0, 2, 0);
            this.AddComponent(new ForgeComponent(0x1992), 0, 3, 0);
        }

        public LargeForgeEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LargeForgeEastDeed();
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

    public class LargeForgeEastDeed : BaseAddonDeed
    {
        [Constructable]
        public LargeForgeEastDeed()
        {
        }

        public LargeForgeEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LargeForgeEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044331;
            }
        }// large forge (east)
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