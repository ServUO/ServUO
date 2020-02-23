using System;

namespace Server.Items
{
    public class Lodestone : Item, ICommodity
    {
        [Constructable]
        public Lodestone()
            : this(1)
        {
        }

        [Constructable]
        public Lodestone(int amount)
            : base(0x5739)
        {
            Stackable = true;
            Amount = amount;
        }

        public Lodestone(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override int LabelNumber
        {
            get
            {
                return 1113348;
            }
        }// lodestone
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
