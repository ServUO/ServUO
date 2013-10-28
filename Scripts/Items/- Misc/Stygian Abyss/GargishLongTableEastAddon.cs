using System;

namespace Server.Items
{
    public class GargishLongTableEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new GargishLongTableEastDeed();
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
        public GargishLongTableEastAddon()
        {
            this.AddComponent(new AddonComponent(0x4032), 0, 1, 0);
            this.AddComponent(new AddonComponent(0x4031), 0, 0, 0);
        }

        public GargishLongTableEastAddon(Serial serial)
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

    public class GargishLongTableEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new GargishLongTableEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1111782;
            }
        }// long table

        [Constructable]
        public GargishLongTableEastDeed()
        {
        }

        public GargishLongTableEastDeed(Serial serial)
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