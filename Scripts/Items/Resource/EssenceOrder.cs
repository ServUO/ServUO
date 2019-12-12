using System;

namespace Server.Items
{
    public class EssenceOrder : Item, ICommodity
    {
        [Constructable]
        public EssenceOrder()
            : this(1)
        {
        }

        [Constructable]
        public EssenceOrder(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
			Hue = 1153;
        }

        public EssenceOrder(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113342;
            }
        }// essence of order
		TextDefinition ICommodity.Description
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
