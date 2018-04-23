using System;
using Server;
using Server.Gumps;
using Server.Customs;

namespace Server.Auction
{
    class AddAuctionGump : BaseGridGump
    {
        Mobile Mob;
        AuctionContainer Cont;
        Item Item;
        public AddAuctionGump(Mobile from, AuctionContainer cont, Item i, string[] param)
            : base((640 - 320) / 2, (480 - 100) / 2)
        {
            Mob = from;
            Cont = cont;
            Item = i;
            string tname = i.GetType().Name;
            if (i.Stackable) tname = i.Amount + " " + tname;
            AddNewPage();
            AddEntryHtml(100, StringList.ItemHeader); AddEntryText(200, 0, param[0] ?? tname); AddNewLine();
            AddEntryHtml(100, StringList.DescHeader); AddEntryText(200, 1, param[1] ?? i.Name); AddNewLine();
            AddEntryHtml(100, StringList.PriceHeader); AddEntryText(200, 2, param[2] ?? "100"); AddNewLine();
            AddEntryHtml(100, StringList.DaysHeader); AddEntryText(200, 3, param[3] ?? "7"); AddNewLine();
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 1, ArrowRightWidth, ArrowRightHeight); AddEntryHtml(280, StringList.AuctionItem);
            FinishPage();
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                int price;
                string name;
                string desc;

                if ((String.IsNullOrWhiteSpace(info.GetTextEntry(0).Text)) || (String.IsNullOrEmpty(info.GetTextEntry(0).Text)) || (info.GetTextEntry(0).Text.Length < 3))
                {
                    sender.Mobile.SendMessage(StringList.InvalidName);
                    string[] param = { info.GetTextEntry(0).Text, info.GetTextEntry(1).Text, info.GetTextEntry(2).Text, info.GetTextEntry(3).Text };
                    sender.Mobile.SendGump(new AddAuctionGump(Mob, Cont, Item, param));
                    return;
                }

                if ((String.IsNullOrWhiteSpace(info.GetTextEntry(1).Text)) || (String.IsNullOrEmpty(info.GetTextEntry(1).Text)) || (info.GetTextEntry(1).Text.Length < 3))
                {
                    sender.Mobile.SendMessage(StringList.InvalidDesc);
                    string[] param = { info.GetTextEntry(0).Text, info.GetTextEntry(1).Text, info.GetTextEntry(2).Text, info.GetTextEntry(3).Text };
                    sender.Mobile.SendGump(new AddAuctionGump(Mob, Cont, Item, param));
                    return;
                }

                if ((!int.TryParse(info.GetTextEntry(2).Text, out price)) || (price < 0) || price >= int.MaxValue)
                {
                    sender.Mobile.SendMessage(StringList.InvalidPrice);
                    string[] param = { info.GetTextEntry(0).Text, info.GetTextEntry(1).Text, info.GetTextEntry(2).Text, info.GetTextEntry(3).Text };
                    sender.Mobile.SendGump(new AddAuctionGump(Mob, Cont, Item, param));
                    return;
                }
                int days = 0;
                if ((!int.TryParse(info.GetTextEntry(3).Text, out days)) || (days < 1) || days > 365)
                {
                    sender.Mobile.SendMessage(StringList.InvalidDuration);
                    string[] param = { info.GetTextEntry(0).Text, info.GetTextEntry(1).Text, info.GetTextEntry(2).Text, info.GetTextEntry(3).Text };
                    sender.Mobile.SendGump(new AddAuctionGump(Mob, Cont, Item, param));
                    return;
                }


                if (Item.ParentEntity != Mob.Backpack)
                {
                    Mob.SendMessage(StringList.MustBeOnPack);
                    string[] param = { info.GetTextEntry(0).Text, info.GetTextEntry(1).Text, info.GetTextEntry(2).Text, info.GetTextEntry(3).Text };
                    sender.Mobile.SendGump(new AddAuctionGump(Mob, Cont, Item, param));
                    return;
                }

                name = info.GetTextEntry(0).Text;
                desc = info.GetTextEntry(1).Text;

                WarningGump wg = new WarningGump(
                    1074862,
                    0xffffff,
                    String.Format(StringList.StartAuctWarnMessage, name, price > 0 ? price.ToString() + StringList.StartAuctGold : StringList.StartAuctDonation),
                    0xffffff,
                    340, 180,
                    new WarningGumpCallback(Accepted),
                    new ObjectHolder(name, desc, Mob, Item, Cont, days, price));
                sender.Mobile.SendGump(wg);
                return;
            }
            sender.Mobile.SendGump(new AuctionGump(Mob, Cont));
        }

        void Accepted(Mobile from, bool ok, object o)
        {
            ObjectHolder oh = (ObjectHolder)o;
            if (ok)
            {
                if (oh.item.ParentEntity != oh.mob.Backpack)
                {
                    oh.mob.SendMessage(StringList.MustBeOnPack);
                    string[] param = { oh.name, oh.desc, oh.price.ToString(), oh.days.ToString() };
                    oh.mob.SendGump(new AddAuctionGump(oh.mob, oh.cont, oh.item, param));
                    return;
                }
                DateTime enddate = DateTime.Now + TimeSpan.FromDays(oh.days);
                AuctionEntry ae = new AuctionEntry(oh.mob, oh.item, oh.name, oh.desc, enddate, oh.price);
                oh.cont.AuctionItems.Add(ae);
                oh.mob.SendGump(new AuctionGump(oh.mob, oh.cont));
                oh.cont.AddItem(oh.item);
                oh.item.Movable = false;
            }
            else
            {
                string[] param = { oh.name, oh.desc, oh.price.ToString(), oh.days.ToString() };
                oh.mob.SendGump(new AddAuctionGump(oh.mob, oh.cont, oh.item, param));
            }
        }

        private class ObjectHolder
        {
            public string name;
            public string desc;
            public Mobile mob;
            public Item item;
            public AuctionContainer cont;
            public int days;
            public int price;
            public ObjectHolder(string m_name, string m_desc, Mobile m_mob, Item m_item, AuctionContainer m_cont, int m_days, int m_price)
            {
                name = m_name;
                desc = m_desc;
                mob = m_mob;
                item = m_item;
                cont = m_cont;
                days = m_days;
                price = m_price;
            }
        }
    }
}