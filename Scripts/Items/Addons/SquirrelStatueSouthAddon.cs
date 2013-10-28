using System;

namespace Server.Items
{
    public class SquirrelStatueSouthAddon : BaseAddon
    {
        [Constructable]
        public SquirrelStatueSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x2D11), 0, 0, 0);
        }

        public SquirrelStatueSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SquirrelStatueSouthDeed();
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

    public class SquirrelStatueSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public SquirrelStatueSouthDeed()
        {
        }

        public SquirrelStatueSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SquirrelStatueSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072884;
            }
        }// squirrel statue (south)
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