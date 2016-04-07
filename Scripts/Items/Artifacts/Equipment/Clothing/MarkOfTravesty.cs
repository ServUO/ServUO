using System;

namespace Server.Items
{
    public class MarkOfTravesty : SavageMask, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MarkOfTravesty()
            : base()
        {
            this.Hue = 0x495;
			
            this.Attributes.BonusMana = 8;
            //Attributes.RegenHits = 3;
			
            this.ClothingAttributes.SelfRepair = 3;
			
            switch( Utility.Random(15) )
            {
                case 0: 
                    this.SkillBonuses.SetValues(0, SkillName.EvalInt, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Magery, 10);
                    break;
                case 1: 
                    this.SkillBonuses.SetValues(0, SkillName.AnimalLore, 10);
                    this.SkillBonuses.SetValues(1, SkillName.AnimalTaming, 10);
                    break;
                case 2: 
                    this.SkillBonuses.SetValues(0, SkillName.Swords, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 3: 
                    this.SkillBonuses.SetValues(0, SkillName.Discordance, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Musicianship, 10);
                    break;
                case 4: 
                    this.SkillBonuses.SetValues(0, SkillName.Fencing, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 5: 
                    this.SkillBonuses.SetValues(0, SkillName.Chivalry, 10);
                    this.SkillBonuses.SetValues(1, SkillName.MagicResist, 10);
                    break;
                case 6: 
                    this.SkillBonuses.SetValues(0, SkillName.Anatomy, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Healing, 10);
                    break;
                case 7: 
                    this.SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Stealth, 10);
                    break;
                case 8: 
                    this.SkillBonuses.SetValues(0, SkillName.Bushido, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Parry, 10);
                    break;
                case 9: 
                    this.SkillBonuses.SetValues(0, SkillName.Archery, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 10: 
                    this.SkillBonuses.SetValues(0, SkillName.Macing, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Tactics, 10);
                    break;
                case 11: 
                    this.SkillBonuses.SetValues(0, SkillName.Necromancy, 10);
                    this.SkillBonuses.SetValues(1, SkillName.SpiritSpeak, 10);
                    break;
                case 12: 
                    this.SkillBonuses.SetValues(0, SkillName.Stealth, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Stealing, 10);
                    break;
                case 13: 
                    this.SkillBonuses.SetValues(0, SkillName.Peacemaking, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Musicianship, 10);
                    break;
                case 14:
                    this.SkillBonuses.SetValues(0, SkillName.Provocation, 10);
                    this.SkillBonuses.SetValues(1, SkillName.Musicianship, 10);
                    break;
            }
        }

        public MarkOfTravesty(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074493;
            }
        }// Mark of Travesty
        public override int BasePhysicalResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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