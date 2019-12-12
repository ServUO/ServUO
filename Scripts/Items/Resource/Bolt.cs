using System;

namespace Server.Items
{
    public class Bolt : Item, ICommodity
    {		
        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }
		
		public override double DefaultWeight { get { return 0.1; } }
		
        [Constructable]
        public Bolt()
            : this(1)
        {
        }

        [Constructable]
        public Bolt(int amount)
            : base(0x1BFB)
        {
            Stackable = true;
            Amount = amount;
        }

        public Bolt(Serial serial)
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