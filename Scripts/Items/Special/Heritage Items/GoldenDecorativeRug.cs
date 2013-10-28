using System;

namespace Server.Items
{
    public class GoldenDecorativeRugAddon : BaseAddon
    {
        [Constructable]
        public GoldenDecorativeRugAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xADB, 1076586), 1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xADC, 1076586), -1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xADD, 1076586), -1, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xADE, 1076586), 1, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xADF, 1076586), -1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE0, 1076586), 0, -1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE1, 1076586), 1, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0xAE2, 1076586), 0, 1, 0);
            this.AddComponent(new LocalizedAddonComponent(0xADA, 1076586), 0, 0, 0);
        }

        public GoldenDecorativeRugAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new GoldenDecorativeRugDeed();
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

    public class GoldenDecorativeRugDeed : BaseAddonDeed
    {
        [Constructable]
        public GoldenDecorativeRugDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public GoldenDecorativeRugDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new GoldenDecorativeRugAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076586;
            }
        }// Golden decorative rug
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