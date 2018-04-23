using System;
using Server;

namespace Server.Items
{
	public class DjinnisBracelet : SilverBracelet
	{
		public override int LabelNumber{ get{ return 1094927; } } // Djinni's Ring [Replica]

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		[Constructable]
		public DjinnisBracelet()
		{
			Name = "Djinni's Bracelet [Replica]";
		
			Attributes.BonusInt = 5;
			Attributes.SpellDamage = 10;
			Attributes.CastSpeed = 2;
		}

		public DjinnisBracelet( Serial serial ) : base( serial )
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
		}
	}
}
