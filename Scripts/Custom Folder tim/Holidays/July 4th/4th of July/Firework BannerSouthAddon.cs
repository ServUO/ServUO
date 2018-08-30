using System;

namespace Server.Items
{
    public class FireworkBannerSouthAddon : BaseAddon
    {
        [Constructable]
        public FireworkBannerSouthAddon()
        {
            this.AddComponent(new AddonComponent(40122), 0, 0, 6);
			this.AddComponent(new AddonComponent(40121), -1, 0, 6);
        }

        public FireworkBannerSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FireworkBannerSouthDeed();
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

    public class FireworkBannerSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public FireworkBannerSouthDeed()
        {
			Name = "A Firework Banner Deed (South)";
        }

        public FireworkBannerSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FireworkBannerSouthAddon();
            }
        }
        /*public override int LabelNumber
        {
            get
            {
                return 1044323;
            }
        }// large bed (south)*/
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