using System;

namespace Server.Items
{
    public class AquaPendant : GoldNecklace
    {
        public override int LabelNumber { get { return 1154246; } } // Aqua Pendant

        [Constructable]
        public AquaPendant()
        {
            this.Hue = 1916;
            this.LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public AquaPendant(Serial serial)
            : base(serial)
        {
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
}
