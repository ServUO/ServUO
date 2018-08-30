//created by thorshammer//

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class EmperorsDaisho : BaseSword
	{
		public override int ArtifactRarity{ get{ return 1000; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }
		public override float MlSpeed{ get{ return 1.50f; } }

		[Constructable]
		public EmperorsDaisho() : base( 0x27A9 )
		{
			Name = "Emperors Daisho";
			Hue = 432;
			LootType = LootType.Blessed;
			Weight = 10;
			SkillBonuses.SetValues( 0, SkillName.Anatomy, 10 );
			SkillBonuses.SetValues( 1, SkillName.Bushido, 10 );
			SkillBonuses.SetValues( 2, SkillName.Parry, 10 );
			SkillBonuses.SetValues( 3, SkillName.Tactics, 10 );
			SkillBonuses.SetValues( 4, SkillName.Swords, 10 );
			Attributes.AttackChance = 33;
			Attributes.DefendChance = 33;
			Attributes.BonusDex = 15;
			Attributes.BonusHits = 20;
			Attributes.BonusStr = 15;
			Attributes.Luck = 300;
			Attributes.CastSpeed = 4;
			Attributes.CastRecovery = 3;
			Attributes.WeaponDamage = 40;
			Attributes.SpellDamage = 10;
			Attributes.ReflectPhysical = 15;
			Attributes.SpellChanneling = 1;
			Attributes.LowerManaCost = 5;
			Attributes.LowerRegCost = 5;
			Attributes.NightSight = 1;
			Attributes.WeaponSpeed = 35;
			Attributes.RegenHits = 8;
			Attributes.RegenMana = 4;
			WeaponAttributes.LowerStatReq = 100;
			WeaponAttributes.SelfRepair = 8;
			WeaponAttributes.HitLeechMana = 30;
			WeaponAttributes.HitLeechHits = 30;
			WeaponAttributes.HitFireball = 20;
			WeaponAttributes.HitLightning = 20;
			WeaponAttributes.HitHarm = 20;
			WeaponAttributes.HitLowerAttack = 30;
			WeaponAttributes.HitLowerDefend = 30;
			WeaponAttributes.HitPhysicalArea = 20;
			WeaponAttributes.HitPoisonArea = 20;
			WeaponAttributes.HitEnergyArea = 20;
			WeaponAttributes.DurabilityBonus = 100;
			WeaponAttributes.ResistPhysicalBonus = 10;
			WeaponAttributes.ResistFireBonus = 10;
			WeaponAttributes.ResistColdBonus = 10;
			WeaponAttributes.ResistPoisonBonus = 10;
			WeaponAttributes.ResistEnergyBonus = 10;
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			pois = 50;
			phys = 50;

			cold = fire = nrgy = chaos = direct = 0;
		}

		public EmperorsDaisho( Serial serial ) : base( serial )
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
