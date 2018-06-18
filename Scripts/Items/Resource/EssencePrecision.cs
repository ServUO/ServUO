using System;

namespace Server.Items
{
    public class EssencePrecision : Item, ICommodity
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
            Stackable = true;
            Amount = amount;
			Hue = 1158;
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
		int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
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
