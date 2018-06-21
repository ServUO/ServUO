using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.UOStore
{
    public class StoreEntry
    {
        public Type ItemType { get; private set; }
        public TextDefinition[] Name { get; private set; }
        public int Tooltip { get; private set; }
        public int GumpID { get; private set; }
        public int ItemID { get; private set; }
        public int Hue { get; private set; }
        public double Price { get; private set; }
        public StoreCategory Category { get; private set; }
        public Func<Mobile, StoreEntry, Item> Constructor { get; private set; }

        public double Cost { get { return Price * Configuration.PointMultiplier; } }

        public StoreEntry(Type itemType, TextDefinition name, int tooltip, int itemID, int gumpID, int hue, double cost, StoreCategory cat, Func<Mobile, StoreEntry, Item> constructor = null)
            : this(itemType, new TextDefinition[] { name }, tooltip, itemID, gumpID, hue, cost, cat, constructor)
        {
        }

        public StoreEntry(Type itemType, TextDefinition[] name, int tooltip, int itemID, int gumpID, int hue, double cost, StoreCategory cat, Func<Mobile, StoreEntry, Item> constructor = null)
        {
            ItemType = itemType;
            Name = name;
            Tooltip = tooltip;
            ItemID = itemID;
            GumpID = gumpID;
            Hue = hue;
            Price = cost;
            Category = cat;
            Constructor = constructor;
        }

        public bool Construct(Mobile m, bool test = false)
        {
            Item item;

            if (Constructor != null)
            {
                item = Constructor(m, this);
            }
            else
            {
                item = Activator.CreateInstance(ItemType) as Item;
            }

            if (item != null)
            {
                if (item is IAccountRestricted)
                {
                    ((IAccountRestricted)item).Account = m.Account.Username;
                }

                if (m.Backpack == null || !m.Alive || !m.Backpack.TryDropItem(m, item, false))
                {
                    UltimaStore.AddPendingItem(m, item);
                    m.SendLocalizedMessage(m.Alive ? 1156846 : 1156848);
                    // Your purchased will be delivered to you once you free up room in your backpack.
                    // Your purchased item will be delivered to you once you are resurrected.
                }
                else
                {
                    if (item is IPromotionalToken && ((IPromotionalToken)item).ItemName != null)
                    {
                        m.SendLocalizedMessage(1075248, ((IPromotionalToken)item).ItemName.ToString()); // A token has been placed in your backpack. Double-click it to redeem your ~1_PROMO~.
                    }
                    else if (item.LabelNumber > 0 || item.Name != null)
                    {
                        m.SendLocalizedMessage(1156844, item.LabelNumber > 0 ? String.Format("#{0}", item.LabelNumber) : item.Name); // Your purchase of ~1_ITEM~ has been placed in your backpack.
                    }
                    else
                    {
                        m.SendLocalizedMessage(1156843); // Your purchased item has been placed in your backpack.
                    }
                }

                if (test)
                    item.Delete();

                return true;
            }
            else
            {
                Utility.WriteConsoleColor(ConsoleColor.Red, String.Format("[Ultima Store Warning]: {0} failed to construct.", ItemType.Name));
            }

            return false;
        }
    }
}