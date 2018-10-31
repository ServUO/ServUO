using System;

namespace Server.Items
{
    public class BurningAmber : GoldRing
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1114790; } } // Burning Amber
		
        [Constructable]
        public BurningAmber()
        {
            Hue = 1174;		
            Attributes.CastRecovery = 3;
            Attributes.RegenMana = 2;
            Attributes.BonusDex = 5;
            Resistances.Fire = 20;
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
        }
    }
}