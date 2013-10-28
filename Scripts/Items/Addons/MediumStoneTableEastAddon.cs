using System;

namespace Server.Items
{
    public class MediumStoneTableEastAddon : BaseAddon
    {
        [Constructable]
        public MediumStoneTableEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public MediumStoneTableEastAddon(int hue)
        {
            this.AddComponent(new AddonComponent(0x1202), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1201), 0, 1, 0);
            this.Hue = hue;
        }

        public MediumStoneTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new MediumStoneTableEastDeed();
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MediumStoneTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public MediumStoneTableEastDeed()
        {
        }

        public MediumStoneTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new MediumStoneTableEastAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044508;
            }
        }// stone table (east)
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