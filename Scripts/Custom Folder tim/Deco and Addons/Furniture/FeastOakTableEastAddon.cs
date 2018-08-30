using System;

namespace Server.Items
{
    public class FeastOakTableEastAddon : BaseAddon
    {
        [Constructable]
        public FeastOakTableEastAddon()
        {
            this.AddComponent(new AddonComponent(40265), 1, 0, 0);
			this.AddComponent(new AddonComponent(40266), 1, 1, 0);
			this.AddComponent(new AddonComponent(40267), 2, 0, 0);
			this.AddComponent(new AddonComponent(40268), 2, 1, 0);
			this.AddComponent(new AddonComponent(40269), 3, 0, 0);
			this.AddComponent(new AddonComponent(40270), 3, 1, 0);
			this.AddComponent(new AddonComponent(40271), 4, 0, 0);
			this.AddComponent(new AddonComponent(40272), 4, 1, 0);
			this.AddComponent(new AddonComponent(40273), 5, 0, 0);
			this.AddComponent(new AddonComponent(40274), 5, 1, 0);
        }

        public FeastOakTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FeastOakTableEastDeed();
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

    public class FeastOakTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public FeastOakTableEastDeed()
        {
			Name = "Feasting Oak Table East Deed";
        }

        public FeastOakTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FeastOakTableEastAddon();
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