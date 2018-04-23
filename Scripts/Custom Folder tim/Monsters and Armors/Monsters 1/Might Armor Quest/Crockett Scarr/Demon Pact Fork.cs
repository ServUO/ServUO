//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class DemonPactFork : WarFork
  {
		public override int OldMinDamage{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 35; } }
		public override int OldMaxDamage{ get{ return 35; } }
		public override int AosMaxDamage{ get{ return 35; } }


      [Constructable]
		public DemonPactFork()
		{
          Name = "Demon Pact Fork";
      WeaponAttributes.HitDispel = 75;
      WeaponAttributes.HitFireArea = 75;
      WeaponAttributes.HitFireball = 75;
      WeaponAttributes.SelfRepair = 5;
      Attributes.AttackChance = 25;
      Attributes.DefendChance = 25;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 25;
     Slayer = SlayerName.Repond ;
		}

		public DemonPactFork( Serial serial ) : base( serial )
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
