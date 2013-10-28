using System;

namespace Server.Items
{
    public class CrushedGlass : Item
    {
        [Constructable]
        public CrushedGlass()
            : this(1)
        {
        }

        [Constructable]
        public CrushedGlass(int amount)
            : base(0x573B)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public CrushedGlass(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113351;
            }
        }// crushed glass
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