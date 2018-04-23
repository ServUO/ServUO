using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class BookOfKnowledge : Spellbook, ITokunoDyable
	{
		[Constructable]
		public BookOfKnowledge() : base()
		{
			Name = "Book Of Knowledge";
			Hue = 1171;
			
			Attributes.SpellDamage = 25;
			Attributes.LowerManaCost = 5;
			Attributes.LowerRegCost = 5;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
			LootType = LootType.Regular;
		}

		public BookOfKnowledge( Serial serial ) : base( serial )
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

