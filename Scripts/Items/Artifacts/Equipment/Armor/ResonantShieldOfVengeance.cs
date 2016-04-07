using System;
using Server;

namespace Server.Items
{
    public class ResonantShieldOfVengeance : GargishWoodenShield
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1150357; } } // Resonant Shield of Vengeance

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public ResonantShieldOfVengeance()
        {
            Hue = 2076;

            switch (Utility.Random(5))
            {
                case 0: AbsorptionAttributes.ResonanceKinetic = 10; break;
                case 1: AbsorptionAttributes.ResonanceFire = 10; break;
                case 2: AbsorptionAttributes.ResonanceCold = 10; break;
                case 3: AbsorptionAttributes.ResonancePoison = 10; break;
                case 4: AbsorptionAttributes.ResonanceEnergy = 10; break;
            }

            Attributes.SpellChanneling = 1;
            Attributes.ReflectPhysical = 20;
            Attributes.DefendChance = 8;

            switch (Utility.Random(5))
            {
                case 0: PhysicalBonus = 10; break;
                case 1: FireBonus = 10; break;
                case 2: ColdBonus = 10; break;
                case 3: PoisonBonus = 10; break;
                case 4: EnergyBonus = 10; break;
            }
        }

        public ResonantShieldOfVengeance(Serial serial) : base(serial)
        {
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

    public class ResonantShieldOfVengeanceHuman : BronzeShield
    {
        public override int LabelNumber { get { return 1150357; } } // Resonant Shield of Vengeance

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public ResonantShieldOfVengeanceHuman()
        {
            Hue = 2076;

            switch (Utility.Random(5))
            {
                case 0: AbsorptionAttributes.ResonanceKinetic = 10; break;
                case 1: AbsorptionAttributes.ResonanceFire = 10; break;
                case 2: AbsorptionAttributes.ResonanceCold = 10; break;
                case 3: AbsorptionAttributes.ResonancePoison = 10; break;
                case 4: AbsorptionAttributes.ResonanceEnergy = 10; break;
            }

            Attributes.SpellChanneling = 1;
            Attributes.ReflectPhysical = 20;
            Attributes.DefendChance = 8;

            switch (Utility.Random(5))
            {
                case 0: PhysicalBonus = 10; break;
                case 1: FireBonus = 10; break;
                case 2: ColdBonus = 10; break;
                case 3: PoisonBonus = 10; break;
                case 4: EnergyBonus = 10; break;
            }
        }

        public ResonantShieldOfVengeanceHuman(Serial serial)
            : base(serial)
        {
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