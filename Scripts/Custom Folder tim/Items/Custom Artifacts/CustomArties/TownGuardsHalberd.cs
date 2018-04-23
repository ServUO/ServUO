using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class TownGuardsHalberd : Halberd, ITokunoDyable
    {

public override int ArtifactRarity{ get{ return 12; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public TownGuardsHalberd()
		{
          Name = "Town Guard's Halberd";
		Hue = 1407;
      WeaponAttributes.HitLightning = 100;
      WeaponAttributes.HitLowerDefend = 40;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 25;
     Slayer = SlayerName.Repond ;
		}

		public TownGuardsHalberd( Serial serial ) : base( serial )
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
