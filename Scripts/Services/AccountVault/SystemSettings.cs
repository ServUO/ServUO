using System;
using System.Text;
using System.Globalization;

using Server.Mobiles;
using Server.Engines.UOStore;

namespace Server.AccountVault
{
    internal static class SystemSettings
    {
        public static bool Enabled = true;
        public static readonly bool UseTokens = true;

        internal static readonly int RentGoldValue = 50000;
        internal static readonly int MaxGoldBalance = 1000000;

        internal static readonly int RentTokenValue = 1;
        internal static readonly int MaxTokenBalance = 3;

        internal static readonly TimeSpan RentTimeSpan = TimeSpan.FromDays(30);
        internal static readonly TimeSpan PastDuePeriod = TimeSpan.FromDays(7);
        internal static readonly TimeSpan ClaimPeriod = TimeSpan.FromHours(3);

        internal static int AuctionDuration = 480;

        public static int MaxBalance => UseTokens ? MaxTokenBalance : MaxGoldBalance;

        public static int RentValue => UseTokens ? RentTokenValue : RentGoldValue;

        internal static string Humanize(this TimeSpan timeSpan)
        {
            StringBuilder builder = new StringBuilder();
            if (timeSpan.Days > 0)
                builder.AppendFormat("{0:%d} day(s)", timeSpan);
            if (timeSpan.Days > 0 && (timeSpan.Hours > 0 || timeSpan.Minutes > 0 || timeSpan.Seconds > 0))
                builder.Append(", ");

            if (timeSpan.Hours > 0)
                builder.AppendFormat("{0:%h} hour(s)", timeSpan);
            if (timeSpan.Hours > 0 && (timeSpan.Minutes > 0 || timeSpan.Seconds > 0))
                builder.Append(", ");

            if (timeSpan.Minutes > 0)
                builder.AppendFormat("{0:%m} minute(s)", timeSpan);
            if (timeSpan.Minutes > 0 && timeSpan.Seconds > 0)
                builder.Append(", ");

            if (timeSpan.Seconds > 0)
                builder.AppendFormat("{0:%s} second(s)", timeSpan);

            return builder.ToString();
        }

        /// <summary>
        /// Based from clilocs 1158063 and 1158064
        /// </summary>
        /// <returns></returns>
        public static TextDefinition RentMessage()
        {
            if (UseTokens)
            {
                return string.Format("Do you wish to rent a storage vault for {0} vault token[s] a month? You will need to keep your vault token balance up to date or risk losing your vault contents.", RentTokenValue);
            }
            else
            {
                return string.Format("Do you wish to rent a storage vault for {0} gold a month? You will need to keep your vault token balance up to date or risk losing your vault contents.", RentGoldValue.ToString("N0", CultureInfo.GetCultureInfo("en-US")));
            }
        }

        public static bool HasBalance(PlayerMobile pm, int amount = -1)
        {
            if (UseTokens)
            {
                if (amount == -1)
                {
                    amount = RentTokenValue;
                }

                var storeProfile = UltimaStore.GetProfile(pm, false);

                return storeProfile != null && storeProfile.VaultTokens >= amount;
            }
            else
            {
                if (amount == -1)
                {
                    amount = RentGoldValue;
                }

                return pm.AccountGold.TotalCurrency >= amount;
            }
        }

        public static void WithdrawBalance(PlayerMobile pm, int amount = -1)
        {
            if (amount == -1)
            {
                amount = RentValue;
            }

            if (UseTokens)
            {
                var storeProfile = UltimaStore.GetProfile(pm, false);

                if (storeProfile != null)
                {
                    storeProfile.VaultTokens -= amount;
                }
            }
            else
            {
                Banker.Withdraw(pm, amount, true);
            }
        }

        public static string[] VaultRegions => _VaultRegions;

        private static string[] _VaultRegions = new[]
        {
            "Royal City",
            "Papua",
            "Delucia",
            "Britain",
            "Magincia",
            "Haven Island",
            "Buccaneer's Den",
            "Yew",
            "Moonglow",
            "Trinsic",
            "Minoc",
            "Wind"
        };

        public static string RegionLabel(string region)
        {
            switch (region)
            {
                case "Royal City": return "#1112571";
                case "Papua": return "#1011057";
                case "Delucia": return "#1011058";
                case "Britain": return "#1011028";
                case "Magincia": return "#1011345";
                case "Haven Island": return "#1078342";
                case "Buccaneer's Den": return "#1075702";
                case "Yew": return "#1011032";
                case "Moonglow": return "#1011344";
                case "Trinsic": return "#1011029";
                case "Minoc": return "#1011031";
                case "Wind": return "#1078263";
            }

            return region;
        }
    }
}
