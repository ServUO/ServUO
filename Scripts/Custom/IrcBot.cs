#region Header

/*
  * Copyright (c) 2002 Aaron Hunter
  * All rights reserved.
  *
  * Redistribution and use in source and binary forms, with or without
  * modification, are permitted provided that the following conditions
  * are met:
  *
  * 1. Redistributions of source code must retain the above copyright
  *     notice, this list of conditions and the following disclaimer.
  * 2. Redistributions in binary form must reproduce the above copyright
  *     notice, this list of conditions and the following disclaimer in the
  *     documentation and/or other materials provided with the distribution.
  * 3. The name of the author may not be used to endorse or promote products
  *     derived from this software without specific prior written permission.
  *
  * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
  * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
  * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
  * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
  * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
  * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
  * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
  * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
  * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  * WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion Header

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Sharkbite.Irc;

using Server;
using Server.Commands;
using Server.Engines.Help;

using Server.Mobiles;
using System.Net.Mail;
using Server.Accounting;

namespace Sharkbite.Irc.Examples
{
    /// <summary>
    /// A basic example which simply echos all
    /// public messages sent to a channel.
    /// It is designed to demonstrate how to connect to an IRC
    /// server and how to register event handlers.
    /// </summary>
    public class ComBot
    {
        static private int announcementHue = Util.AnnouncementHue;
        static private bool botEnabled = false;
        static private string chatChannel = "#Comraich";
        static private string staffChannel = "#Comraich_Staff";

        public static ComBot Instance;

        private Dictionary<string, string> _queuedCommands = new Dictionary<string, string>();

        /// <summary>
        /// The connection object is the focal point of the library.
        /// It used to retrieve references to the various library components.
        /// </summary>
        public Connection connection;
        private bool isOp;
        private UserInfo user;
        private string message;

        /// <summary>
        /// Create a bot and register its handlers.
        /// </summary>
        public ComBot()
        {
            CreateConnection();
            RegisterHandlers();
        }

        private void CreateConnection()
        {
            //The hostname of the IRC server
            string server = "irc.darkmyst.org";

            //The bot's nick on IRC
            string nick = "ComBot";

            //Fire up the Ident server for those IRC networks
            //silly enough to use it.
            Identd.Start( nick );

            //A ConnectionArgs contains all the info we need to establish
            //our connection with the IRC server and register our bot.
            //This line uses the simplfied contructor and the default values.
            //With this constructor the Nick, Real Name, and User name are
            //all set to the same value. It will use the default port of 6667 and no server
            //password.
            ConnectionArgs cargs = new ConnectionArgs( nick, server );

            //When creating a Connection two additional protocols may be
            //enabled: CTCP and DCC. In this example we will disable them
            //both.
            connection = new Connection( cargs, false, false );

            //NOTE
            //We could have created multiple Connections to different IRC servers
            //and each would process messages simultaneously and independently.
            //There is no fixed limit on how many Connection can be opened at one time but
            //it is important to realize that each runs in its own Thread. Also,  separate event 
            //handlers are required for each connection, i.e. the
            //same OnRegistered() handler cannot be used for different connection
            //instances.
        }

        void EventSink_Login(LoginEventArgs e)
        {
            if (e.Mobile != null)
            {
                if (e.Mobile.AccessLevel == AccessLevel.Player)
                    connection.Sender.PublicMessage(chatChannel, String.Format("5{0} has logged in!", e.Mobile.RawName));
                else {
                    connection.Sender.PublicMessage(staffChannel, String.Format("5{0} has logged in!", e.Mobile.RawName));
                }
            }
        }

        void Listener_OnWhois( WhoisInfo whoisInfo )
        {

            if ( whoisInfo == null || whoisInfo.GetChannels() == null || whoisInfo.GetChannels().Length == 0 )
                return;

            bool staff = false;

            foreach ( string channel in whoisInfo.GetChannels() ) {
                if ( channel.ToLower() == "@" + staffChannel.ToLower() || channel.ToLower() == staffChannel.ToLower() || channel.ToLower() == "+" + staffChannel.ToLower() ) {
                    staff = true;
                }

                else if ( channel.ToLower() == "@" + chatChannel.ToLower() ) {
                    staff = true;
                }
            }

            if ( staff )
                IRCCommands( user, message );

            IRCPublic( user, message );
        }

        private void IRCPublic( UserInfo User, string message )
        {
            
        }

        private void IRCCommands( UserInfo User, string message )
        {
            if ( message.ToLower().StartsWith( "!bcast " ) ) {
                string bcast = Regex.Replace( message, "!bcast ", "", RegexOptions.IgnoreCase );

                CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, String.Format( "Staff message from {0}:", User.Nick ) );
                CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, bcast );
                connection.Sender.PublicMessage( chatChannel, String.Format( "In game broadcast from {0}: 5{1}", User.Nick, bcast ) );
                return;
            }

            if ( message.ToLower().StartsWith( "!bcastname " ) ) {
                string bcast = Regex.Replace( message, "!bcastname ", "", RegexOptions.IgnoreCase );
                string[] splitbcast = bcast.Split( ' ' );
                bcast = bcast.Replace( splitbcast[0], "" );
                CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, String.Format( "Staff message from {0}:", splitbcast[0] ) );
                CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, bcast );
                connection.Sender.PublicMessage( "#Comraich", String.Format( "In game broadcast from {0}:5{1}", splitbcast[0], bcast ) );
                return;
            }

            if ( message.ToLower().StartsWith( "!bcastnoname " ) ) {
                string bcast = Regex.Replace( message, "!bcastnoname ", "", RegexOptions.IgnoreCase );
                CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, "Staff message:" );
                CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, bcast );
                connection.Sender.PublicMessage( chatChannel, String.Format( "In game broadcast: 5{0}", bcast ) );
                return;
            }

            if ( message.ToLower().StartsWith( "!chanserv " ) ) {
                string command = Regex.Replace( message, "!chanserv ", "", RegexOptions.IgnoreCase );
                connection.Sender.PrivateMessage( "chanserv", String.Format( "{0}", command ) );
                return;
            }

            if ( message.ToLower().StartsWith( "!pages" ) ) {
                int pages = PageQueue.List.Count;
                connection.Sender.PublicMessage( staffChannel, String.Format( "5There are {0} pages in the queue.", pages ) );
                return;
            }

            /*if ( message.ToLower().StartsWith( "!createaccount" ) ) {
                string email = Regex.Replace( message, "!createaccount ", "", RegexOptions.IgnoreCase );

                if ( email.Length < 1 || !email.Contains( "@" ) ) {
                    connection.Sender.PublicMessage( staffChannel, String.Format( "5Not a valid email address." ) );
                    return;
                }

                MailAddress address = new MailAddress( email );

                string acctName = address.User.ToLower();
                if ( acctName.Length > 12 )
                    acctName = address.User.ToLower().Substring( 0, 12 );

                IAccount account = Accounts.GetAccount( acctName );

                if ( account == null ) {
                    connection.Sender.PublicMessage( staffChannel, String.Format( "5Creating account for email {0}.", email ) );
                    Mobile dummy = new Slime();
                    dummy.Name = "ComBot Dummy";
                    Server.CustomCommands.MakeAccount( email, dummy );
                    dummy.Delete();

                    account = Accounts.GetAccount( acctName );

                    if ( account != null )
                        connection.Sender.PublicMessage( staffChannel, String.Format( "5Account created.", email ) );
                    else
                        connection.Sender.PublicMessage( staffChannel, String.Format( "5Could not create account. Invalid email or already existing account.", email ) );


                }
                else
                    connection.Sender.PublicMessage( staffChannel, String.Format( "5Account name {0} already exists.", acctName ) );
                    

                return;
            }*/
        }


        private static void OnCommand_ComBot( CommandEventArgs e )
        {
            if ( !Instance.connection.Connected ) {
                Instance = null;
                e.Mobile.SendAsciiMessage( "ComBot is disconnected. Killing the thread." );
            }
            else if ( Instance != null ) {
                Instance.connection.Disconnect( "I was told to leave." );
                Instance = null;
            }
            Initialize();
            e.Mobile.SendAsciiMessage( "ComBot is being initalized." );
            
        }

        public static void Initialize()
        {
            if ( botEnabled ) {
                Instance = new ComBot();
                Instance.Start();
            }

            CommandSystem.Register( "Combot", AccessLevel.GameMaster, new CommandEventHandler( OnCommand_ComBot ) );
        }

        void EventSink_CharacterCreated( CharacterCreatedEventArgs e )
        {
            if ( e.Mobile != null ) {
                if ( e.Mobile.AccessLevel == AccessLevel.Player ) {
                    connection.Sender.PublicMessage( chatChannel, String.Format( "5{0} has just arrived in Comraich!", e.Mobile.RawName ) );
                    CommandHandlers.BroadcastMessage( AccessLevel.Player, announcementHue, String.Format( "{0} has just arrived in Comraich!", e.Mobile.RawName ) );
                }
            }
        }

        public void OnDisconnected()
        {
            //If this disconnection was involutary then you should have received an error
            //message ( from OnError() ) before this was called.
            Console.WriteLine( "Connection to the server has been closed." );
        }

        public void OnError( ReplyCode code, string message )
        {
            //All anticipated errors have a numeric code. The custom Thresher ones start at 1000 and
            //can be found in the ErrorCodes class. All the others are determined by the IRC spec
            //and can be found in RFC2812Codes.
            if ( (int)code == 439 )
                return;

            if ( code == ReplyCode.ConnectionFailed ) {
                Instance.connection.Disconnect( "Connection Failed" );
                Instance = null;
            }

            Console.WriteLine( "An error of type " + code + " due to " + message + " has occurred." );
        }

        public void OnPublic( UserInfo User, string channel, string Message )
        {
            if ( !Message.StartsWith( "!" ) )
                return;

            user = User;
            message = Message;

            connection.Sender.Whois( user.Nick );
        }

        public void OnPrivate( UserInfo User, string Message )
        {
            if ( !Message.StartsWith( "!" ) )
                return;

            user = User;
            message = Message;
            connection.Sender.Whois( user.Nick );
        }

        public void OnRegistered()
        {
            //We have to catch errors in our delegates because Thresher purposefully
            //does not handle them for us. Exceptions will cause the library to exit if they are not
            //caught.
            try {
                //Don't need this anymore in this example but this can be left running
                //if you want.
                Identd.Stop();

                connection.Sender.PrivateMessage( "nickserv", String.Format( "identify bluebird" ) );

                //The connection is ready so lets join a channel.
                //We can join any number of channels simultaneously but
                //one will do for now.
                //All commands are sent to IRC using the Sender object
                //from the Connection.
                connection.Sender.Join( chatChannel );
                connection.Sender.Join( staffChannel );

                EventSink.Login += new LoginEventHandler( EventSink_Login );
                EventSink.CharacterCreated += new CharacterCreatedEventHandler( EventSink_CharacterCreated );
            }
            catch ( Exception e ) {
                Console.WriteLine( "Error in OnRegistered(): " + e );
            }
        }

        public void PublicMessage( string channel, string message )
        {
            connection.Sender.PublicMessage( channel, message );
        }

        public void RegisterHandlers()
        {
            //OnRegister tells us that we have successfully established a connection with
            //the server. Once this is established we can join channels, check for people
            //online, or whatever.
            connection.Listener.OnRegistered += new RegisteredEventHandler( OnRegistered );

            //Listen for any messages sent to the channel
            connection.Listener.OnPublic += new PublicMessageEventHandler( OnPublic );

            //Listen for bot commands sent as private messages
            connection.Listener.OnPrivate += new PrivateMessageEventHandler( OnPrivate );

            //Listen for notification that an error has ocurred 
            connection.Listener.OnError += new ErrorMessageEventHandler( OnError );

            //Listen for notification that we are no longer connected.
            connection.Listener.OnDisconnected += new DisconnectedEventHandler( OnDisconnected );

            connection.Listener.OnWhois += new WhoisEventHandler( Listener_OnWhois );
        }

        public void Start()
        {
            //Notice that by having the actual connect call here 
            //the constructor can add the necessary listeners before 
            //the connection process begins. If listeners are added
            //after connecting they may miss certain events. the OnRegistered()
            //event will certainly be missed.

            try {
                //Calling Connect() will cause the Connection object to open a 
                //socket to the IRC server and to spawn its own thread. In this
                //separate thread it will listen for messages and send them to the 
                //Listener for processing.
                connection.Connect();

                Console.WriteLine( "Combot connected." );
                //The main thread ends here but the Connection's thread is still alive.
                //We are now in a passive mode waiting for events to arrive.
            }
            catch ( Exception e ) {
                Console.WriteLine( "Error during connection process." );
                Console.WriteLine( e );
                Identd.Stop();
            }
        }
    }
}