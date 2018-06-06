using System;

namespace Server.Items
{
    public class GargishSignOfOrder : GargishOrderShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GargishSignOfOrder()
            : base()
        {		
            SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0);
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 10;
            Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
        }

        public GargishSignOfOrder(Serial serial)
            : base(serial)
        {
        }     
        
        public override int LabelNumber { get{return 1113534;} }// Sign of Order
        
        public override int BasePhysicalResistance
        {
            get
            {
                return 1;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 0;
            }
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
