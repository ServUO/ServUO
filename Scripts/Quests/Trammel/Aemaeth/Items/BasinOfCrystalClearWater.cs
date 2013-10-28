using System;

namespace Server.Items
{
    public class BasinOfCrystalClearWater : Item
    {
        [Constructable]
        public BasinOfCrystalClearWater()
            : base(0x1008)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 5.0;
        }

        public BasinOfCrystalClearWater(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075303;
            }
        }// Basin of Crystal-Clear Water
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