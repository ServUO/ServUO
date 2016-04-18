using System;

namespace Server.Items
{
    public class EssenceDiligence : Item
    {
        [Constructable]
        public EssenceDiligence()
            : this(1)
        {
        }

        [Constructable]
        public EssenceDiligence(int amount)
            : base(0x571C)
        {
            this.Stackable = true;
            this.Amount = amount;
			this.Hue = 1166;
        }

        public EssenceDiligence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113338;
            }
        }// essence of diligence
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