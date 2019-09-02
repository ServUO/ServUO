using System;

namespace Server.Items
{
    public class VileTentacles : Item, ICommodity
    {
        public override int LabelNumber { get { return 1113333; } } // vile tentacles
        public override double DefaultWeight { get { return 0.1; } }

        [Constructable]
        public VileTentacles()
            : this(1)
        {
        }

        [Constructable]
        public VileTentacles(int amount)
            : base(0x5727)
        {
            Stackable = true;
            Amount = amount;
        }

        public VileTentacles(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        
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
