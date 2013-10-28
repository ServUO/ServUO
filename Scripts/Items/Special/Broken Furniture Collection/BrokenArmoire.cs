using System;

namespace Server.Items
{
    [Flipable(0xC12, 0xC13)]
    public class BrokenArmoireComponent : AddonComponent
    {
        public BrokenArmoireComponent()
            : base(0xC12)
        {
        }

        public BrokenArmoireComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076262;
            }
        }// Broken Armoire
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

    public class BrokenArmoireAddon : BaseAddon
    {
        [Constructable]
        public BrokenArmoireAddon()
            : base()
        {
            this.AddComponent(new BrokenArmoireComponent(), 0, 0, 0);
        }

        public BrokenArmoireAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BrokenArmoireDeed();
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

    public class BrokenArmoireDeed : BaseAddonDeed
    {
        [Constructable]
        public BrokenArmoireDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public BrokenArmoireDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BrokenArmoireAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076262;
            }
        }// Broken Armoire
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