/*Login/logout broadcasting
 * Author: mordero
 * Email: mordero@gmail.com
 * Description: Will broadcast to current players when someone else has logged in/out. If the person who has logged in/out is above a certain access level, just broadcasts to the staff.
 * Install: Just drag it into your custom script folder.
 */
using System;
using System.Text;
using Server;
using Server.Commands;

namespace mordero.Custom
{
    class Broadcast
    {
        //{0} is the name of the player
        private readonly static string m_LoginMessage = "{0} has entered the world ";//logging in
        private readonly static string m_LogoutMessage = "{0} has left the world";//logging out
        private readonly static int m_LoginHue = 1153;//logging in hue
        private readonly static int m_LogoutHue = 1153;//logging out hue
        //maximum access level to announce
        private static AccessLevel m_AnnounceLevel = AccessLevel.Player;
        /// <summary>
        /// Subscribes to the login and out event
        /// </summary>
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);
            EventSink.Logout += new LogoutEventHandler(EventSink_Logout);
        }
        /// <summary>
        /// On player logout, broadcast a message.
        /// </summary>
        public static void EventSink_Logout(LogoutEventArgs e)
        {
            if (e.Mobile.Player)
            {
                if (e.Mobile.AccessLevel <= m_AnnounceLevel)
                    CommandHandlers.BroadcastMessage(AccessLevel.Player, m_LogoutHue, String.Format(m_LogoutMessage, e.Mobile.Name));
                else //broadcast any other level to the staff
                    CommandHandlers.BroadcastMessage(AccessLevel.Counselor, m_LogoutHue, String.Format(m_LogoutMessage, e.Mobile.Name));
            }
        }
        /// <summary>
        /// On player login, broadcast a message.
        /// </summary>
        public static void EventSink_Login(LoginEventArgs e)
        {
            if (e.Mobile.Player)
            {
                if (e.Mobile.AccessLevel <= m_AnnounceLevel)
                    CommandHandlers.BroadcastMessage(AccessLevel.Player, m_LoginHue, String.Format(m_LoginMessage, e.Mobile.Name));
                else //broadcast any other level to the staff
                    CommandHandlers.BroadcastMessage(AccessLevel.Counselor, m_LoginHue, String.Format(m_LoginMessage, e.Mobile.Name));
            }
        }
    }
}