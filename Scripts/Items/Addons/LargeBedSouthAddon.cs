using System;

namespace Server.Items
{
    public class LargeBedSouthAddon : BaseAddon
    {
        [Constructable]
        public LargeBedSouthAddon()
        {
            this.AddComponent(new AddonComponent(0xA83), 0, 0, 0);
            this.AddComponent(new AddonComponent(0xA7F), 0, 1, 0);
            this.AddComponent(new AddonComponent(0xA82), 1, 0, 0);
            this.AddComponent(new AddonComponent(0xA7E), 1, 1, 0);
        }

        public LargeBedSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LargeBedSouthDeed();
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

    public class LargeBedSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public LargeBedSouthDeed()
        {
        }

        public LargeBedSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LargeBedSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044323;
            }
        }// large bed (south)
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