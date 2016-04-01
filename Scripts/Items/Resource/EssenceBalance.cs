using System;

namespace Server.Items
{
    public class EssenceBalance : Item
    {
        [Constructable]
        public EssenceBalance()
            : this(1)
        {
        }

        [Constructable]
        public EssenceBalance(int amount)
            : base(0x571C)
        {
            this.Stackable = true;
            this.Amount = amount;
			this.Hue = 1268;
        }

        public EssenceBalance(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113324;
            }
        }// essence of balance
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