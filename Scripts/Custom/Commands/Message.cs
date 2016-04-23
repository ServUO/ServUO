using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.IO;
using Server.Accounting;

namespace Server.Commands
{
    public class DotCommand_Message
    {
        private static StreamWriter writer;
        private static string LogPath = Path.Combine( "Logs\\Msg", String.Format( "{0}.log", DateTime.Now.ToLongDateString() ) );

        public static void Initialize()
        {
            CommandSystem.Register( "msg", AccessLevel.Player, new CommandEventHandler( OnCommand_msg ) );
            CommandSystem.Register( "who", AccessLevel.Player, new CommandEventHandler(OnCommand_msg));
            CommandSystem.Register( "reply", AccessLevel.Player, new CommandEventHandler( OnCommand_Reply ) );
            CommandSystem.Register( "Last", AccessLevel.Player, new CommandEventHandler( OnCommand_Last ) );
            
        }

        [Usage( "msg [\"name\"] [message]" )]
        [Description( "Sends a message to another player" )]
        private static void OnCommand_msg( CommandEventArgs e )
        {
            new MsgInstance( e.Mobile, e.Arguments, e.ArgString );
        }

        [Usage("msg [\"name\"] [message]")]
        [Description("Sends a message to another player")]
        private static void OnCommand_who(CommandEventArgs e)
        {
            new MsgInstance(e.Mobile, e.Arguments, e.ArgString);
        }

        [Usage( "reply [message]" )]
        [Description( "Replies to the last person to send you a message" )]
        private static void OnCommand_Reply( CommandEventArgs e )
        {
            new ReplyInstance( e.Mobile, e.Arguments, e.ArgString );
        }

        [Usage( "last [message]" )]
        [Description( "Sends a message to the last person you sent a message to" )]
        private static void OnCommand_Last( CommandEventArgs e )
        {
            new LastInstance( e.Mobile, e.Arguments, e.ArgString );
        }

        private class MsgInstance : IPlayerSelect, ITextEntry
        {
            private Mobile m_Player;
            private Mobile m_SelectedPlayer;
            private string m_EnteredMessageText;

            public MsgInstance( Mobile player, string[] args, string argString )
            {
                m_Player = player;
                m_EnteredMessageText = string.Empty;

                if ( args.Length < 1 ) {
                    //They just used .msg by itself, go through the whole online list
                    PlayerSelect.SelectOnlinePlayer( player, this );
                }
                else if ( args.Length == 1 ) {
                    //They entered .msg with a name, but no message
                    PlayerSelect.SelectOnlinePlayer( player, this, args[0] );
                }
                else {
                    //They entered a name and a message.  Sort out the message from the name
                    m_EnteredMessageText = argString.Remove( 0, args[0].Length ).Trim();

                    PlayerSelect.SelectOnlinePlayer( player, this, args[0] );
                }
            }

            public void OnPlayerSelected( PlayerMobile selectedMobile )
            {
                m_SelectedPlayer = selectedMobile;
                if ( m_EnteredMessageText != string.Empty )
                    SendTheMessage( m_EnteredMessageText );
                else
                    TextEntry.SendTextEntryGump( m_Player, this, ( "Message for " + selectedMobile.Name ), "" );
            }

            public void OnPlayerSelectCanceled()
            {
                return;
            }

            public void OnTextEntered( string enteredText )
            {
                SendTheMessage( enteredText );
            }

            public void OnTextEntryCanceled()
            {
                return;
            }

            private void SendTheMessage( string theMessage )
            {
                //Save the information for .reply and .last
                ReplyInstance.SetReply( m_SelectedPlayer, m_Player );
                LastInstance.SetLast( m_Player, m_SelectedPlayer );

                m_Player.SendMessage( m_Player.SpeechHue, "You said to " + m_SelectedPlayer.Name + ": " + theMessage );
                m_SelectedPlayer.SendMessage( m_Player.SpeechHue, m_Player.Name + " says to you: " + theMessage );
            }
        }

