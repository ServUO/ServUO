using System;

namespace Server.Items
{
    public class EssencePassion : Item
    {
        [Constructable]
        public EssencePassion()
            : this(1)
        {
        }

        [Constructable]
        public EssencePassion(int amount)
            : base(0x571C)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 1161;
        }

        public EssencePassion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113326;
            }
        }// essence of passion
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