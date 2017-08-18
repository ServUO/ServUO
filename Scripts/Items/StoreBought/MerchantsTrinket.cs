using Server;
using System;

namespace Server.Items
{
    public class MerchantsTrinket : GoldEarrings
    {
        private bool _Greater;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Bonus { get { return Greater ? 10 : 5; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Greater { get { return _Greater; } set { _Greater = value; InvalidateProperties(); } }

        public override int LabelNumber { get { return _Greater ? 1156828 : 1156827; } } // Merchant's Trinket - 5% / 10%

        [Constructable]
        public MerchantsTrinket()
            : this(false)
        {
        }

        [Constructable]
        public MerchantsTrinket(bool greater)
        {
            Greater = greater;
            LootType = LootType.Blessed;
        }

        public MerchantsTrinket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(_Greater);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            _Greater = reader.ReadBool();
        }
    }
}