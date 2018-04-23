//Dragon Legs Of Evolution By: Sean (http://www.wftpradio.net/)

using System;
using Server;

namespace Server.Items
{
	public class DragonLegsOfEvolution: DragonLegs
	{

		private int mEvolutionPoints = 0;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int EvolutionPoints { get { return mEvolutionPoints; } set { mEvolutionPoints = value; } }

		public override int ArtifactRarity{ get{ return 666; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public DragonLegsOfEvolution()
		{	Name = "Dragon Legs Of Evolution";
			Hue = 2255;

            ArmorAttributes.SelfRepair = 10;
        }

        public DragonLegsOfEvolution(Serial serial)
            : base(serial)
        {
        }
           public override int OnHit( BaseWeapon weapon, int damageTaken )
        {
            if (Utility.Random(2) == 1)
            {
                Console.WriteLine("should be applying gain");
                ApplyGain();
            }

            return base.OnHit(weapon, damageTaken);
        }

        public void ApplyGain()
        {
            int expr;
            if (mEvolutionPoints < 1001) // edit this to change how high you wish the Attributes to go 10000 means max attributes will be 100
            {
                mEvolutionPoints++;
                this.Name = "Dragon Legs Of Evolution (" + mEvolutionPoints.ToString() + ")";

                if ((mEvolutionPoints / 1) > 0)
                {
                    expr = mEvolutionPoints / 1;

                    this.Attributes.AttackChance = expr;
                    this.Attributes.WeaponSpeed  = expr;
                    this.Attributes.BonusHits = expr;
                }

                if ((mEvolutionPoints / 2) > 0)
                {
                    expr = mEvolutionPoints / 1;

                    this.Attributes.WeaponDamage = expr;
                }

                if ((25 + (mEvolutionPoints / 2)) > 0) this.Attributes.WeaponSpeed = (25 + (mEvolutionPoints / 2));

                if ((mEvolutionPoints / 20) > 0)
                {
                    expr = mEvolutionPoints / 20;

                    this.Attributes.CastRecovery = expr;
                    this.Attributes.CastSpeed = expr;
                    this.Attributes.BonusStr = expr;
                    this.Attributes.BonusDex = expr;
                    this.Attributes.BonusInt = expr;
                }
                InvalidateProperties();


            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(mEvolutionPoints);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            mEvolutionPoints = reader.ReadInt();
        }
    }
}