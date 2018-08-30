using System;

namespace Server.Items
{
    public class FeastOakTableSouthAddon : BaseAddon
    {
        [Constructable]
        public FeastOakTableSouthAddon()
        {
            this.AddComponent(new AddonComponent(40264), 0, 0, 0);
			this.AddComponent(new AddonComponent(40263), -1, 0, 0);
			this.AddComponent(new AddonComponent(40262), 0, -1, 0);
			this.AddComponent(new AddonComponent(40261), -1, -1, 0);
			this.AddComponent(new AddonComponent(40260), 0, -2, 0);
			this.AddComponent(new AddonComponent(40259), -1, -2, 0);
			this.AddComponent(new AddonComponent(40258), 0, -3, 0);
			this.AddComponent(new AddonComponent(40257), -1, -3, 0);
			this.AddComponent(new AddonComponent(40256), 0, -4, 0);
			this.AddComponent(new AddonComponent(40255), -1, -4, 0);
        }

        public FeastOakTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FeastOakTableSouthDeed();
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

    public class FeastOakTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public FeastOakTableSouthDeed()
        {
			Name = "Feasting Oak Table South Deed";
        }

        public FeastOakTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FeastOakTableSouthAddon();
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