using System;

namespace Server.Items
{
    public class NightEyes : Glasses, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public NightEyes()
            : base()
        {
            this.Name = ("Night Eyes");
		
            this.Hue = 26;
		
            this.Attributes.NightSight = 1;
            this.Attributes.DefendChance = 10;
            this.Attributes.CastRecovery = 3;			
        }

        public NightEyes(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
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