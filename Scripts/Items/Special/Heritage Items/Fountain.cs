using System;

namespace Server.Items
{
    public class FountainAddon : StoneFountainAddon
    {
        [Constructable]
        public FountainAddon()
            : base()
        {
        }

        public FountainAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FountainDeed();
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

    public class FountainDeed : BaseAddonDeed
    {
        [Constructable]
        public FountainDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public FountainDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FountainAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076283;
            }
        }// Fountain
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