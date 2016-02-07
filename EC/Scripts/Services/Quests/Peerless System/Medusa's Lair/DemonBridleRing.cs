using System;

namespace Server.Items
{
    public class DemonBridleRing : GoldRing
    {
        [Constructable]
        public DemonBridleRing()
        {
            this.Name = ("Demon Bridle Ring");
		
            this.Hue = 39;	
		
            this.Attributes.CastRecovery = 2;
            this.Attributes.CastSpeed = 1;	
            this.Attributes.RegenHits = 1;
            this.Attributes.RegenMana = 1;
            this.Attributes.DefendChance = 10;
            this.Attributes.LowerManaCost = 4;
            this.Resistances.Fire = 5;
        }

        public DemonBridleRing(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
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

            if (this.Hue == 0x4F4)
                this.Hue = 0x4F7;
        }
    }
}