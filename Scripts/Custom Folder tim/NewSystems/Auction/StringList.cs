namespace Server.Auction
{
    class StringList
    {
        public const string

            //board.cs
            BoardName = "Auction Board",
            Dead = "You can't use the auction board while dead.",
            TooFar = "You are too far away",
            TipName = "{0}'s Auction Board", //{0} = region name
            TipCount = "There are {0} items on auction", // {0} = item count
            EndOwnerWarn = "Your item '{0}' was sold to '{1}' by '{2}' gold pieces. A check was deposited into your bank",
            EndBuyerWarn = "Your bid to '{0}' was the highest. '{1}' gold pieces was withdrawn from your bank and the item was move to your bank",
            EndOwnerNoBids = "Your item '{0}' did not receive a valid offer and was moved to your bank",
            NoGoldWarn = "Your bid to '{0}' is due lack of gold in your bank.",

            //AuctionGump.cs // addauctiongump // ViewItemgump
            GumpTitle = "{0}'s Auction Board - {1} {2}",
            OwnerHeader = "OWNER",
            ItemHeader = "ITEM",
            PriceHeader = "PRICE",
            LastBidHeader = "LAST BID",
            EndHeader = "END",
            DonationTag = "DONATION",
            DescHeader = "DESC",
            DaysHeader = "DAYS",
            DashTag = "--",
            EndTodayTag = "Today {0}", //0 = datetime
            EmptyTag = "-- EMPTY --",
            Page = "Page {0} of {1}",
            AuctionItem = "Auction Item",
            SelectItem = "What you wish to auction?",
            BadSelection = "This is not a valid auctionale item",
            MustBeOnPack = "The item must be in your pack to be auctioned.",
            InvalidName = "You have selected an invalid name",
            InvalidDesc = "You have selected an invalid description",
            InvalidPrice = "You have selected an invalid price",
            InvalidDuration = "You hace selected an invalid number of days",

            StartAuctWarnMessage = "You are about to auction your '{0}' for '{1}'. You will no be able to remove your item from auction",
            StartAuctGold = "gold coins",
            StartAuctDonation = "donation",

            CantBidOwn = "You can't bid in your own item.",
            DonationMoved = "The donation was moved to your pack.",
            BidRegist = "Your bid was registred, you will be noticed if you win the auction",
            AlreadyAuctioned = "The item was already auctioned.",
            HigherValue = "You need to enter a value higher than the last bid.",
            InvalidValue = "You have selected an invalid bid value";
    }
}
