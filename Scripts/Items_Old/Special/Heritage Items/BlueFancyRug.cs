using System;

namespace Server.Items
{
    public class BlueFancyRugAddon : BaseAddon
    {
        [Constructable]
        public BlueFancyRugAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xAC2, 1076273), 1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAC3, 1076273), -1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAC4, 1076273), -1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAC5, 1076273), 1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAF6, 1076273), -1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAF7, 1076273), 0, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAF8, 1076273), 1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAF9, 1076273), 0, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAFA, 1076273), 0, 0, 0);
        }

        public BlueFancyRugAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BlueFancyRugDeed();
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

    public class BlueFancyRugDeed : BaseAddonDeed
    {
        [Constructable]
        public BlueFancyRugDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public BlueFancyRugDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BlueFancyRugAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076273;
            }
        }// Blue fancy rug
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