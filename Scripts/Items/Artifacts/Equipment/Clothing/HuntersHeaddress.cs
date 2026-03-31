using System;

namespace Server.Items
{
    public class HuntersHeaddress : DeerMask
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HuntersHeaddress()
        {
            Hue = 0x594;
            SkillBonuses.SetValues(0, SkillName.Archery, 20);
            Attributes.BonusDex = 8;
            Attributes.BonusStam = 10;
            Attributes.NightSight = 1;
            Attributes.AttackChance = 15;
            Attributes.RegenStam = 4;
        }

        public HuntersHeaddress(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061595;
            }
        }// Hunter's Headdress
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch ( version )
            {
                case 0:
                    {
                        this.Resistances.Cold = 0;
                        break;
                    }
            }
        }
    }
}