using System;

namespace Server.Items
{
    public class CaptainJohnsHat : TricorneHat
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CaptainJohnsHat()
        {
            Hue = 0x455;
            Attributes.BonusDex = 8;
            Attributes.NightSight = 1;
            Attributes.AttackChance = 15;

            SkillName[] combatSkills = new SkillName[]
            {
                SkillName.Swords, SkillName.Macing, SkillName.Fencing,
                SkillName.Archery, SkillName.Wrestling
            };
            SkillBonuses.SetValues(0, combatSkills[Utility.Random(combatSkills.Length)], 20.0);
            WeaponAttributes.HitLowerDefend = 30;
        }

        public CaptainJohnsHat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094911;
            }
        }// Captain John's Hat [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 12;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
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