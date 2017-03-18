#region Header
// **************************************\
//     _  _   _   __  ___  _   _   ___   |
//    |# |#  |#  |## |### |#  |#  |###   |
//    |# |#  |# |#    |#  |#  |# |#  |#  |
//    |# |#  |#  |#   |#  |#  |# |#  |#  |
//   _|# |#__|#  _|#  |#  |#__|# |#__|#  |
//  |##   |##   |##   |#   |##    |###   |
//        [http://www.playuo.org]        |
// **************************************/
//  [2014] QuestArrow.cs
// ************************************/
#endregion

using System;
using Server.Prompts;
using Server.Gumps;

namespace Server.Network
{
	public static class PromptExtensions
	{
		public static void SendTo( this Prompt prompt, Mobile m )
		{
            if (m.NetState != null && m.NetState.IsEnhancedClient)
			{
				m.Send( new PromptGumpStub( prompt, m ).GetPacket() );
			}
			else
			{
				if ( prompt.MessageCliloc != 1042971 || prompt.MessageArgs != String.Empty )
					m.SendLocalizedMessage( prompt.MessageCliloc, prompt.MessageArgs, prompt.MessageHue );

				m.Send( new UnicodePrompt( prompt, m ) );
			}
		}
	}

	public class PromptGumpStub : Gump
	{
		public Mobile User { get; private set; }

		public override int GetTypeID()
        {
            return 0x2AE;
        }

		public PromptGumpStub( Prompt prompt, Mobile to )
			: base( 0, 0 )
		{
			User = to;

			Serial senderSerial = prompt.Sender != null ? prompt.Sender.Serial : to.Serial;

			Serial = senderSerial;

			Intern( "TEXTENTRY" );
			Intern( senderSerial.Value.ToString() );
			Intern( to.Serial.Value.ToString() );
			Intern( prompt.TypeId.ToString() );
			Intern( prompt.MessageCliloc.ToString() ); // TODO: Is there a way to include args here?
			Intern( "1" ); // 0 = Ascii response, 1 = Unicode Response

			AddBackground( 50, 50, 540, 350, 0xA28 );

			AddPage( 0 );

			AddHtmlLocalized( 264, 80, 200, 24, 1062524, false, false );
			AddHtmlLocalized( 120, 108, 420, 48, 1062638, false, false );
			AddBackground( 100, 148, 440, 200, 0xDAC );
			AddTextEntryIntern( 120, 168, 400, 200, 0x0, 44, 0 );
			AddButton( 175, 355, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0 );
			AddButton( 405, 355, 0x819, 0x818, 0, GumpButtonType.Reply, 0 );
		}

		public Packet GetPacket()
		{
			return GetPacketFor(User != null ? User.NetState : null);
		}
	}
}
