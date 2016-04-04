using System;

namespace Server.Items
{
    public class OrnateElvenTableEastAddon : BaseAddon
    {
        [Constructable]
        public OrnateElvenTableEastAddon()
        {
			this.AddComponent(new AddonComponent(0x308F), 0, 1, 0);
			this.AddComponent(new AddonComponent(0x3090), 0, 0, 0);
			this.AddComponent(new AddonComponent(0x3091), 0, -1, 0);
		}

        public OrnateElvenTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new OrnateElvenTableEastDeed();
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

    public class OrnateElvenTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public OrnateElvenTableEastDeed()
        {
        }

        public OrnateElvenTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new OrnateElvenTableEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073384;
            }
        }// ornate table (east)
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