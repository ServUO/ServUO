using System;

namespace Server.Items
{
    public class CrownOfArcaneTemperament : Circlet
	{
		public override bool IsArtifact { get { return true; } }

        public override Race RequiredRace { get { return null; } }
        public override int LabelNumber { get { return 1113762; } } // Crown of Arcane Temperament

        [Constructable]
        public CrownOfArcaneTemperament()
        {
			Attributes.BonusMana = 8;
			Attributes.RegenMana = 3;
			Attributes.SpellDamage = 8;
			Attributes.LowerManaCost = 6;
			Hue = 2012;
			AbsorptionAttributes.CastingFocus = 2;
        }

        public CrownOfArcaneTemperament(Serial serial)
            : base(serial)
        {
        }
		public override int BasePhysicalResistance {  get { return 10; } }
        public override int BaseFireResistance { get { return 14; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 12; } }
        public override int BaseEnergyResistance {  get { return 7; } }
		
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
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