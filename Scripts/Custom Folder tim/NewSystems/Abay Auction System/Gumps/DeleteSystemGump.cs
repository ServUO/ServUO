#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;

using Server;
using Server.Gumps;
using Xanthos.Utilities;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for DeleteSystemGump.
	/// </summary>
	public class DeleteAbayGump : Gump
	{
		public DeleteAbayGump( Mobile m ) : base( 100, 100 )
		{
			m.CloseGump( typeof( DeleteAbayGump ) );
			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImageTiled(0, 0, 350, 250, 5174);
			AddImageTiled(1, 1, 348, 248, 2702);
			AddAlphaRegion(1, 1, 348, 248);
			AddLabel(70, 15, AbayLabelHue.kRedHue, AbaySystem.ST[ 96 ] );
			AddHtml( 30, 45, 285, 130, AbaySystem.ST[ 97 ] , (bool)false, (bool)false);

			// Close: Button 1
			AddButton(30, 185, 4017, 4017, 1, GumpButtonType.Reply, 0);
			AddLabel(70, 185, AbayLabelHue.kRedHue, AbaySystem.ST[ 98 ] );
			AddButton(30, 215, 4014, 4015, 0, GumpButtonType.Reply, 0);
			AddLabel(70, 215, AbayLabelHue.kGreenHue, AbaySystem.ST[ 99 ] );
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if ( info.ButtonID == 1 )
			{
				AbaySystem.ForceDelete( sender.Mobile );
			}
		}
	}
}
