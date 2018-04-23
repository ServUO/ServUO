//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/25/2017 1:01:07 AM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class RoyalLegplates : PlateLegs
	{
		public override int ArtifactRarity{ get{ return 42; } }
		public override int InitMinHits{ get{ return 200; } }
		public override int InitMaxHits{ get{ return 200; } }

		[Constructable]
		public RoyalLegplates()
		{
			Name = "Royal Legplates";
			Hue = 2;
			LootType = LootType.Blessed;
			Weight = 6;
			SkillBonuses.SetValues( 0, SkillName.Swords, 10 );
			SkillBonuses.SetValues( 1, SkillName.Tactics, 10 );
			Attributes.AttackChance = 10;
			Attributes.DefendChance = 10;
			Attributes.BonusInt = 8;
			Attributes.BonusDex = 5;
			Attributes.BonusStr = 5;
			Attributes.WeaponDamage = 20;
			Attributes.ReflectPhysical = 10;
			Attributes.WeaponSpeed = 10;
			PhysicalBonus = 10;
			ColdBonus = 10;
			FireBonus = 10;
			PoisonBonus = 10;
			EnergyBonus = 10;
		}

		public RoyalLegplates( Serial serial ) : base( serial )
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
