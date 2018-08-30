//Created by Neptune
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AssassinStaff : DoubleBladedStaff
  {
		public override int OldMinDamage{ get{ return 25; } }
		public override int AosMinDamage{ get{ return 25; } }
		public override int OldMaxDamage{ get{ return 40; } }
		public override int AosMaxDamage{ get{ return 40; } }


      [Constructable]
		public AssassinStaff()
		{
          Name = "Assassin Staff";
      WeaponAttributes.HitDispel = 25;
      WeaponAttributes.HitLeechHits = 85;
      WeaponAttributes.HitLeechMana = 85;
      WeaponAttributes.HitLeechStam = 85;
      WeaponAttributes.HitLowerAttack = 25;
      WeaponAttributes.HitLowerDefend = 25;
      Attributes.AttackChance = 25;
      Attributes.BonusDex = 25;
      Attributes.BonusInt = 25;
      Attributes.DefendChance = 25;
      Attributes.LowerManaCost = 15;
      Attributes.LowerRegCost = 15;
      Attributes.SpellChanneling = 1;
      Attributes.SpellDamage = 25;
      Attributes.WeaponDamage = 70;
      Attributes.WeaponSpeed = 35;
	Layer = Layer.OneHanded;
		}

		public AssassinStaff( Serial serial ) : base( serial )
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
