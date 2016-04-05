using System;

namespace Server.Items
{
    public class StoneSlithClaw : Cyclone
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StoneSlithClaw()
        {
            this.Name = ("Stone Slith Claw");
		
            this.Hue = 1150;
			this.Weight = 6.0;
            this.WeaponAttributes.HitHarm = 40;
            this.Slayer = SlayerName.DaemonDismissal;
            this.WeaponAttributes.HitLowerDefend = 40;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 45;
			this.StrRequirement = 40;
        }

        public StoneSlithClaw(Serial serial)
            : base(serial)
        {
        }

        public override int MinThrowRange
        {
            get
            {
                return 4;
            }
        }

		public override int MaxThrowRange
		{
			get
			{
				return 9;
			}
		}

		public override float MlSpeed
		{
			get
			{
				return 3.0f;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}