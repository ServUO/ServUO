using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands
{
    public class ConvertPlayers
    {
        public static void Initialize()
        {
            CommandSystem.Register("ConvertPlayers", AccessLevel.Administrator, new CommandEventHandler(Convert_OnCommand));
        }

        public static void Convert_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Converting all players to PlayerMobile.  You will be disconnected.  Please Restart the server after the world has finished saving.");
            List<Mobile> mobs = new List<Mobile>(World.Mobiles.Values);
            int count = 0;
            
            foreach (Mobile m in mobs)
            {
                if (m.Player && !(m is PlayerMobile))
                {
                    count++;
                    if (m.NetState != null)
                        m.NetState.Dispose();
                    
                    PlayerMobile pm = new PlayerMobile(m.Serial);
                    pm.DefaultMobileInit();
                    
                    List<Item> copy = new List<Item>(m.Items);
                    for (int i = 0; i < copy.Count; i++)
                        pm.AddItem(copy[i]);
                    
                    CopyProps(pm, m);
                    
                    for (int i = 0; i < m.Skills.Length; i++)
                    {
                        pm.Skills[i].Base = m.Skills[i].Base;
                        pm.Skills[i].SetLockNoRelay(m.Skills[i].Lock);
                    }
                    
                    World.Mobiles[m.Serial] = pm;
                }
            }
            
            if (count > 0)
            {
                NetState.ProcessDisposedQueue();
                World.Save();
            
                Console.WriteLine("{0} players have been converted to PlayerMobile. {1}.", count, Core.Service ? "The server is now restarting" : "Press any key to restart the server");
                
                if (!Core.Service)
                    Console.ReadKey(true);

                Core.Kill(true);
            }
            else
            {
                e.Mobile.SendMessage("Couldn't find any Players to convert.");
            }
        }

        private static void CopyProps(Mobile to, Mobile from)
        {
            Type type = typeof(Mobile);
            
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            for (int p = 0; p < props.Length; p++)
            {
                PropertyInfo prop = props[p];
                
                if (prop.CanRead && prop.CanWrite)
                {
                    try
                    {
                        prop.SetValue(to, prop.GetValue(from, null), null);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}