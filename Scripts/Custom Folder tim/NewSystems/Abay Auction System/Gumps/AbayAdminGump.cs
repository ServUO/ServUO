#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;

using Server;
using Server.Gumps;
using Xanthos.Utilities;

namespace Arya.Abay
{
	/// <summary>
	/// The admin gump for the Abay system
	/// </summary>
	public class AbayAdminGump : Gump
	{
		public AbayAdminGump( Mobile m ) : base ( 100, 100 )
		{
			m.CloseGump( typeof( AbayAdminGump ) );
			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddBackground(0, 0, 270, 270, 9300);
			AddAlphaRegion(0, 0, 270, 270);
			AddLabel(36, 5, Xanthos.Utilities.AbayLabelHue.kRedHue, @"Abay System Administration");
			AddImageTiled(16, 30, 238, 1, 9274);

			AddLabel(15, 65, AbayLabelHue.kLabelHue, string.Format( @"Deadline: {0} at {1}", AbayScheduler.Deadline.ToShortDateString(), AbayScheduler.Deadline.ToShortTimeString() ) );
			AddLabel(15, 40, AbayLabelHue.kGreenHue, string.Format( @"{0} Abays, {1} Pending", AbaySystem.Abays.Count, AbaySystem.Pending.Count ) );

			// B 1 : Validate
			AddButton(15, 100, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddLabel(55, 100, AbayLabelHue.kLabelHue, @"Force Verification");

			// B 2 : Profile
			AddButton(15, 130, 4005, 4006, 2, GumpButtonType.Reply, 0);
			AddLabel(55, 130, AbayLabelHue.kLabelHue, @"Profile the System");

			// B 3 : Temporary Shutdown
			AddButton(15, 160, 4005, 4006, 3, GumpButtonType.Reply, 0);
			AddLabel(55, 160, AbayLabelHue.kLabelHue, @"Temporarily Shut Down");

			// B 4 : Delete
			AddButton(15, 190, 4005, 4006, 4, GumpButtonType.Reply, 0);
			AddLabel(55, 190, AbayLabelHue.kLabelHue, @"Permanently Shut Down");

			// B 0 : Close
			AddButton(15, 230, 4023, 4024, 0, GumpButtonType.Reply, 0);
			AddLabel(55, 230, AbayLabelHue.kLabelHue, @"Exit");
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 1: // Validate

					AbaySystem.VerifyAbays();
					AbaySystem.VerifyPendencies();

					sender.Mobile.SendGump( new AbayAdminGump( sender.Mobile ) );
					break;

				case 2: // Profile

					AbaySystem.ProfileAbays();

					sender.Mobile.SendGump( new AbayAdminGump( sender.Mobile ) );
					break;

				case 3: // Disable

					AbaySystem.Disable();
					sender.Mobile.SendMessage( AbayConfig.MessageHue, "The system has been stopped. It will be restored with the next reboot." );
					break;

				case 4: // Delete

					sender.Mobile.SendGump( new DeleteAbayGump( sender.Mobile ) );
					break;
			}
		}

	}
}
