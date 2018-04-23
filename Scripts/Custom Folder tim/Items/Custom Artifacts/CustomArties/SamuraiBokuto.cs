using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class SamuraiBokuto : Bokuto, ITokunoDyable

  {
      public override int ArtifactRarity{ get{ return 12; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public SamuraiBokuto()
		{
          Name = "Samurai's Bokuto";
          Hue = 2409;
      WeaponAttributes.HitLeechHits = 55;
      WeaponAttributes.HitLowerAttack = 10;
      WeaponAttributes.HitLowerDefend = 25;
      Attributes.AttackChance = 15;
      Attributes.BonusDex = 10;
      Attributes.DefendChance = 5;
      Attributes.WeaponDamage = 45;
      Attributes.WeaponSpeed = 20;
		}

		public SamuraiBokuto( Serial serial ) : base( serial )
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
