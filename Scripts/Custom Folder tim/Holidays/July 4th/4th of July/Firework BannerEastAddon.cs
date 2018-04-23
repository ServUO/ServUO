using System;

namespace Server.Items
{
    public class FireworkBannerEastAddon : BaseAddon
    {
        [Constructable]
        public FireworkBannerEastAddon()
        {
            this.AddComponent(new AddonComponent(40140), 1, 0, 6);
			this.AddComponent(new AddonComponent(40141), 1, 1, 6);
        }

        public FireworkBannerEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FireworkBannerEastDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FireworkBannerEastDeed : BaseAddonDeed
    {
        [Constructable]
        public FireworkBannerEastDeed()
        {
			Name = "Feasting Oak Table East Deed";
        }

        public FireworkBannerEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FireworkBannerEastAddon();
            }
        }
        /*public override int LabelNumber
        {
            get
            {
                return 1044324;
            }
        }// large bed (east)*/
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}