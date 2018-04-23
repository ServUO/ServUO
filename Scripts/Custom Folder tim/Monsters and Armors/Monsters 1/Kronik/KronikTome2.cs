
using System;
using Server;

namespace Server.Items
{
	public class KronikTome2 : Spellbook
	{

		[Constructable]
		public KronikTome2()
		{
			Name = "Tome of Doom";
			Hue = 2351;
		  	Content = 18446744073709551615;
		 
                  Attributes.AttackChance = 100;
                  Attributes.DefendChance = 100;
                  Attributes.ReflectPhysical = 100;
                  Attributes.SpellDamage = 100;
		  Attributes.LowerRegCost = 500;
		  Attributes.LowerManaCost = 250;
		}

		public KronikTome2( Serial serial ) : base( serial )
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