using System;

namespace Server.Items
{
    public class GrayBrickFireplaceSouthAddon : BaseAddon
    {
        [Constructable]
        public GrayBrickFireplaceSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x94B), -1, 0, 0);
            this.AddComponent(new AddonComponent(0x945), 0, 0, 0);
        }

        public GrayBrickFireplaceSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new GrayBrickFireplaceSouthDeed();
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

    public class GrayBrickFireplaceSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public GrayBrickFireplaceSouthDeed()
        {
        }

        public GrayBrickFireplaceSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new GrayBrickFireplaceSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1061847;
            }
        }// grey brick fireplace (south)
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