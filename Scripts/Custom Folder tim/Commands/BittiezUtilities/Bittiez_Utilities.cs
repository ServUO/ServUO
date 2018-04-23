using System;
using System.Text;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;

namespace Bittiez
{
    public class Tools
    {
        #region Tools Stuff
        public static int Version = 3;
        public static void Initialize()
        {
            ConsoleWrite(ConsoleColor.Blue, "Bittiez Utilities Version " + Format_Version(Version));
        }
        public static string Format_Version(int version)
        {
            string v = version.ToString();
            if (v.Length >= 3)
                return string.Format("{0}.{1}.{2}", v.Substring(0, v.Length - 2), v.Substring(v.Length - 2, 1), v.Substring(v.Length - 1));

            if (v.Length >= 2)
                return string.Format("{0}.{1}.{2}", 0, v.Substring(v.Length - 2, 1), v.Substring(v.Length - 1, 1));

            if (v.Length >= 1)
                return string.Format("{0}.{1}.{2}", 0, 0, v.Substring(0, 1));


            return v;
        }
        #endregion

        #region Find_Items_At_Location
        /// <summary>
        /// Returns a List of items at a specified location
        /// </summary>
        /// <param name="LOCATION">Point3D location(Mobile.Location)</param>
        /// <param name="MAP">Map(Mobile.Map)</param>
        /// <returns></returns>
        public static List<Item> Find_Items_At_Location(Point3D LOCATION, Map MAP)
        {
            Sector sector = MAP.GetSector(LOCATION);
            return sector.Items;
        }
        #endregion

        #region Start_Timer_Delayed_Call
        /// <summary>
        /// This will start a timer and call a void function(CallBack) after the time is up.
        /// Start_Timer_Delayed_Call(TimeSpan.FromSeconds(5), MyMethodHere);
        /// </summary>
        /// <param name="Delay">TimeSpan delay(TimeSpan.FromSeconds(5))</param>
        /// <param name="CallBack">This is the method to call, must be a void method(public void Example(){//You code here})</param>
        public static void Start_Timer_Delayed_Call(TimeSpan Delay, TimerCallback CallBack)
        {
            Server.Timer.DelayCall(Delay, new TimerCallback(CallBack));
        }
        #endregion

        #region List_Connected_Players
        /// <summary>
        /// This will List all connect players.
        /// </summary>
        /// <returns>This returns a List of all connected players.</returns>
        public static List<Mobile> List_Connected_Players()
        {
            List<Mobile> Players = new List<Mobile>();

            foreach (NetState ns in NetState.Instances)
                if (ns.Mobile != null)
                {
                    Players.Add(ns.Mobile);
                }
            return Players;
        }
        #endregion

        #region ConsoleWrite
        /// <summary>
        /// Writes a line to the console using a specific color
        /// </summary>
        /// <param name="Color">The color of the text(ConsoleColor.COLOR)</param>
        /// <param name="Text">The string you want to write</param>
        public static void ConsoleWrite(ConsoleColor Color, string Text)
        {
            ConsoleColor o = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine(Text);
            Console.ForegroundColor = o;
        }
        #endregion

        #region List_All_Skills
        /// <summary>
        /// This will return a List-SkillName- of all skills in the system.
        /// EX:
        /// List-SkillName- Skillz = Bittiez.Tools.List_All_Skills();
        /// </summary>
        /// <returns>List-SkillName- of all skills.</returns>
        public static List<SkillName> List_All_Skills()
        {
            List<SkillName> SN = new List<SkillName>();
            foreach (SkillName s in Enum.GetValues(typeof(SkillName)))
                SN.Add(s);
            return SN;
        }
        #endregion

        #region List_Items_In_Container
        /// <summary>
        /// This will return a list of all items in a container, if Search_Sub_Containers is true, it will recursevly search all containers recursevly.
        /// Ex:
        /// List-Item- Items = List_Items_In_Container((BaseContainer)Mobile.Backpack, true);
        /// </summary>
        /// <param name="Container">Container, such as Mobile.Backpack</param>
        /// <param name="Search_Sub_Containers">True/False to search through all containers or not</param>
        /// <returns>Returns an item List of all items inside a container.</returns>
        public static List<Item> List_Items_In_Container(BaseContainer Container, bool Search_Sub_Containers)
        {
            if (Container == null)
                return null;

            List<Item> items = Container.Items, item_b;
            List<Item> AllItems = new List<Item>();

            foreach (Item i in items)
            {
                if (Search_Sub_Containers)
                    if (i is BaseContainer)
                    {
                        item_b = List_Items_In_Container((BaseContainer)i, Search_Sub_Containers);
                        if (item_b != null)
                            foreach (Item i_sub in item_b)
                                if (i_sub != null)
                                    AllItems.Add(i_sub);
                    }
                if (i != null)
                    AllItems.Add(i);
            }

            return AllItems;
        }
        #endregion
    }
}
