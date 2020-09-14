using System;
using System.IO;

namespace Server.Engines.Auction
{
    public static class VaultLogging
    {
        public static string LogDirectory { get; set; }

        static VaultLogging()
        {
            var directory = Path.Combine(Core.BaseDirectory, "Logs/Auctions");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            LogDirectory = directory;
        }

        public static void LogAuctionEvent(Auction auction, string text)
        {
            string path = Path.Combine(LogDirectory, string.Format("{0}-{1:X6}-{2}.log", auction.AuctionItem.GetType().Name.ToString(), auction.AuctionItem.Serial.ToString(), GetTimeStamp(auction.StartTime)));

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(string.Format("{0}: {1}", DateTime.Now, text));
            }
        }

        public static void LogRefund(Auction auction, Mobile from, long refund)
        {
            LogAuctionEvent(auction, string.Format("Refund of {0} to {1}", refund, from));
        }

        public static void NewHighBid(Auction auction, Mobile topBid, Mobile oldBid, long amount)
        {
            if (oldBid != null)
            {
                LogAuctionEvent(auction, string.Format("{0} has outbid {1} with a high bid of {2}.", topBid, oldBid, amount));
            }
            else
            {
                LogAuctionEvent(auction, string.Format("{0} has placed a top bid of {1}", topBid, amount));
            }
        }

        public static void Buyout(Auction auction, Mobile buyout, long amount)
        {
            LogAuctionEvent(auction, string.Format("{0} payed {1} buyout to win the auction.", buyout, amount));
        }

        public static void WinAuction(Auction auction, Mobile winner, long bid, long reserveBid)
        {
            LogAuctionEvent(auction, string.Format("{0} has won the auction with a winning bid of {1} [{2} reserve refunded].", winner, bid, reserveBid - bid));
        }

        private static string GetTimeStamp(DateTime time)
        {
            return string.Format(" [{0}-{1}-{2}-{3}-{4}-{5}]",
                time.Day,
                time.Month,
                time.Year,
                time.Hour,
                time.Minute,
                time.Second);
        }
    }
}
