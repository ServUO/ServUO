using System;

namespace Server.Items
{
    public class BurningAmber : GoldRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BurningAmber()
        {
            this.Name = ("Burning Amber");
		
            this.Hue = 1174;	
		
            this.Attributes.CastRecovery = 3;
            this.Attributes.RegenMana = 2;
            this.Attributes.BonusDex = 5;
            this.Resistances.Fire = 20;
        }

        public BurningAmber(Serial serial)
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

            if (this.Hue == 0x4F4)
                this.Hue = 0x4F7;
        }
    }
}