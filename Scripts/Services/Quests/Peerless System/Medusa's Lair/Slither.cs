using System;

namespace Server.Items
{
    public class Slither : BaseTalisman, ITokunoDyable
    {
        [Constructable]
        public Slither()
            : base(0x2F5B)
        {
            this.Hue = 589;
			
            this.Name = ("Slither");
				
            this.Blessed = RandomTalisman.GetRandomBlessed();				
			
            this.Attributes.BonusHits = 10;
            this.Attributes.RegenHits = 2;
            this.Attributes.DefendChance = 10;
        }

        public Slither(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }
    }
}