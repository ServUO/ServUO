using System;

namespace Server.Items
{
    public class SalvagerSuitPlans : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154229; } } // Plans to A Salvager Suit

        [Constructable]
        public SalvagerSuitPlans() : this(1)
        {
        }

        [Constructable]
        public SalvagerSuitPlans(int amount) : base(0x2258)
        {
            this.Hue = 1264;
            this.Stackable = false;
            this.Weight = 1.0;
            this.Amount = amount;
            this.LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 18000; } }
        public override bool UseSeconds { get { return false; } }

        public SalvagerSuitPlans(Serial serial) : base(serial)
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
