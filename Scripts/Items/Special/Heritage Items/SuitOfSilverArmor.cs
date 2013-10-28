using System;

namespace Server.Items
{
    [Flipable(0x3D86, 0x3D87)]
    public class SuitOfSilverArmorComponent : AddonComponent
    {
        public SuitOfSilverArmorComponent()
            : base(0x3D86)
        {
        }

        public SuitOfSilverArmorComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076266;
            }
        }// Suit of Silver Armor
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

    public class SuitOfSilverArmorAddon : BaseAddon
    {
        [Constructable]
        public SuitOfSilverArmorAddon()
            : base()
        {
            this.AddComponent(new SuitOfSilverArmorComponent(), 0, 0, 0);
        }

        public SuitOfSilverArmorAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SuitOfSilverArmorDeed();
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

    public class SuitOfSilverArmorDeed : BaseAddonDeed
    {
        [Constructable]
        public SuitOfSilverArmorDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public SuitOfSilverArmorDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SuitOfSilverArmorAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076266;
            }
        }// Suit of Silver Armor
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