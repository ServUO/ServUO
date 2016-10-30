using System;

namespace Server.Items
{
    public class SmallPieceofBlackrock : Item
    {
        [Constructable]
        public SmallPieceofBlackrock(): this(1)
        {
        }

        [Constructable]
        public SmallPieceofBlackrock(int amount) : base(0x0F28)
        {
            this.Hue = 1175;
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public SmallPieceofBlackrock(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber { get { return 1150016; } }

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