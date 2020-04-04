using System;

namespace Server.Items
{
    public class DupresShield : BaseShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DupresShield()
            : base(0x2B01)
        {
            LootType = LootType.Blessed;
            Weight = 6.0;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 1;
            SkillBonuses.SetValues(0, SkillName.Parry, 5);
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
        }// Dupre�s Shield
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
        public override int StrReq
        {
            get
            {
                return 50;
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