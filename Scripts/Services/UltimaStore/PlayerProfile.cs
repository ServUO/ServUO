using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.UOStore
{
    public class PlayerProfile
    {
        public const StoreCategory DefaultCategory = StoreCategory.Featured;
        public const SortBy DefaultSortBy = SortBy.Newest;

        public Mobile Player { get; private set; }
        public StoreCategory Category { get; set; }
        public SortBy SortBy { get; set; }

        public Dictionary<StoreEntry, int> Cart { get; private set; }

        public PlayerProfile(Mobile m)
        {
            Player = m;

            Category = DefaultCategory;
            SortBy = DefaultSortBy;
        }

        public void AddToCart(StoreEntry entry, int amount)
        {
            if (Cart == null)
                Cart = new Dictionary<StoreEntry, int>();

            if (Cart.Count < UltimaStore.MaxCart || Cart.ContainsKey(entry))
            {
                Cart[entry] = amount;
            }
        }

        public void RemoveFromCart(StoreEntry entry)
        {
            if (Cart != null && Cart.ContainsKey(entry))
            {
                Cart.Remove(entry);

                if (Cart.Count == 0)
                    Cart = null;
            }
        }

        public void SetCartAmount(StoreEntry entry, int amount)
        {
            if (amount == 0)
            {
                RemoveFromCart(entry);
            }
            else
            {
                AddToCart(entry, amount);
            }
        }

        public PlayerProfile(GenericReader reader)
        {
            int version = reader.ReadInt();

            Player = reader.ReadMobile();
            Category = (StoreCategory)reader.ReadInt();
            SortBy = (SortBy)reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Player);
            writer.Write((int)Category);
            writer.Write((int)SortBy);
        }
    }
}