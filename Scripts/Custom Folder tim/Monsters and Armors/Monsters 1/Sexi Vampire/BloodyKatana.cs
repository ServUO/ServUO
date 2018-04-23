//Created by Needles
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class BloodyKatana : Katana
  {
		public override int OldMinDamage{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 20; } }
		public override int OldMaxDamage{ get{ return 40; } }
		public override int AosMaxDamage{ get{ return 40; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int DefMaxRange{ get{ return 5; } }

      [Constructable]
		public BloodyKatana()
		{
          Name = "Bloody Katana";
          Hue = 2939;
      Attributes.BonusStr = 50;
      Attributes.BonusDex = 30;
      Attributes.BonusInt = 10;
      WeaponAttributes.SelfRepair = 5;
      WeaponAttributes.UseBestSkill = 1;
      Attributes.AttackChance = 20;
      Attributes.DefendChance = 20;
      Attributes.ReflectPhysical = 20;
      WeaponAttributes.HitHarm = 100;
      WeaponAttributes.HitLightning = 75;
      WeaponAttributes.HitFireball = 50;
      Attributes.SpellChanneling = 1;
      Attributes.SpellDamage = 20;
      Attributes.WeaponDamage = 20;
      Attributes.WeaponSpeed = 20;
      LootType = LootType.Regular;
     Slayer = SlayerName.Repond ;
		}

		public BloodyKatana( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
