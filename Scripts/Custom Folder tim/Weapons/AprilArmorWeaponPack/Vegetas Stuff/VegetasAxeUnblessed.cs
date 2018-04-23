//////////////////////
//Created By Kyleman//               
//////////////////////
using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0xF45, 0xF46 )]
	public class VegetasAxeUnblessed : BaseAxe
	{
	 	public override int ArtifactRarity{ get{ return 15; } }
	 	public override int InitMinHits{ get{ return 250; } }
	 	public override int InitMaxHits{ get{ return 255; } }
		
		public override int DefMaxRange{ get{ return 2; } }
		
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }

		public override int AosStrengthReq{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 20; } }
		public override int AosSpeed{ get{ return 37; } }

		
	 	[Constructable]
	 	public VegetasAxeUnblessed() : base( 0xF45 )
	 	{
	 	 	Name = "vegetas axe";
	 	 	Hue = 3;
	 	 	Attributes.SpellChanneling = 1;
	 	 	Attributes.BonusStr = 15;
	 	 	Attributes.BonusHits = 15;
	 	 	Attributes.RegenHits = 10;
	 	 	WeaponAttributes.HitLeechHits = 50;
	 	 	Attributes.AttackChance = 25;
	 	 	Attributes.WeaponDamage = 65;
	 	 	Attributes.WeaponSpeed = 35;
	 	 	WeaponAttributes.HitColdArea = 50;
	 	 	WeaponAttributes.ResistColdBonus = 15;
	 	 	WeaponAttributes.ResistEnergyBonus = 5;
	 	 	WeaponAttributes.SelfRepair = 20;
	 	 	WeaponAttributes.HitLightning = 75;
	 	}

	 	public VegetasAxeUnblessed(Serial serial) : base( serial )
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