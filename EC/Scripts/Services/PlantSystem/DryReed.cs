using System;

namespace Server.Items
{
    public class DryReed : Item//, ICraftable
    {
        [Constructable]
        public DryReed()
            : this(1)
        {
        }

        [Constructable]
        public DryReed(int amount)
            : base(0xF42)
        {
            this.Weight = 1.0;

            this.Hue = 0;  
            this.Name = "Dry Reed";			
            this.Stackable = true;
            this.Amount = amount; 
        }

        public DryReed(Serial serial)
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