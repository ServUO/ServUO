using System;

namespace Server.Items
{
    public class EssencePersistence : Item, ICommodity
    {
        [Constructable]
        public EssencePersistence()
            : this(1)
        {
        }

        [Constructable]
        public EssencePersistence(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
            Hue = 37;
        }

        public EssencePersistence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113343;
            }
        }// essence of persistence
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
