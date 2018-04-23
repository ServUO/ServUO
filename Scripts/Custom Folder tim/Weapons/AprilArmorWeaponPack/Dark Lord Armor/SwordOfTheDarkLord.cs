// Scripted by SPanky
using System;
using Server;

namespace Server.Items
{
	public class swordofthedarklord : Katana
	{
		public override int ArtifactRarity{ get{ return 60; } } 

		//public override int EffectID{ get{ return nn; } } // 0x1BFE (bolt) or 0xF42 (arrow)
		//public override Type AmmoType{ get{ return typeof( nn ); } } // Bolt or Arrow
		//public override Item Ammo{ get{ return new nn(); } } // Bolt or Arrow

		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } } //Only select one primary
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.MovingShot; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Dismount; } }
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }  //Only select one Secondary
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MovingShot; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public swordofthedarklord() 
		{
			Weight = 8.0;
            		Name = "Sword of the Dark Lord";
            		Hue = 1175;

			//WeaponAttributes.DurabilityBonus = nn;
			//WeaponAttributes.HitColdArea = nn;
			//WeaponAttributes.HitDispel = nn;
			//WeaponAttributes.HitEnergyArea = nn;
			//WeaponAttributes.HitFireArea = nn;
			//WeaponAttributes.HitFireBall = nn;
			//WeaponAttributes.HitHarm = nn;
			//WeaponAttributes.HitLeechHits = nn;
			//WeaponAttributes.HitLeechMana = nn;
			//WeaponAttributes.HitLeechStam = nn;                                   
			WeaponAttributes.HitLightning = 12;
			//WeaponAttributes.HitLowerAttack = nn;
			//WeaponAttributes.HitLowerDefence = nn;
			//WeaponAttributes.HitMagicArrow = nn;
			//WeaponAttributes.HitPhysicalArea = nn;
			//WeaponAttributes.HitPoisonArea = nn;
			//WeaponAttributes.LowerStatReq = nn;
			//WeaponAttributes.MageWeapon = 1;    
			//WeaponAttributes.ResistColdBonus = nn;
			//WeaponAttributes.ResistEnergyBonus = nn;
			//WeaponAttributes.ResistPhysicalBonus = nn;
			//WeaponAttributes.ResistPoisonBonus = nn;
			WeaponAttributes.SelfRepair = 24;
			//WeaponAttributes.UseBestSkill = 1;

			Attributes.AttackChance = 26;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			//Attributes.BonusMana = nn;
			//Attributes.BonusStam = nn;
			//Attributes.BonusStr = nn;
			Attributes.CastRecovery = 16;
			Attributes.CastSpeed = 14;
			//Attributes.DefendChance = nn;
			//Attributes.EnhancePotions = nn;
			//Attributes.LowerManaCost = nn;
			//Attributes.LowerRegCost = nn;
			//Attributes.Luck = nn;
			//Attributes.Nightsight = 1;
			//Attributes.ReflectPhysical = nn;
			//Attributes.RegenHits = nn;
			//Attributes.RegenMana = nn;
			//Attributes.RegenStam = nn;
			Attributes.SpellChanneling = 1;
			//Attributes.SpellDamage = nn;
			Attributes.WeaponDamage = 26;
			Attributes.WeaponSpeed = 16;

			//Consecrated = true; 
			//Cursed = true;
			//DexRequirement = nn;
			IntRequirement = 60;
			//Slayer = SlayerName.nn;
			StrRequirement = 40;

			LootType = LootType.Blessed;
		}

		public swordofthedarklord( Serial serial ) : base( serial )
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