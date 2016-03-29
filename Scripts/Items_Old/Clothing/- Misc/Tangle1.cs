using System;

namespace Server.Items
{
    public class Tangle1 : HalfApron
    {
        [Constructable]
        public Tangle1()
            : base()
        {
            this.Name = ("Tangle");
		
            this.Hue = 506;
			
            this.Attributes.BonusInt = 10;
            this.Attributes.DefendChance = 5;
            this.Attributes.RegenMana = 2;
        }

        public Tangle1(Serial serial)
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