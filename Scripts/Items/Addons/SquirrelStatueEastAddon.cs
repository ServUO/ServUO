using System;

namespace Server.Items
{
    public class SquirrelStatueEastAddon : BaseAddon
    {
        [Constructable]
        public SquirrelStatueEastAddon()
        {
            this.AddComponent(new AddonComponent(0x2D10), 0, 0, 0);
        }

        public SquirrelStatueEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SquirrelStatueEastDeed();
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

    public class SquirrelStatueEastDeed : BaseAddonDeed
    {
        [Constructable]
        public SquirrelStatueEastDeed()
        {
        }

        public SquirrelStatueEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SquirrelStatueEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073398;
            }
        }// squirrel statue (east)
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