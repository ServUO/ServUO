using System;

namespace Server.Items
{
    public class SandstoneFireplaceSouthAddon : BaseAddon
    {
        [Constructable]
        public SandstoneFireplaceSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x482), -1, 0, 0);
            this.AddComponent(new AddonComponent(0x47B), 0, 0, 0);
        }

        public SandstoneFireplaceSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SandstoneFireplaceSouthDeed();
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

    public class SandstoneFireplaceSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public SandstoneFireplaceSouthDeed()
        {
        }

        public SandstoneFireplaceSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SandstoneFireplaceSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1061845;
            }
        }// sandstone fireplace (south)
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