        public static object Format( object o )
        {
            if ( o is Mobile ) {
                Mobile m = (Mobile)o;

                if ( m.Account == null )
                    return String.Format( "{0} (no account)", m );
                else
                    return String.Format( "{0} ('{1}')", m, ( (Account)m.Account ).Username );
            }
            else if ( o is Item ) {
                Item item = (Item)o;

                return String.Format( "0x{0:X} ({1})", item.Serial.Value, item.GetType().Name );
            }

            return o;
        }

        public static void AppendPath( ref string path, string toAppend )
        {
            path = Path.Combine( path, toAppend );

            if ( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
        }


        private class ReplyInstance : ITextEntry
        {
            private Mobile m_Player;
            private Mobile m_SelectedPlayer;


            private static Hashtable m_ReplyListStorage;	//Stores the information used for .reply
            public static void Initialize()
            {
                m_ReplyListStorage = new Hashtable();
            }

            public static void SetReply( Mobile fromMobile, Mobile toMobile )
            {
                m_ReplyListStorage[fromMobile] = toMobile;
            }




            public ReplyInstance( Mobile player, string[] args, string argString )
            {
                m_Player = player;
                if ( !m_ReplyListStorage.Contains( m_Player ) ) {
                    m_Player.SendMessage( "You haven't recieved any messages yet." );
                    return;
                }

                m_SelectedPlayer = (Mobile)m_ReplyListStorage[m_Player];
                if ( m_SelectedPlayer == null || m_SelectedPlayer.Deleted ) {
                    m_Player.SendMessage( "That player is no longer online" );
                    return;
                }

                if ( args.Length < 1 ) {
                    //They just used .reply by itself, so send them a text entry gump
                    TextEntry.SendTextEntryGump( m_Player, this, ( "Message for " + m_SelectedPlayer.Name ), "" );
                }
                else {
                    //Otherwise, they entered a message, so send it
                    SendTheMessage( argString );
                }
            }

            public void OnTextEntered( string enteredText )
            {
                SendTheMessage( enteredText );
            }

            public void OnTextEntryCanceled()
            {
                return;
            }

            private void SendTheMessage( string theMessage )
            {
                //Save the information for .reply
                ReplyInstance.SetReply( m_SelectedPlayer, m_Player );

                m_Player.SendMessage( m_Player.SpeechHue, "You said to " + m_SelectedPlayer.Name + ": " + theMessage );
                m_SelectedPlayer.SendMessage( m_Player.SpeechHue, m_Player.Name + " says to you: " + theMessage );
            }
        }



        private class LastInstance : ITextEntry
        {
            private Mobile m_Player;
            private Mobile m_SelectedPlayer;


            private static Hashtable m_LastListStorage;	//Stores the information used for .reply
            public static void Initialize()
            {
                m_LastListStorage = new Hashtable();
            }

            public static void SetLast( Mobile fromMobile, Mobile toMobile )
            {
                m_LastListStorage[fromMobile] = toMobile;
            }




            public LastInstance( Mobile player, string[] args, string argString )
            {
                m_Player = player;
                if ( !m_LastListStorage.Contains( m_Player ) ) {
                    m_Player.SendMessage( "You haven't sent any messages yet." );
                    return;
                }

                m_SelectedPlayer = (Mobile)m_LastListStorage[m_Player];
                if ( m_SelectedPlayer == null || m_SelectedPlayer.Deleted ) {
                    m_Player.SendMessage( "That player is no longer online" );
                    return;
                }

                if ( args.Length < 1 ) {
                    //They just used .reply by itself, so send them a text entry gump
                    TextEntry.SendTextEntryGump( m_Player, this, ( "Message for " + m_SelectedPlayer.Name ), "" );
                }
                else {
                    //Otherwise, they entered a message, so send it
                    SendTheMessage( argString );
                }
            }

            public void OnTextEntered( string enteredText )
            {
                SendTheMessage( enteredText );
            }

            public void OnTextEntryCanceled()
            {
                return;
            }

            private void SendTheMessage( string theMessage )
            {
                //Save the information for .reply
                ReplyInstance.SetReply( m_SelectedPlayer, m_Player );

                m_Player.SendMessage( m_Player.SpeechHue, "You said to " + m_SelectedPlayer.Name + ": " + theMessage );
                m_SelectedPlayer.SendMessage( m_Player.SpeechHue, m_Player.Name + " says to you: " + theMessage );
            }
        }
    }
}




