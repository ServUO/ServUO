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
            m_From = from;
            m_Book = book;
            m_Object = obj;
            m_Price = price;
            m_Page = page;

            AddPage(0);

            AddBackground(100, 10, 300, 150, 5054);

            AddHtmlLocalized(125, 20, 250, 24, 1019070, false, false); // You have agreed to purchase:
            AddHtmlLocalized(125, 45, 250, 24, 1045151, false, false); // a bulk order deed

            AddHtmlLocalized(125, 70, 250, 24, 1019071, false, false); // for the amount of:
            AddLabel(125, 95, 0, price.ToString());

            AddButton(250, 130, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(282, 130, 100, 24, 1011012, false, false); // CANCEL

            AddButton(120, 130, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(152, 130, 100, 24, 1011036, false, false); // OKAY
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                PlayerVendor pv = m_Book.RootParent as PlayerVendor;

                if (m_Book.Entries.Contains(m_Object) && pv != null)
                {
                    int price = 0;

                    VendorItem vi = pv.GetVendorItem(m_Book);

                    if (vi != null && !vi.IsForSale)
                    {
                        if (m_Object is BOBLargeEntry)
                            price = ((BOBLargeEntry)m_Object).Price;
                        else if (m_Object is BOBSmallEntry)
                            price = ((BOBSmallEntry)m_Object).Price;
                    }

                    if (price != m_Price)
                    {
                        pv.SayTo(m_From, "The price has been been changed. If you like, you may offer to purchase the item again.");
                    }
                    else if (price == 0)
                    {
                        pv.SayTo(m_From, 1062382); // The deed selected is not available.
                    }
                    else
                    {
                        Item item = null;

                        if (m_Object is BOBLargeEntry)
                            item = ((BOBLargeEntry)m_Object).Reconstruct();
                        else if (m_Object is BOBSmallEntry)
                            item = ((BOBSmallEntry)m_Object).Reconstruct();

                        if (item == null)
                        {
                            m_From.SendMessage("Internal error. The bulk order deed could not be reconstructed.");
                        }
                        else
                        {
                            pv.Say(m_From.Name);

                            Container pack = m_From.Backpack;

                            if ((pack == null) || ((pack != null) && (!pack.CheckHold(m_From, item, true, true, 0, item.PileWeight + item.TotalWeight))))
                            {
                                pv.SayTo(m_From, 503204); // You do not have room in your backpack for this
                                m_From.SendGump(new BOBGump(m_From, m_Book, m_Page, null));
                            }
                            else
                            {
                                if ((pack != null && pack.ConsumeTotal(typeof(Gold), price)) || Banker.Withdraw(m_From, price))
                                {
                                    m_Book.Entries.Remove(m_Object);
                                    m_Book.InvalidateProperties();
                                    pv.HoldGold += price;
                                    m_From.AddToBackpack(item);
                                    m_From.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.

                                    if (m_Book.Entries.Count / 5 < m_Book.ItemCount)
                                    {
                                        m_Book.ItemCount--;
                                        m_Book.InvalidateItems();
                                    }

                                    if (m_Book.Entries.Count > 0)
                                        m_From.SendGump(new BOBGump(m_From, m_Book, m_Page, null));
                                    else
                                        m_From.SendLocalizedMessage(1062381); // The book is empty.
                                }
                                else
                                {
                                    pv.SayTo(m_From, 503205); // You cannot afford this item.
                                    item.Delete();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (pv == null)
                        m_From.SendLocalizedMessage(1062382); // The deed selected is not available.
                    else
                        pv.SayTo(m_From, 1062382); // The deed selected is not available.
                }
            }
            else
            {
                m_From.SendLocalizedMessage(503207); // Cancelled purchase.
            }
        }
    }
}