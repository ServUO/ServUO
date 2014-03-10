using System;

namespace Server.Items
{
    public class CrownOfArcaneTemperament : Circlet
    {
        [Constructable]
        public CrownOfArcaneTemperament()
        {
			this.Attributes.BonusMana = 8;
			this.Attributes.RegenMana = 3;
			this.Attributes.SpellDamage = 8;
			this.Attributes.LowerManaCost = 6;
			this.Hue = 149; //Hue not exact
			AbsorptionAttributes.CastingFocus = 2;
			this.Name = ("Crown of Arcane Temperament");
        }

        public CrownOfArcaneTemperament(Serial serial)
            : base(serial)
        {
        }

		public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

		public override Race RequiredRace
        {
            get
            {
                return Race.Human;
            }
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
                return 14;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 4;
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
                return 7;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}