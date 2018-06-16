using System;

namespace Server.Items
{
    public class EssenceControl : Item, ICommodity
    {
        [Constructable]
        public EssenceControl()
            : this(1)
        {
        }

        [Constructable]
        public EssenceControl(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
			Hue = 1165;
        }

        public EssenceControl(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113340;
            }
        }// essence of control
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
