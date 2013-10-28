using System;

namespace Server.Items
{
    public class EssencePrecision : Item
    {
        [Constructable]
        public EssencePrecision()
            : this(1)
        {
        }

        [Constructable]
        public EssencePrecision(int amount)
            : base(0x571C)
        {
            this.Stackable = true;
            this.Amount = amount;
			this.Hue = 1158;
        }

        public EssencePrecision(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113327;
            }
        }// essence of precision
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