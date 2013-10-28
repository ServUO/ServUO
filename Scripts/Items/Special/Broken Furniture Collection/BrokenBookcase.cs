using System;

namespace Server.Items
{
    [Flipable(0xC14, 0xC15)]
    public class BrokenBookcaseComponent : AddonComponent
    {
        public BrokenBookcaseComponent()
            : base(0xC14)
        {
        }

        public BrokenBookcaseComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076258;
            }
        }// Broken Bookcase
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

    public class BrokenBookcaseAddon : BaseAddon
    {
        [Constructable]
        public BrokenBookcaseAddon()
            : base()
        {
            this.AddComponent(new BrokenBookcaseComponent(), 0, 0, 0);
        }

        public BrokenBookcaseAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BrokenBookcaseDeed();
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

    public class BrokenBookcaseDeed : BaseAddonDeed
    {
        [Constructable]
        public BrokenBookcaseDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public BrokenBookcaseDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BrokenBookcaseAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076258;
            }
        }// Broken Bookcase
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