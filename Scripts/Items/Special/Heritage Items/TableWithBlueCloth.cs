using System;

namespace Server.Items
{
    public class TableWithBlueClothAddon : BaseAddon
    {
        [Constructable]
        public TableWithBlueClothAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0x118C, 1076276), 0, 0, 0);
        }

        public TableWithBlueClothAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new TableWithBlueClothDeed();
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

    public class TableWithBlueClothDeed : BaseAddonDeed
    {
        [Constructable]
        public TableWithBlueClothDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public TableWithBlueClothDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new TableWithBlueClothAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076276;
            }
        }// Table With A Blue Tablecloth
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