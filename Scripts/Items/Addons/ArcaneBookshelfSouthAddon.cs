using System;

namespace Server.Items
{
    public class ArcaneBookshelfSouthAddon : BaseAddon
    {
        [Constructable]
        public ArcaneBookshelfSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x3087), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x3086), 0, 1, 0);
        }

        public ArcaneBookshelfSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcaneBookshelfSouthDeed();
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

    public class ArcaneBookshelfSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcaneBookshelfSouthDeed()
        {
        }

        public ArcaneBookshelfSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcaneBookshelfSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072871;
            }
        }// arcane bookshelf (south)
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