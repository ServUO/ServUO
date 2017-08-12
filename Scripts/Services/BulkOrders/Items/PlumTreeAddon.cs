using System;
using Server.Mobiles;

namespace Server.Items
{
    public class PlumTreeAddon : BaseFruitTreeAddon
    {
        public override BaseAddonDeed Deed { get { return new PlumTreeAddonDeed(); } }

        [Constructable]
        public PlumTreeAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x9E38, 1029965), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x9E39, 1029965), 0, 0, 0);
        }

        public override Item Fruit
        {
            get
            {
                return new Plum();
            }
        }

        public PlumTreeAddon(Serial serial)
            : base(serial)
        {
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

    public class PlumTreeAddonDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1157312; } } // Plum Tree
        public override BaseAddon Addon { get { return new PlumTreeAddon(); } }

        [Constructable]
        public PlumTreeAddonDeed()
        {
        }

        public PlumTreeAddonDeed(Serial serial)
            : base(serial)
        {
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
}
