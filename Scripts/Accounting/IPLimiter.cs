using Server.Network;
using System.Collections.Generic;
using System.Net;

namespace Server.Misc
{
    public class IPLimiter
    {
        public static bool Enabled = true;
        public static bool SocketBlock = true;// true to block at connection, false to block at login request
        public static int MaxAddresses = 10;

        public static IPAddress[] Exemptions = new IPAddress[]	//For hosting services where there are cases where IPs can be proxied
        {
            IPAddress.Parse( "127.0.0.1" ),
        };

        public static bool IsExempt(IPAddress ip)
        {
            for (int i = 0; i < Exemptions.Length; i++)
            {
                if (ip.Equals(Exemptions[i]))
                    return true;
            }

            return false;
        }

        public static bool Verify(IPAddress ourAddress)
        {
            if (!Enabled || IsExempt(ourAddress))
                return true;

            List<NetState> netStates = NetState.Instances;

            int count = 0;

            for (int i = 0; i < netStates.Count; ++i)
            {
                NetState compState = netStates[i];

                if (ourAddress.Equals(compState.Address))
                {
                    ++count;

                    if (count >= MaxAddresses)
                        return false;
                }
            }

            return true;
        }
    }
}