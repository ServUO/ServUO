using System;

namespace Server.Items
{
    public class OrnateElvenChestSouthAddon : BaseAddonContainer
    {
        [Constructable]
        public OrnateElvenChestSouthAddon()
            : base(0x3098)
        {
            this.AddComponent(new LocalizedContainerComponent(0x3099, 1072862), -1, 0, 0);
        }

        public OrnateElvenChestSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new OrnateElvenChestSouthDeed();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072862;
            }
        }// ornate elven chest (south)
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

    public class OrnateElvenChestSouthDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public OrnateElvenChestSouthDeed()
            : base()
        {
        }

        public OrnateElvenChestSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon
        {
            get
            {
                return new OrnateElvenChestSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072862;
            }
        }// ornate elven chest (south)
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