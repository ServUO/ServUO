using System;

namespace Server.Items
{
    public class GargishCouchSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new GargishCouchSouthDeed();
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
        public GargishCouchSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x4027), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x4028), 1, 0, 0);
        }

        public GargishCouchSouthAddon(Serial serial)
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

    public class GargishCouchSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new GargishCouchSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1111775;
            }
        }// gargish couch (South)

        [Constructable]
        public GargishCouchSouthDeed()
        {
        }

        public GargishCouchSouthDeed(Serial serial)
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