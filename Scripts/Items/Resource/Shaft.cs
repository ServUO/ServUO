using System;

namespace Server.Items
{
    public class Shaft : Item, ICommodity
    {
        [Constructable]
        public Shaft()
            : this(1)
        {
        }

        [Constructable]
        public Shaft(int amount)
            : base(0x1BD4)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Shaft(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }
        TextDefinition ICommodity.Description
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
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