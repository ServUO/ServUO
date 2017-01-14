using System;

namespace Server.Items
{
    public class HuntersHeaddress : DeerMask
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HuntersHeaddress()
        {
            this.Hue = 0x594;

            this.SkillBonuses.SetValues(0, SkillName.Archery, 20);

            this.Attributes.BonusDex = 8;
            this.Attributes.NightSight = 1;
            this.Attributes.AttackChance = 15;
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
        public override int BaseColdResistance
        {
            get
            {
                return 23;
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