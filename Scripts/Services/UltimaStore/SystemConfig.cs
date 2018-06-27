using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Points;

namespace Server.Engines.UOStore
{
    public enum CurrencyType
    {
        None,
        Sovereigns,
        Gold,
        PointsSystem,
        Custom
    }

    public static class Configuration
    {
        public static string CurrencyName = "Sovereigns";                  // What do you want to call your currency?
        public static string CurrencyInfoWebsite = "";                     // Website address for your currency information. Default is https://uo.com/ultima-store/
        public static double PointMultiplier = 1.0;                      // Multiplier for store item cost. The EA costs act as a 'base cost' where you can increase to  your choosing. 10.0 is the same as BaseCost * 10
        public static CurrencyType CurrencyType = CurrencyType.Sovereigns; // See enum CurrencyType. Gold - PointsSystem is autmoatically handled, where custom you will have to implement your self. See ReadMe.

        public static PointsType PointsSystemCurrency = PointsType.None;    // If PointsSystem is chosen, this is the PointsSystem enabled for store use. See PointsSystem.cs for all of the systems you can use!

        public static bool ShowCurrencyType = true;                         // This will show the currency type, uses 'CurrencyName' in the Ultima Store Gump.

        /// <summary>
        /// If you're using a custom currency, this is where you will put your accessor to 'get' the amount of that currency a player has.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double GetCustomCurrency(Mobile m)
        {
            // Comment out the next line, and put your points accessor here

            Utility.WriteConsoleColor(ConsoleColor.Red, "[Ultima Store]: No custom currency method has been implemented.");
            m.SendMessage(1174, "Custom Currency is not set up for this system. Contact a shard administrator.");

            return 0.0;
        }

        /// <summary>
        /// If using a custom currency, this is where you will 'deduct' that currency when the item is bought. This is important, so you're not giving out freebies!
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static void DeductCustomCurrecy(Mobile m, int amount)
        {
            // Comment out the next line, and put in your accessor to remove currency for the purchase
            Utility.WriteConsoleColor(ConsoleColor.Red, "[Ultima Store]: No custom currency deduction method has been implemented.");
        }
    }
}
