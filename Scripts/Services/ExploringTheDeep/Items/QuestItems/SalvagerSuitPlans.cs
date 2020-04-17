using Server.Network;

namespace Server.Items
{
    public class SalvagerSuitPlans : BaseDecayingItem
    {
        public override int LabelNumber => 1154229;  // Plans to A Salvager Suit

        [Constructable]
        public SalvagerSuitPlans() : this(1)
        {
        }

        [Constructable]
        public SalvagerSuitPlans(int amount) : base(0x2258)
        {
            Hue = 92;
            Stackable = false;
            Weight = 1.0;
            Amount = amount;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154230); // *You examine the document carefully.  It appears to be the detailed schematic of some kind of suit.  It is beyond your understanding.  You decide to take it back to the Master Tinker*
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 18000;
        public override bool UseSeconds => false;

        public SalvagerSuitPlans(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
