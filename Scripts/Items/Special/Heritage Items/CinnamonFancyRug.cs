using System;

namespace Server.Items
{
    public class CinnamonFancyRugAddon : BaseAddon
    {
        [Constructable]
        public CinnamonFancyRugAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xAE3, 1076587), 1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE4, 1076587), -1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE5, 1076587), -1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE6, 1076587), 1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE7, 1076587), -1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE8, 1076587), 0, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE9, 1076587), 1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAEA, 1076587), 0, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAEB, 1076587), 0, 0, 0);
        }

        public CinnamonFancyRugAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new CinnamonFancyRugDeed();
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

    public class CinnamonFancyRugDeed : BaseAddonDeed
    {
        [Constructable]
        public CinnamonFancyRugDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public CinnamonFancyRugDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new CinnamonFancyRugAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076587;
            }
        }// Cinnamon fancy rug
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