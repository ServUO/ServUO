using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class RecipeScrollBuyGump : Gump
    {
        private Mobile m_From;
        private RecipeBook m_Book;
        private RecipeScrollDefinition m_Recipe;
        private int m_Price;

        public RecipeScrollBuyGump(Mobile from, RecipeBook book, RecipeScrollDefinition recipe, int price)
            : base(100, 200)
        {
            m_From = from;
            m_Book = book;
            m_Recipe = recipe;
            m_Price = price;

            AddPage(0);

            AddBackground(100, 10, 300, 150, 5054);

            AddHtmlLocalized(125, 20, 250, 24, 1019070, false, false); // You have agreed to purchase:
            AddHtmlLocalized(125, 45, 250, 24, 1074560, false, false); // recipe scroll

            AddHtmlLocalized(125, 70, 250, 24, 1019071, false, false); // for the amount of:
            AddLabel(125, 95, 0, price.ToString());

            AddButton(250, 130, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(282, 130, 100, 24, 1011012, false, false); // CANCEL

            AddButton(120, 130, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(152, 130, 100, 24, 1011036, false, false); // OKAY
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                PlayerVendor pv = m_Book.RootParent as PlayerVendor;

                if (pv != null)
                {
                    int price = 0;

                    VendorItem vi = pv.GetVendorItem(m_Book);

                    if (vi != null && !vi.IsForSale)
                    {
                        price = m_Recipe.Price;
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
                        Item item = new RecipeScroll(m_Recipe.RecipeID);

                        pv.Say(m_From.Name);

                        Container pack = m_From.Backpack;

                        if ((pack != null && pack.ConsumeTotal(typeof(Gold), price)) || Banker.Withdraw(m_From, price))
                        {
                            m_Book.Recipes.ForEach(x =>
                            {
                                if (x.RecipeID == m_Recipe.RecipeID)
                                    x.Amount = x.Amount - 1;
                            });

                            m_Book.InvalidateProperties();

                            pv.HoldGold += price;

                            if (m_From.AddToBackpack(item))
                                m_From.SendLocalizedMessage(1158820); // The recipe has been placed in your backpack.
                            else
                                pv.SayTo(m_From, 503204); // You do not have room in your backpack for this.

                            m_From.SendGump(new RecipeBookGump(m_From, m_Book));
                        }
                        else
                        {
                            pv.SayTo(m_From, 503205); // You cannot afford this item.
                            item.Delete();
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
