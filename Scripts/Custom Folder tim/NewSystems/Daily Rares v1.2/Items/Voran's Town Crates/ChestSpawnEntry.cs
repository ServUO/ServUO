using System;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;

namespace Server.ContextMenus
{
	public class ChestSpawnEntry : ContextMenuEntry
	{
		private VoransTownCrate m_Item;
		private Mobile m_From;

		public ChestSpawnEntry( VoransTownCrate item, Mobile from ) : base(0554 )
		{
			m_From = from;
			m_Item=item;
		}

		public override void OnClick()
		{
			VoransTownCrateGump g = new VoransTownCrateGump( m_Item ); 
			m_From.SendGump( g ); 
			return;
			
		}
	}
}
