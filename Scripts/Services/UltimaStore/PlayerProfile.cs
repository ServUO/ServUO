using System.Collections.Generic;

namespace Server.Engines.UOStore
{
    public class PlayerProfile
    {
        public const StoreCategory DefaultCategory = StoreCategory.Featured;
        public const SortBy DefaultSortBy = SortBy.Newest;

        public Dictionary<StoreEntry, int> Cart { get; private set; }

        public Mobile Player { get; private set; }

        public StoreCategory Category { get; set; }
        public SortBy SortBy { get; set; }

        public int VaultTokens { get; set; }

        public PlayerProfile(Mobile m)
        {
            Cart = new Dictionary<StoreEntry, int>();

            Player = m;

            Category = DefaultCategory;
            SortBy = DefaultSortBy;
        }

        public PlayerProfile(GenericReader reader)
        {
            Cart = new Dictionary<StoreEntry, int>();

            Deserialize(reader);
        }

        public void AddToCart(StoreEntry entry, int amount)
        {
            if (Cart.Count < Configuration.CartCapacity)
            {
                Cart[entry] = amount;
            }
        }

        public void RemoveFromCart(StoreEntry entry)
        {
            Cart.Remove(entry);
        }

        public void SetCartAmount(StoreEntry entry, int amount)
        {
            if (amount > 0)
            {
                AddToCart(entry, amount);
            }
            else
            {
                RemoveFromCart(entry);
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(VaultTokens);

            writer.Write(Player);

            writer.Write((int)Category);
            writer.Write((int)SortBy);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    VaultTokens = reader.ReadInt();
                    goto case 0;
                case 0:
                    Player = reader.ReadMobile();

                    Category = (StoreCategory)reader.ReadInt();
                    SortBy = (SortBy)reader.ReadInt();
                    break;
            }
        }
    }
}
