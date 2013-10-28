using System;

namespace Server.Items
{
    public class GargishCotSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new GargishCotSouthDeed();
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
        public GargishCotSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x400D), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x400C), 0, -1, 0);
        }

        public GargishCotSouthAddon(Serial serial)
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

    public class GargishCotSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new GargishCotSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1111920;
            }
        }// gargish cot (south)

        [Constructable]
        public GargishCotSouthDeed()
        {
        }

        public GargishCotSouthDeed(Serial serial)
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