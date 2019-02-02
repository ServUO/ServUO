using System;

namespace Server.Items
{
    public class CupidStatue : Item
    {
        public override int LabelNumber { get { return 1099220; } } // cupid statue

        [Constructable]
        public CupidStatue()
            : base(Utility.RandomList(20348, 20349))
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public CupidStatue(Serial serial)
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
