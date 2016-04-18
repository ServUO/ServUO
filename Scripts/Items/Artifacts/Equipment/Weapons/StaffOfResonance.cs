using System;

namespace Server.Items
{
    public class StaffOfResonance : GlassStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StaffOfResonance()
        {
			switch (Utility.Random(5)) // Random resonance property
			{
			case 0 : this.AbsorptionAttributes.ResonanceFire = 20; break;
			case 1 : this.AbsorptionAttributes.ResonanceCold = 20; break;
			case 2 : this.AbsorptionAttributes.ResonancePoison = 20; break;
			case 3 : this.AbsorptionAttributes.ResonanceEnergy = 20; break;
			case 4 : this.AbsorptionAttributes.ResonanceKinetic = 20; break;			
			}
			this.Attributes.SpellChanneling = 1;
			this.WeaponAttributes.MageWeapon = 10;
			this.WeaponAttributes.HitHarm = 50;
			this.Attributes.DefendChance = 10;
			this.Attributes.WeaponSpeed = 20;
			this.Attributes.WeaponDamage = -40;
			this.Attributes.LowerManaCost = 5;
			this.AosElementDamages.Poison = 100;
			this.Hue = 1451; //Hue not exact
			this.Name = ("Staff of Resonance");
        }

        public StaffOfResonance(Serial serial)
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