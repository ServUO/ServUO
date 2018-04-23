//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/27/2017 10:58:44 AM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class ShieldOfSin : ChaosShield
	{
		public override int ArtifactRarity{ get{ return 77; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ShieldOfSin()
		{
			Name = "Shield Of Sin";
			Hue = 10;
			Weight = 5;
			Attributes.AttackChance = 15;
			Attributes.DefendChance = 15;
			Attributes.EnhancePotions = 20;
			Attributes.WeaponDamage = 30;
			Attributes.ReflectPhysical = 20;
			Attributes.LowerRegCost = 20;
			Attributes.WeaponSpeed = 30;
			PhysicalBonus = 8;
			ColdBonus = 7;
			FireBonus = 7;
			PoisonBonus = 7;
			EnergyBonus = 4;
		}

		public ShieldOfSin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}
}
