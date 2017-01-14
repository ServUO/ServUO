using System;

namespace Server.Items
{
    public class SilverSnakeSkin : Item
    {
        [Constructable]
        public SilverSnakeSkin()
            : this(1)
        {
        }

        [Constructable]
        public SilverSnakeSkin(int amount)
            : base(0x5744)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public SilverSnakeSkin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113357;
            }
        }// silver snake skin
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