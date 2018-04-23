using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class RamusNecromanticScalpel : ButcherKnife, ITokunoDyable
  {
		public override int ArtifactRarity{ get{ return 15; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public RamusNecromanticScalpel()
		{
          Name = "Ramus' Necromantic Scalpel";
          Hue = 1372;
      WeaponAttributes.HitLeechHits = 60;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 20;
     Slayer = SlayerName.Repond ;
		}

		public RamusNecromanticScalpel( Serial serial ) : base( serial )
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
