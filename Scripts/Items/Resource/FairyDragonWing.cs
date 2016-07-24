using System;

namespace Server.Items
{
    public class FairyDragonWing : Item
    {
        public override int LabelNumber { get { return 1112899; } } // Fairy Dragon Wing

        [Constructable]
        public FairyDragonWing()
            : this(1)
        {
        }

        [Constructable]
        public FairyDragonWing(int amount)
            : base(0x1084)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public FairyDragonWing(Serial serial)
            : base(serial)
        {
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