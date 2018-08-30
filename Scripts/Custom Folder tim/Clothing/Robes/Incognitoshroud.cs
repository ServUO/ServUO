//Created by Jareish/Infernal Reign/Legion for the Shard Elysium Isles

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class Incognitoshroud : HoodedShroudOfShadows
	{

		[Constructable]
		public Incognitoshroud()
		{
			Hue = 1272;
			Name = "Shroud of Icognito";
			LootType = LootType.Blessed;
			
		}
		
		public override bool OnEquip(Mobile m) 
	      { 
			m.NameMod = "a Hooded Figure";
			m.DisplayGuildTitle = false;
			m.SendMessage( "Your face becomes hidden and you are unrecognisable" );
			return base.OnEquip(m); 
		}
		
		public override void OnRemoved( object parent) 
	      { 
		if (parent is Mobile) 
	        { 
	         Mobile m = (Mobile)parent; 
		   m.NameMod = null;
		   m.SendMessage( "You remove the Shroud and your face becomes clear" );
		   m.DisplayGuildTitle = true;
		  }

	         base.OnRemoved(parent); 
      	}

		public Incognitoshroud ( Serial serial ) : base( serial )
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