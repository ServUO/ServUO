using System;

namespace Server.Items
{
    public class StoneOvenEastAddon : BaseAddon
    {
        [Constructable]
        public StoneOvenEastAddon()
        {
            this.AddComponent(new AddonComponent(0x92C), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x92B), 0, 1, 0);
        }

        public StoneOvenEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new StoneOvenEastDeed();
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

    public class StoneOvenEastDeed : BaseAddonDeed
    {
        [Constructable]
        public StoneOvenEastDeed()
        {
        }

        public StoneOvenEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new StoneOvenEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044345;
            }
        }// stone oven (east)
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