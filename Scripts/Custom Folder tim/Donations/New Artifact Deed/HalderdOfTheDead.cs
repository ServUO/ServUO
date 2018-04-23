using System;
using Server;

namespace Server.Items
{
	public class HalberdOfTheDead : Halberd
	{
		public override int ArtifactRarity{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HalberdOfTheDead()
		{
			Name = "Halberd Of The Dead";
			Hue = 1175;
			WeaponAttributes.HitFireArea = 100;
			Slayer = SlayerName.Silver;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = 50;
		}
		

		public HalberdOfTheDead( Serial serial ) : base( serial )
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

			if ( Attributes.CastSpeed == 3 )
				Attributes.CastRecovery = 3;
		}
	}
}