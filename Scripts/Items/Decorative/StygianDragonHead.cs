using System;

namespace Server.Items
{
    public class StygianDragonHead : Item
    {
        public override int LabelNumber { get { return 1031700; } } // Stygian Dragon Head

        [Constructable]
        public StygianDragonHead()
            : this(1)
        {
        }

        [Constructable]
        public StygianDragonHead(int amount)
            : base(0x2DB4)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public StygianDragonHead(Serial serial)
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