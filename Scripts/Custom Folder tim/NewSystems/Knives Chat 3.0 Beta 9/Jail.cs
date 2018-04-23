/*
 * The line following this entry determines if your server combines Xanthos's Jail
 * features with the Chat filter.  To enable, remove the '//' in front of the
 * '#define Use_Jail'.  If there is ever a change which makes Xanthos's Jail no
 * longer work with Chat, readd the '//' to disable.
 */

//#define Use_Xanthos

using System;
using Server;

namespace Knives.Chat3
{
    public class ChatJail
    {
        public static void SendToJail(Mobile m)
        {
            #if (Use_Xanthos)
                Xanthos.JailSystem.Jail.JailThem((Server.Mobiles.PlayerMobile)m, Xanthos.JailSystem.Jail.JailOption.None);
            #endif
        }
    }
}