using System;

namespace Server.Items
{
    public class EssenceFeeling : Item, ICommodity
    {
        [Constructable]
        public EssenceFeeling()
            : this(1)
        {
        }

        [Constructable]
        public EssenceFeeling(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
			Hue = 455;
        }

        public EssenceFeeling(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113339;
            }
        }// essence of feeling
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
