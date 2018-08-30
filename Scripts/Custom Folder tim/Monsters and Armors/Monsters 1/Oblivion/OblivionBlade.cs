//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class OblivionBlade : Katana
  {
		public override int OldMinDamage{ get{ return 18; } }
		public override int AosMinDamage{ get{ return 18; } }
		public override int OldMaxDamage{ get{ return 24; } }
		public override int AosMaxDamage{ get{ return 24; } }


      [Constructable]
		public OblivionBlade()
		{
          Name = "Blade of Oblivion";
          Hue = 1261;
      WeaponAttributes.HitLeechHits = 50;
      WeaponAttributes.HitLeechMana = 50;
      WeaponAttributes.HitLeechStam = 50;
      WeaponAttributes.HitLowerAttack = 50;
      WeaponAttributes.SelfRepair = 5;
      Attributes.AttackChance = 25;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 3;
      Attributes.DefendChance = 25;
      Attributes.LowerManaCost = 15;
      Attributes.LowerRegCost = 30;
      Attributes.NightSight = 1;
      Attributes.RegenHits = 3;
      Attributes.RegenMana = 6;
      Attributes.RegenStam = 12;
      Attributes.SpellChanneling = 1;
      Attributes.SpellDamage = 15;
      Attributes.WeaponDamage = 55;
      Attributes.WeaponSpeed = 55;
		}

		public OblivionBlade( Serial serial ) : base( serial )
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
