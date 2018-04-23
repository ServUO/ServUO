//Created with Script Creator By Marak & Rockstar
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class HydraKryss : Kryss
  {
		public override int OldMinDamage{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 20; } }
		public override int OldMaxDamage{ get{ return 25; } }
		public override int AosMaxDamage{ get{ return 25; } }


      [Constructable]
		public HydraKryss()
		{
          Name = "Fang of the Hydra";
      WeaponAttributes.HitLeechHits = 150;
      WeaponAttributes.HitLeechMana = 150;
      WeaponAttributes.HitLeechStam = 150;
      Attributes.AttackChance = 50;
      Attributes.BonusDex = 20;
      Attributes.BonusHits = 20;
      Attributes.BonusInt = 20;
      Attributes.BonusMana = 5;
      Attributes.BonusStam = 10;
      Attributes.DefendChance = 50;
      Attributes.LowerManaCost = 100;
      Attributes.LowerRegCost = 200;
      Attributes.ReflectPhysical = 25;
      Attributes.SpellChanneling = 1;
      Attributes.SpellDamage = 75;
      Attributes.WeaponDamage = 200;
      Attributes.WeaponSpeed = 100;
      Attributes.BonusMana = 5;
		}

		public HydraKryss( Serial serial ) : base( serial )
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
