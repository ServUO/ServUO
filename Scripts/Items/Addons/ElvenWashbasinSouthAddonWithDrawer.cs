using System;

namespace Server.Items
{
    public class ElvenWashBasinSouthAddonWithDrawer : BaseAddonContainer
    {
        [Constructable]
        public ElvenWashBasinSouthAddonWithDrawer()
			: base(0x30E2)
        {
            this.AddComponent(new AddonContainerComponent(0x30E1), -1, 0, 0);
        }

        public ElvenWashBasinSouthAddonWithDrawer(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new ElvenWashBasinSouthWithDrawerDeed();
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

    public class ElvenWashBasinSouthWithDrawerDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public ElvenWashBasinSouthWithDrawerDeed()
        {
        }

        public ElvenWashBasinSouthWithDrawerDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon
        {
            get
            {
                return new ElvenWashBasinSouthAddonWithDrawer();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072865;
            }
        }// elven wash basin (south)
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