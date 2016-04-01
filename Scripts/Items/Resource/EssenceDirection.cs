using System;

namespace Server.Items
{
    public class EssenceDirection : Item
    {
        [Constructable]
        public EssenceDirection()
            : this(1)
        {
        }

        [Constructable]
        public EssenceDirection(int amount)
            : base(0x571C)
        {
            this.Stackable = true;
            this.Amount = amount;
			this.Hue = 1156;
        }

        public EssenceDirection(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113328;
            }
        }// essence of direction
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