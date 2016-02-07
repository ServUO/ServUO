using System;

namespace Server.Items
{
    public class ArcaneBookshelfEastAddon : BaseAddon
    {
        [Constructable]
        public ArcaneBookshelfEastAddon()
        {
            this.AddComponent(new AddonComponent(0x3084), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x3085), -1, 0, 0);
        }

        public ArcaneBookshelfEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcaneBookshelfEastDeed();
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

    public class ArcaneBookshelfEastDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcaneBookshelfEastDeed()
        {
        }

        public ArcaneBookshelfEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcaneBookshelfEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073371;
            }
        }// arcane bookshelf (east)
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