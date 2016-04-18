using System;

namespace Server.Items
{
    public class GargishCotEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new GargishCotEastDeed();
            }
        }

        #region Mondain's Legacy
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        #endregion

        [Constructable]
        public GargishCotEastAddon()
        {
            this.AddComponent(new AddonComponent(0x400E), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x400F), 1, 0, 0);
        }

        public GargishCotEastAddon(Serial serial)
            : base(serial)
        {
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

    public class GargishCotEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new GargishCotEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1111921;
            }
        }// gargish cot (east)

        [Constructable]
        public GargishCotEastDeed()
        {
        }

        public GargishCotEastDeed(Serial serial)
            : base(serial)
        {
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
}