
using System;
using Server;

namespace Server.Items
{
	public class KronikTome : Spellbook
	{

		[Constructable]
		public KronikTome()
		{
			Name = "Tome of Kronik";
			Hue = 2351;
		  	Content = 18446744073709551615;
			Layer = Layer.Talisman;
		 
                  Attributes.BonusDex = 10;
                  Attributes.BonusHits = 10;
                  Attributes.BonusStam = 10;
                  Attributes.BonusStr = 10;
		  Attributes.BonusMana = 10;
		  Attributes.BonusInt = 10;
                  Attributes.CastRecovery = 10;
                  Attributes.CastSpeed = 10;
                  Attributes.SpellDamage = 25;
		  Attributes.WeaponDamage = 25;
		  Attributes.WeaponSpeed = 25;
		}

		public KronikTome( Serial serial ) : base( serial )
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

			if ( Hue == 0x55A )
				Hue = 0x4F6;
		}
	}
}