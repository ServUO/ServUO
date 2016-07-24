using System;

namespace Server.Items
{
    public class EnchantEssence : Item
    {
        public override int LabelNumber { get { return 1031698; } } // Enchanted Essence

        [Constructable]
        public EnchantEssence()
            : this(1)
        {
        }

        [Constructable]
        public EnchantEssence(int amount)
            : base(0x2DB2)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public EnchantEssence(Serial serial)
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