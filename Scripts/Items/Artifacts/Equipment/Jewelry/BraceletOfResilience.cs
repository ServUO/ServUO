using System;

namespace Server.Items
{
    public class BraceletOfResilience : GoldBracelet
    {
        [Constructable]
        public BraceletOfResilience()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.DefendChance = 5;
            this.Resistances.Fire = 5;
            this.Resistances.Cold = 5;
            this.Resistances.Poison = 5;
            this.Resistances.Energy = 5;
        }

        public BraceletOfResilience(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077627;
            }
        }// Bracelet of Resilience
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