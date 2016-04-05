using System;

namespace Server.Items
{
    public class DupresShield : BaseShield, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DupresShield()
            : base(0x2B01)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 6.0;

            this.Attributes.BonusHits = 5;
            this.Attributes.RegenHits = 1;

            this.SkillBonuses.SetValues(0, SkillName.Parry, 5);
        }

        public DupresShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075196;
            }
        }// Dupre’s Shield
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
                return 0;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 1;
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
        public override int AosStrReq
        {
            get
            {
                return 50;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 15;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}