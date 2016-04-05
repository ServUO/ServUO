using System;

namespace Server.Items
{
    public class RaptorClaw : Boomerang
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RaptorClaw()
        {
            this.Name = ("Raptor Claw");

			this.Weight = 4.0;
            this.Hue = 53;
            this.Slayer = SlayerName.Silver;
			this.Attributes = new AosAttributes(this);
            this.Attributes.AttackChance = 12;			
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 35;
			this.WeaponAttributes = new AosWeaponAttributes(this);
            this.WeaponAttributes.HitLeechStam = 40;
			this.StrRequirement = 25;
        }

		public override int AosMinDamage
		{
			get
			{
				return 8;
			}
		}

		public override int AosMaxDamage
		{
			get
			{
				return 12;
			}
		}

		public override int MaxThrowRange
		{
			get
			{
				return 7;
			}
		}

		public override float MlSpeed
		{
			get
			{
				return 2;
			}
		}

        public RaptorClaw(Serial serial)
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