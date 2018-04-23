// Scripted by SPanky
using System;
using Server;

namespace Server.Items
{
	public class swordofthelight : Katana
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

		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }  //Only select one Secondary
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MovingShot; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int InitMinHits{ get{ return 255; } } // Set the minium amount of hit points for the weapon.
		public override int InitMaxHits{ get{ return 255; } } // Set the Maxium amount of hit points for the weapon.

		[Constructable]
		public swordofthelight()
		{
			Weight = 3.0;
            		Name = "Sword of the Light";
            		Hue = 1153;

			//WeaponAttributes.DurabilityBonus = nn; // Pick and choose the attributes for your weapon (remember to remove the // before the ones you entend to use)
			//WeaponAttributes.HitColdArea = nn;
			//WeaponAttributes.HitDispel = nn;
			//WeaponAttributes.HitEnergyArea = nn;
			//WeaponAttributes.HitFireArea = nn;
			//WeaponAttributes.HitFireBall = nn;
			//WeaponAttributes.HitHarm = nn;
			//WeaponAttributes.HitLeechHits = nn;
			//WeaponAttributes.HitLeechMana = nn;
			//WeaponAttributes.HitLeechStam = nn;                                   
			//WeaponAttributes.HitLightning = nn;
			//WeaponAttributes.HitLowerAttack = nn;
			//WeaponAttributes.HitLowerDefence = nn;
			//WeaponAttributes.HitMagicArrow = nn;
			//WeaponAttributes.HitPhysicalArea = nn;
			//WeaponAttributes.HitPoisonArea = nn;
			//WeaponAttributes.LowerStatReq = nn;
			WeaponAttributes.MageWeapon = 1;    
			//WeaponAttributes.ResistColdBonus = nn;
			//WeaponAttributes.ResistEnergyBonus = nn;
			//WeaponAttributes.ResistPhysicalBonus = nn;
			//WeaponAttributes.ResistPoisonBonus = nn;
			WeaponAttributes.SelfRepair = 20;
			//WeaponAttributes.UseBestSkill = 1;

			Attributes.AttackChance = 30;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			//Attributes.BonusMana = nn;
			//Attributes.BonusStam = nn;
			//Attributes.BonusStr = nn;
			Attributes.CastRecovery = 16;
			Attributes.CastSpeed = 12;
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
			//Attributes.WeaponDamage = nn;
			//Attributes.WeaponSpeed = nn;

			//Consecrated = true; 
			//Cursed = true;
			DexRequirement = 50;
			IntRequirement = 45;
			//Slayer = SlayerName.nn;
			StrRequirement = 60;

			LootType = LootType.Blessed;
		}

		public swordofthelight( Serial serial ) : base( serial )
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