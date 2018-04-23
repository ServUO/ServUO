//Created by xG00BERx

using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ShroudOfTheThieves : HoodedShroudOfShadows
	{

		[Constructable]
		public ShroudOfTheThieves()
		{
			Hue = 919;
			Name = "Shroud Of The Thieves";

			SkillBonuses.SetValues( 0, SkillName.Stealing, 15.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 15.0 );
			SkillBonuses.SetValues( 2, SkillName.Snooping, 15.0 );

			LootType = LootType.Blessed;
			
		}
		
		public override bool OnEquip(Mobile m) 
	      { 
			m.NameMod = "A Thief";
			m.DisplayGuildTitle = false;
			m.SendMessage( "Your face is Shrouded by the Robe and you have now look like another thief" );
			return base.OnEquip(m); 
		}
		
		public override void OnRemoved( object parent) 
	      { 
		if (parent is Mobile) 
	        { 
	         Mobile m = (Mobile)parent; 
		   m.NameMod = null;
		   m.SendMessage( "You remove the Shroud and you become recognizable" );
		   m.DisplayGuildTitle = true;
		  }

	         base.OnRemoved(parent); 
      	}

		public ShroudOfTheThieves ( Serial serial ) : base( serial )
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