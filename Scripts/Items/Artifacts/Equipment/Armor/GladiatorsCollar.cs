using System;

namespace Server.Items
{
    public class GladiatorsCollar : PlateGorget
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GladiatorsCollar()
        {
            Hue = 0x26d;
            Attributes.BonusHits = 10;
            Attributes.AttackChance = 10;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 8;
            ArmorAttributes.MageArmor = 1;
            SkillBonuses.SetValues(0, SkillName.Bushido, 10.0);
        }

        public GladiatorsCollar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094917;
            }
        }// Gladiator's Collar [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 20;
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
                return 20;
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