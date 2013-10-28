using System;

namespace Server.Items
{
    public class LordBritishThroneAddon : BaseAddon
    {
        [Constructable]
        public LordBritishThroneAddon()
        {
            this.AddComponent(new AddonComponent(0x1526), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1527), 0, -1, 0);
        }

        public LordBritishThroneAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LordBritishThroneDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LordBritishThroneDeed : BaseAddonDeed
    {
        [Constructable]
        public LordBritishThroneDeed()
        {
            this.LootType = LootType.Blessed;
        }

        public LordBritishThroneDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LordBritishThroneAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073243;
            }
        }// Replica of Lord British's Throne - Museum of Vesper
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