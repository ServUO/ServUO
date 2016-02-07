using System;

namespace Server.Items
{
	public class RunedDriftwoodBow : Bow
	{
		[Constructable]
		public RunedDriftwoodBow()
		{
			this.Attributes.WeaponDamage = 50;
			this.Attributes.WeaponSpeed = 30;
			this.WeaponAttributes.HitLowerDefend = 40;
			this.WeaponAttributes.HitLightning = 40;
			//this.LowerAmmoCost = 15; Todo: add lower ammo cost prop to baseranged and have OnFired check for bow
			this.Hue = 2034; // Hue not exact
			this.Name = ("Runed Driftwood Bow");
	}

		public RunedDriftwoodBow(Serial serial)
			: base(serial)
		{
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