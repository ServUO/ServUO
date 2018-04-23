//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/3/2012 5:57:24 PM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class NewbieWarHammer : WarHammer
	{
		public override int ArtifactRarity{ get{ return 10; } }
		public override int InitMinHits{ get{ return 100; } }
		public override int InitMaxHits{ get{ return 100; } }

		[Constructable]
		public NewbieWarHammer()
		{
			Name = "Newbie War Hammer";
			Hue = 88;
			LootType = LootType.Blessed;
			Attributes.AttackChance = 10;
			Attributes.DefendChance = 10;
			Attributes.BonusMana = 5;
			Attributes.BonusInt = 5;
			Attributes.BonusStam = 5;
			Attributes.BonusDex = 5;
			Attributes.BonusHits = 10;
			Attributes.BonusStr = 5;
			Attributes.WeaponDamage = 10;
			Attributes.SpellDamage = 10;
			Attributes.SpellChanneling = 1;
			Attributes.LowerManaCost = 10;
			Attributes.WeaponSpeed = 10;
			Attributes.RegenHits = 5;
			Attributes.RegenStam = 5;
			Attributes.RegenMana = 5;
			WeaponAttributes.SelfRepair = 10;
			WeaponAttributes.HitLeechStam = 5;
			WeaponAttributes.HitLeechMana = 5;
			WeaponAttributes.HitLeechHits = 5;
			WeaponAttributes.HitFireball = 25;
			WeaponAttributes.HitMagicArrow = 25;
			WeaponAttributes.HitLightning = 25;
			WeaponAttributes.HitHarm = 25;
			WeaponAttributes.HitLowerAttack = 25;
			WeaponAttributes.HitLowerDefend = 25;
			WeaponAttributes.HitDispel = 25;
			WeaponAttributes.MageWeapon = 1;
		}

		public NewbieWarHammer( Serial serial ) : base( serial )
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
