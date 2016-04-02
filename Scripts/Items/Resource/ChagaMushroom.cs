using System;

namespace Server.Items
{
    public class ChagaMushroom : Item
    {
        [Constructable]
        public ChagaMushroom()
            : this(1)
        {
        }

        [Constructable]
        public ChagaMushroom(int amount)
            : base(0x5743)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public ChagaMushroom(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113356;
            }
        }// chaga mushroom
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