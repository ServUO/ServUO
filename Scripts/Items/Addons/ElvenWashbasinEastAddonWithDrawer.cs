using System;
using Server;

namespace Server.Items
{
    public class ElvenWashBasinEastAddonWithDrawer : BaseAddonContainer
    {
        [Constructable]
        public ElvenWashBasinEastAddonWithDrawer()
			: base(0x30E0)
        {
            this.AddComponent(new AddonContainerComponent(0x30DF), 0, -1, 0);
        }

        public ElvenWashBasinEastAddonWithDrawer(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new ElvenWashBasinEastWithDrawerDeed();
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

    public class ElvenWashBasinEastWithDrawerDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public ElvenWashBasinEastWithDrawerDeed()
			: base()
        {
        }

        public ElvenWashBasinEastWithDrawerDeed(Serial serial)
            : base(serial)
        {
        }

		public override BaseAddonContainer Addon
		{
			get
			{
				return new ElvenWashBasinEastAddonWithDrawer();
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