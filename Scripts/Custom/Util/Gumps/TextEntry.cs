using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;

namespace Server.Gumps
{
	public interface ITextEntry
	{
		void OnTextEntered( string enteredText );
		void OnTextEntryCanceled( );
	}


	public class TextEntry
	{
		public static void SendTextEntryGump( Mobile callingPlayer, ITextEntry parentScript )
		{
			callingPlayer.SendGump( new TextEntryGump( callingPlayer, parentScript, "", "" ) );
		}

		public static void SendTextEntryGump( Mobile callingPlayer, ITextEntry parentScript, string line1, string line2 )
		{
			callingPlayer.SendGump( new TextEntryGump( callingPlayer, parentScript, line1, line2 ) );
		}

		private class TextEntryGump : Gump
		{
			public const int gumpOffsetX = 50;
			public const int gumpOffsetY = 50;
			public const int playersPerPage = 15;

			private ITextEntry m_ParentScript;

			public TextEntryGump( Mobile callingPlayer, ITextEntry parentScript, string line1, string line2 ) : base ( gumpOffsetX, gumpOffsetY )
			{
				m_ParentScript = parentScript;
				callingPlayer.CloseGump( typeof( TextEntryGump ) );

				BuildCurrentGumpPage( line1, line2);
			}

			public void BuildCurrentGumpPage( string line1, string line2 )
			{
				AddPage( 0 );
				AddBackground( gumpOffsetX, gumpOffsetY, 543, 180, GumpUtil.Background_PlainGrey );
				AddImageTiled( (gumpOffsetX+10), (gumpOffsetY+10), 523, 160, GumpUtil.Background_PureBlack );
				AddImageTiled( (gumpOffsetX+11), (gumpOffsetY+11), 521, 158, 0xbbc );

				AddLabel( (gumpOffsetX+40), (gumpOffsetY+30), 0, line1 );
				AddLabel( (gumpOffsetX+40), (gumpOffsetY+50), 0, line2 );

				AddBackground( (gumpOffsetX+40), (gumpOffsetY+80), 460, 40, GumpUtil.Background_BlackEdged );
				AddTextEntry( (gumpOffsetX+50), (gumpOffsetY+88), 450, 20, 0x480, 0, "" );


				AddButton( (gumpOffsetX+300), (gumpOffsetY+130), GumpUtil.ButtonRedCancelUp, GumpUtil.ButtonRedCancelDown, 0, GumpButtonType.Reply, 0 );
				AddButton( (gumpOffsetX+400), (gumpOffsetY+130), GumpUtil.ButtonGreenOKUp, GumpUtil.ButtonGreenOKDown, GumpUtil.BUTTONID_CONFIRM, GumpButtonType.Reply, 0 );
			}

			//Handles button presses
			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile player = state.Mobile;
				switch ( info.ButtonID )
				{
					case 0: // Closed
					{
						player.SendMessage( "Canceled." );
						m_ParentScript.OnTextEntryCanceled();
						return;
					}
					case GumpUtil.BUTTONID_CONFIRM: // OK
					{
						string theMessage = info.GetTextEntry( 0 ).Text;
						if ( theMessage == String.Empty )
						{
							player.SendMessage( "No text was entered." );
							m_ParentScript.OnTextEntryCanceled();
							return;
						}
						
						m_ParentScript.OnTextEntered( theMessage );
						break;
					}
					default:
					{
						player.SendMessage( "Error:  invalid gump return" );
						break;
					}
				}
			}
		}
	}
}


