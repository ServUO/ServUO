using System;

namespace Server.Items
{
    public class EssenceControl : Item
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
            this.Stackable = true;
            this.Amount = amount;
			this.Hue = 1165;
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