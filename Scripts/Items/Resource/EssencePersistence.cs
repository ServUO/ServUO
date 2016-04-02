using System;

namespace Server.Items
{
    public class EssencePersistence : Item
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
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 37;
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