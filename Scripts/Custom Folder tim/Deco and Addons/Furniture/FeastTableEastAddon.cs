using System;

namespace Server.Items
{
    public class FeastTableEastAddon : BaseAddon
    {
        [Constructable]
        public FeastTableEastAddon()
        {
            this.AddComponent(new AddonComponent(40307), 0, 0, 0);
			this.AddComponent(new AddonComponent(40308), 0, 1, 0);
			this.AddComponent(new AddonComponent(40309), 1, 0, 0);
			this.AddComponent(new AddonComponent(40310), 1, 1, 0);
			this.AddComponent(new AddonComponent(40311), 2, 0, 0);
			this.AddComponent(new AddonComponent(40312), 2, 1, 0);
			this.AddComponent(new AddonComponent(40313), 3, 0, 0);
			this.AddComponent(new AddonComponent(40314), 3, 1, 0);
			this.AddComponent(new AddonComponent(40315), 4, 0, 0);
			this.AddComponent(new AddonComponent(40316), 4, 1, 0);
			this.AddComponent(new AddonComponent(40317), 5, 0, 0);
			this.AddComponent(new AddonComponent(40318), 5, 1, 0);
        }

        public FeastTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FeastTableEastDeed();
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

    public class FeastTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public FeastTableEastDeed()
        {
			Name = "Feasting Table East Deed";
        }

        public FeastTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FeastTableEastAddon();
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