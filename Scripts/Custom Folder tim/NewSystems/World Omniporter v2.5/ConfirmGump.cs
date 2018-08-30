using System;
using Server;
using System.Collections.Generic;

namespace Server.Gumps
{
	public delegate void ConfirmGumpCallback( Mobile from, bool okay );

	public class ConfirmGump : Gump
	{
		private ConfirmGumpCallback m_Callback;

		public ConfirmGump( string header, int headerColor, string content, int contentColor, int width, int height, ConfirmGumpCallback callback ) : base( (640 - width) / 2, (480 - height) / 2 )
		{
			m_Callback = callback;

			Closable = false;

			AddPage( 0 );

			AddBackground( 0, 0, width, height, 5054 );

			AddImageTiled( 10, 10, width - 20, 20, 2624 );
			AddAlphaRegion( 10, 10, width - 20, 20 );
			AddHtml( 10, 10, width - 20, 20, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", headerColor, header ), false, false );

			AddImageTiled( 10, 40, width - 20, height - 80, 2624 );
			AddAlphaRegion( 10, 40, width - 20, height - 80 );

			AddHtml( 10, 40, width - 20, height - 80, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", contentColor, content ), false, true );

			AddImageTiled( 10, height - 30, width - 20, 20, 2624 );
			AddAlphaRegion( 10, height - 30, width - 20, 20 );

			AddButton( 10, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 40, height - 30, 170, 20, 1011036, 32767, false, false ); // OKAY

			AddButton( 10 + ((width - 20) / 2), height - 30, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 40 + ((width - 20) / 2), height - 30, 170, 20, 1011012, 32767, false, false ); // CANCEL
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			Mobile m = sender.Mobile;
			if ( info.ButtonID == 1 && m_Callback != null )
				m_Callback( m, true );
			else if ( m_Callback != null )
				m_Callback( m, false );
		}
	}
}