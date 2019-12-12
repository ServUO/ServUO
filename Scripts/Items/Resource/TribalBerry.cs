using System;

namespace Server.Items
{
    public class TribalBerry : Item, ICommodity
    {
		TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override int LabelNumber { get { return 1040001; } }// tribal berry
		
        [Constructable]
        public TribalBerry()
            : this(1)
        {
        }

        [Constructable]
        public TribalBerry(int amount)
            : base(0x9D0)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
            Hue = 6;
        }

        public TribalBerry(Serial serial)
            : base(serial)
        {
        }       
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
