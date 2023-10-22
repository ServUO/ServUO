#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos with contributions by Dracna
//	
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Xanthos.Evo
{
	public delegate void EvoActionGumpCallback( Mobile from, bool okay, object state );

	public class EvoActionGump : Gump
	{
		private EvoActionGumpCallback m_Callback;
		private object m_State;

		public EvoActionGump( EvoActionGumpCallback callback, object state ) : base( (640 - 260) / 2, (480 - 76) / 2 )
		{
			m_Callback = callback;
			m_State = state;

			Closable = false;

			AddPage( 0 );

			this.AddBackground(0, 0, 260, 76, 9200);
			this.AddLabel(17, 10, 0, @"What action would you like to take?");
			this.AddLabel(48, 41, 573, @"Mount");
			this.AddButton(13, 40, 4005, 4007, 0, GumpButtonType.Reply, 0);
			this.AddLabel(201, 41, 573, @"Breed");
			this.AddButton(165, 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 && m_Callback != null )
				m_Callback( sender.Mobile, true, m_State );
			else if ( m_Callback != null )
				m_Callback( sender.Mobile, false, m_State );
		}
	}
}