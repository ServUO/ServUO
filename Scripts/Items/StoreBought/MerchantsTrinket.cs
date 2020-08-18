namespace Server.Items
{
    public class MerchantsTrinket : GoldEarrings
    {
        private bool _Greater;
        private int _UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Bonus => Greater ? 10 : 5;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Greater { get { return _Greater; } set { _Greater = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return _UsesRemaining; } set { _UsesRemaining = value; InvalidateProperties(); } }

        public override int LabelNumber => 1071399; // Merchant's Trinket

        [Constructable]
        public MerchantsTrinket()
            : this(false)
        {
            LootType = LootType.Blessed;
        }

        [Constructable]
        public MerchantsTrinket(bool greater)
        {
            Greater = greater;
            LootType = LootType.Blessed;

            UsesRemaining = 90;
        }

        public MerchantsTrinket(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1071398, Bonus.ToString()); // Discount Rate of Vendor Charge: ~1_val~%
            list.Add(1159250); // Non-commission vendors only.
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            if (_UsesRemaining > 0)
            {
                list.Add(1060584, _UsesRemaining.ToString()); // uses remaining: ~1_val~
            }

            base.AddWeightProperty(list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(_UsesRemaining);
            writer.Write(_Greater);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _UsesRemaining = reader.ReadInt();
                    _Greater = reader.ReadBool();
                    break;
                case 0:
                    _Greater = reader.ReadBool();
                    _UsesRemaining = 90;
                    break;
            }
        }
    }
}
