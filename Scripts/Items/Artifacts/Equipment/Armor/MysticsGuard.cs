using System;

namespace Server.Items
{
    public class MysticsGuard : GargishWoodenShield
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber {get { return 1113536; } }
		public override int ArtifactRarity{ get { return 5; } }

        [Constructable]
        public MysticsGuard()
            : base()
        {
            ArmorAttributes.SoulCharge = 30;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 2;
	        Hue = 0x671;
	        Weight = 10.0;
        }

        public MysticsGuard(Serial serial)
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
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
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