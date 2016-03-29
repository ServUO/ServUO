using System;

namespace Server.Items
{
    public class AbbatoirAddon : BaseAddon
    {
        [Constructable]
        public AbbatoirAddon()
        {
            this.AddComponent(new AddonComponent(0x120E), -1, -1, 0);
            this.AddComponent(new AddonComponent(0x120F), 0, -1, 0);
            this.AddComponent(new AddonComponent(0x1210), 1, -1, 0);
            this.AddComponent(new AddonComponent(0x1215), -1, 0, 0);
            this.AddComponent(new AddonComponent(0x1216), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1211), 1, 0, 0);
            this.AddComponent(new AddonComponent(0x1214), -1, 1, 0);
            this.AddComponent(new AddonComponent(0x1213), 0, 1, 0);
            this.AddComponent(new AddonComponent(0x1212), 1, 1, 0);
        }

        public AbbatoirAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AbbatoirDeed();
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

    public class AbbatoirDeed : BaseAddonDeed
    {
        [Constructable]
        public AbbatoirDeed()
        {
        }

        public AbbatoirDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AbbatoirAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044329;
            }
        }// abbatoir
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