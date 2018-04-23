using System;

namespace Server.Items
{
    public class FeastTableSouthAddon : BaseAddon
    {
        [Constructable]
        public FeastTableSouthAddon()
        {
            this.AddComponent(new AddonComponent(40306), 0, 0, 0);
			this.AddComponent(new AddonComponent(40305), -1, 0, 0);
			this.AddComponent(new AddonComponent(40304), 0, -1, 0);
			this.AddComponent(new AddonComponent(40303), -1, -1, 0);
			this.AddComponent(new AddonComponent(40302), 0, -2, 0);
			this.AddComponent(new AddonComponent(40301), -1, -2, 0);
			this.AddComponent(new AddonComponent(40300), 0, -3, 0);
			this.AddComponent(new AddonComponent(40299), -1, -3, 0);
			this.AddComponent(new AddonComponent(40298), 0, -4, 0);
			this.AddComponent(new AddonComponent(40297), -1, -4, 0);
			this.AddComponent(new AddonComponent(40296), 0, -5, 0);
			this.AddComponent(new AddonComponent(40295), -1, -5, 0);
        }

        public FeastTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FeastTableSouthDeed();
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

    public class FeastTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public FeastTableSouthDeed()
        {
			Name = "Feasting Table South Deed";
        }

        public FeastTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FeastTableSouthAddon();
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