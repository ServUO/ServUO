using System;
using Server;

namespace Server.Items
{
    public class ElvenWashBasinEastAddon : BaseAddonContainer
    {
        [Constructable]
        public ElvenWashBasinEastAddon()
			: base(0x30E0)
        {
            this.AddComponent(new AddonContainerComponent(0x30DF), 0, -1, 0);
        }

        public ElvenWashBasinEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new ElvenWashBasinEastDeed();
            }
        }
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
				return 0x0104;
			}
		}
		public override int DefaultDropSound
		{
			get
			{
				return 0x0042;
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

    public class ElvenWashBasinEastDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public ElvenWashBasinEastDeed()
			: base()
        {
        }

        public ElvenWashBasinEastDeed(Serial serial)
            : base(serial)
        {
        }

		public override BaseAddonContainer Addon
		{
			get
			{
				return new ElvenWashBasinEastAddon();
			}
		}

        public override int LabelNumber
        {
            get
            {
                return 1073387;
            }
        }// elven wash basin (east)
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