using System;

namespace Server.Items
{
    public class DecorativeStableSet : Backpack
    {
        public override int LabelNumber { get { return 1159272; } } // Decorative Stable Set

        [Constructable]
        public DecorativeStableSet()
        {
            DropItem(new CowStatue());
            DropItem(new HorseStatue());
            DropItem(new ChickenStatue());
            DropItem(new MetalTubDeed());
            DropItem(new Feedbag());
			DropItem(new CowPie());

            Bag bag = new Bag();
            // Needs fencing added
            DropItem(bag);
        }

        public DecorativeStableSet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
