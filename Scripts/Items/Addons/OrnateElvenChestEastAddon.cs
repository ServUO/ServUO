using System;

namespace Server.Items
{
    public class OrnateElvenChestEastAddon : BaseAddonContainer
    {
        [Constructable]
        public OrnateElvenChestEastAddon()
            : base(0x309A)
        {
            this.AddComponent(new LocalizedContainerComponent(0x309B, 1073383), 0, -1, 0);
        }

        public OrnateElvenChestEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new OrnateElvenChestEastDeed();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073383;
            }
        }// ornate elven chest (east)
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override int DefaultGumpID
        {
            get
            {
                return 0x10C;
            }
        }
        public override int DefaultDropSound
        {
            get
            {
                return 0x42;
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

    public class OrnateElvenChestEastDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public OrnateElvenChestEastDeed()
            : base()
        {
        }

        public OrnateElvenChestEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon
        {
            get
            {
                return new OrnateElvenChestEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073383;
            }
        }// ornate elven chest (east)
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