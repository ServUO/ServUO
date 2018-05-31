using System;

namespace Server.Items
{
    public class CastOffZombieSkin : GargishLeatherArms
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CastOffZombieSkin() 
        {	
            Hue = 1893;		
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);	
            SkillBonuses.SetValues(1, SkillName.SpiritSpeak, 5.0);	
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 8;
            Attributes.IncreasedKarmaLoss = 5;
        }

        public CastOffZombieSkin(Serial serial)
            : base(serial)
        {
        }
        
        public override int LabelNumber { get{return 1113538;} }// Cast-off Zombie Skin

        public override int BasePhysicalResistance
        {
            get
            {
                return 13;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return -2;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 17;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 6;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
