using System;

namespace Server.Items
{
    public class PillarOfStrength : LargeStoneShield
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1113533; } }

        [Constructable]
        public PillarOfStrength()
            : base()
        {
            Attributes.BonusStr = 10;
            Attributes.BonusHits = 10;
            Attributes.WeaponDamage = 20;
        }

        public PillarOfStrength(Serial serial)
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
                return 1;
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
            writer.Write((int)0);//version
        }
    }
}