using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    public class BODBuyGump : Gump
    {
        private readonly PlayerMobile m_From;
        private readonly BulkOrderBook m_Book;
        private readonly object m_Object;
        private readonly int m_Price;
        private readonly int m_Page;
        public BODBuyGump(PlayerMobile from, BulkOrderBook book, object obj, int page, int price)
            : base(100, 200)
        {
            this.m_From = from;
            this.m_Book = book;
            this.m_Object = obj;
            this.m_Price = price;
            this.m_Page = page;

            this.AddPage(0);

            this.AddBackground(100, 10, 300, 150, 5054);

            this.AddHtmlLocalized(125, 20, 250, 24, 1019070, false, false); // You have agreed to purchase:
            this.AddHtmlLocalized(125, 45, 250, 24, 1045151, false, false); // a bulk order deed

            this.AddHtmlLocalized(125, 70, 250, 24, 1019071, false, false); // for the amount of:
            this.AddLabel(125, 95, 0, price.ToString());

            this.AddButton(250, 130, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(282, 130, 100, 24, 1011012, false, false); // CANCEL

            this.AddButton(120, 130, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(152, 130, 100, 24, 1011036, false, false); // OKAY
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                PlayerVendor pv = this.m_Book.RootParent as PlayerVendor;

                if (this.m_Book.Entries.Contains(this.m_Object) && pv != null)
                {
                    int price = 0;

                    VendorItem vi = pv.GetVendorItem(this.m_Book);

                    if (vi != null && !vi.IsForSale)
                    {
                        if (this.m_Object is BOBLargeEntry)
                            price = ((BOBLargeEntry)this.m_Object).Price;
                        else if (this.m_Object is BOBSmallEntry)
                            price = ((BOBSmallEntry)this.m_Object).Price;
                    }

                    if (price != this.m_Price)
                    {
                        pv.SayTo(this.m_From, "The price has been been changed. If you like, you may offer to purchase the item again.");
                    }
                    else if (price == 0)
                    {
                        pv.SayTo(this.m_From, 1062382); // The deed selected is not available.
                    }
                    else
                    {
                        Item item = null;

                        if (this.m_Object is BOBLargeEntry)
                            item = ((BOBLargeEntry)this.m_Object).Reconstruct();
                        else if (this.m_Object is BOBSmallEntry)
                            item = ((BOBSmallEntry)this.m_Object).Reconstruct();

                        if (item == null)
                        {
                            this.m_From.SendMessage("Internal error. The bulk order deed could not be reconstructed.");
                        }
                        else
                        {
                            pv.Say(this.m_From.Name);

                            Container pack = this.m_From.Backpack;

                            if ((pack == null) || ((pack != null) && (!pack.CheckHold(this.m_From, item, true, true, 0, item.PileWeight + item.TotalWeight))))
                            {
                                pv.SayTo(this.m_From, 503204); // You do not have room in your backpack for this
                                this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book, this.m_Page, null));
                            }
                            else
                            {
                                if ((pack != null && pack.ConsumeTotal(typeof(Gold), price)) || Banker.Withdraw(this.m_From, price))
                                {
                                    this.m_Book.Entries.Remove(this.m_Object);
                                    this.m_Book.InvalidateProperties();
                                    pv.HoldGold += price;
                                    this.m_From.AddToBackpack(item);
                                    this.m_From.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.
									
                                    if (this.m_Book.Entries.Count / 5 < this.m_Book.ItemCount)
                                    {
                                        this.m_Book.ItemCount--;
                                        this.m_Book.InvalidateItems();
                                    }

                                    if (this.m_Book.Entries.Count > 0)
                                        this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book, this.m_Page, null));
                                    else
                                        this.m_From.SendLocalizedMessage(1062381); // The book is empty.
                                }
                                else
                                {
                                    pv.SayTo(this.m_From, 503205); // You cannot afford this item.
                                    item.Delete();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (pv == null)
                        this.m_From.SendLocalizedMessage(1062382); // The deed selected is not available.
                    else
                        pv.SayTo(this.m_From, 1062382); // The deed selected is not available.
                }
            }
            else
            {
                this.m_From.SendLocalizedMessage(503207); // Cancelled purchase.
            }
        }
    }
}