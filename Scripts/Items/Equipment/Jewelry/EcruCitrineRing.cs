using System;

namespace Server.Items
{
    public class EcruCitrineRing : GoldRing
    {
        [Constructable]
        public EcruCitrineRing()
            : base()
        {
            this.Weight = 1.0;
			
            BaseRunicTool.ApplyAttributesTo(this, Utility.RandomMinMax(2, 3), 0, 100);
			
            if (Utility.RandomBool())
                this.Attributes.EnhancePotions = 75;	
            else
                this.Attributes.BonusStr += 5;
        }

        public EcruCitrineRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073457;
            }
        }// ecru citrine ring
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