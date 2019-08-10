using System;

namespace Server.Items
{
    public class EcruCitrineRing : GoldRing
    {
		public override int LabelNumber { get { return 1073457; } }// ecru citrine ring
		
        [Constructable]
        public EcruCitrineRing()
            : base()
        {
            Weight = 1.0;

            BaseRunicTool.ApplyAttributesTo(this, true, 0, Utility.RandomMinMax(2, 3), 0, 100);
			
            if (Utility.RandomBool())
                Attributes.EnhancePotions = 50;	
            else
                Attributes.BonusStr += 5;
        }

        public EcruCitrineRing(Serial serial)
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