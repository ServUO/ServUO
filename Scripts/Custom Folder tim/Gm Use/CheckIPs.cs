using System.Collections.Generic;
using System.Net;
using Server.Network;

namespace Server.Commands
{
    #region

    

    #endregion

    public class CheckIPs
    {
        public static void Initialize()
        {
            CommandSystem.Register("CheckIPs", AccessLevel.GameMaster, CheckIPs_OnCommand);
        }

        [Usage("CheckIPs")]
        [Description("Shows a list of all multi clients online.")]
        private static void CheckIPs_OnCommand(CommandEventArgs e)
        {
            Dictionary<IPAddress, List<Mobile>> ipDictionary = new Dictionary<IPAddress, List<Mobile>>();

            List<NetState> states = NetState.Instances;
            for (int i = 0; i < states.Count; ++i)
            {
                Mobile m = states[i].Mobile;

                if (m == null)
                    continue;

                if (m.AccessLevel > AccessLevel.Player)
                    continue;

                IPAddress ipAddress = states[i].Address;

                List<Mobile> mobileList;
                if (ipDictionary.TryGetValue(ipAddress, out mobileList))
                    mobileList.Add(m);
                else
                {
                    mobileList = new List<Mobile>();
                    mobileList.Add(m);
                    ipDictionary.Add(ipAddress, mobileList);
                }
            }

            foreach (KeyValuePair<IPAddress, List<Mobile>> keyValuePair in ipDictionary)
            {
                List<Mobile> mobileList = keyValuePair.Value;

                if (mobileList.Count > 1)
                {
                    string multiEntry = string.Format("[{0}] ", keyValuePair.Key);
                    multiEntry += mobileList[0].Name;

                    for (int i = 1; i < mobileList.Count; i++)
                        multiEntry += string.Format(" and {0}", mobileList[i].Name);

                    e.Mobile.SendAsciiMessage(multiEntry);
                }
            }
        }
    }
}