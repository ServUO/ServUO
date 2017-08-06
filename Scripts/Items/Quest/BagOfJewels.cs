using System;
using Server;

namespace Server.Items
{
    public class BagOfJewels : Item
    {
        public override int LabelNumber { get { return 1075307; } } // Bag of Jewels
        public override bool HiddenQuestItemHue { get { return true; } } 

        [Constructable]
        public BagOfJewels()
            : base(0xE76)
        {
            LootType = LootType.Blessed;
            Weight = 2.0;
            Hue = 155;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1075453); // Contents: 30 jewels
        }

        public BagOfJewels(Serial serial)
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