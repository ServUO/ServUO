using System;

namespace Server.Items
{
    public class ArcanistStatueEastAddon : BaseAddon
    {
        [Constructable]
        public ArcanistStatueEastAddon()
        {
            this.AddComponent(new AddonComponent(0x2D0E), 0, 0, 0);
        }

        public ArcanistStatueEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcanistStatueEastDeed();
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

    public class ArcanistStatueEastDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcanistStatueEastDeed()
        {
        }

        public ArcanistStatueEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcanistStatueEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072886;
            }
        }// arcanist statue (east)
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