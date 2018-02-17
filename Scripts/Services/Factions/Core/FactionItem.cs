using System;

namespace Server.Factions
{
    public interface IFactionItem
    {
        FactionItem FactionItemState { get; set; }
    }

    public class FactionItem
    {
        public static readonly TimeSpan ExpirationPeriod = TimeSpan.FromDays(21.0);
        private readonly Item m_Item;
        private readonly Faction m_Faction;
        private DateTime m_Expiration;
        private int m_MinRank;

        public FactionItem(Item item, Faction faction, int level)
        {
            m_Item = item;
            m_Faction = faction;
            m_MinRank = level;
        }

        public FactionItem(GenericReader reader, Faction faction)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        m_MinRank = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Item = reader.ReadItem();
                        m_Expiration = reader.ReadDateTime();
                        break;
                    }
            }

            m_Faction = faction;
        }

        public Item Item
        {
            get
            {
                return m_Item;
            }
        }
        public Faction Faction
        {
            get
            {
                return m_Faction;
            }
        }
        public DateTime Expiration
        {
            get
            {
                return m_Expiration;
            }
        }
        public int MinRank
        {
            get
            {
                return m_MinRank;
            }
        }
        public bool HasExpired
        {
            get
            {
                if (m_Item == null || m_Item.Deleted)
                    return true;

                return (m_Expiration != DateTime.MinValue && DateTime.UtcNow >= m_Expiration);
            }
        }
        public static int GetMaxWearables(Mobile mob)
        {
            PlayerState pl = PlayerState.Find(mob);

            if (pl == null)
                return 0;

            if (pl.Faction.IsCommander(mob))
                return 9;

            return pl.Rank.MaxWearables;
        }

        public static FactionItem Find(Item item)
        {
            if (item is IFactionItem)
            {
                FactionItem state = ((IFactionItem)item).FactionItemState;

                if (state != null && state.HasExpired)
                {
                    state.Detach();
                    state = null;
                }

                return state;
            }

            return null;
        }

        public static Item Imbue(Item item, Faction faction, bool expire, int hue, int MinRank = 0)
        {
            if (!(item is IFactionItem))
                return item;

            FactionItem state = Find(item);

            if (state == null)
            {
                state = new FactionItem(item, faction, MinRank);
                state.Attach();
            }

            if (expire)
                state.StartExpiration();

            if (hue >= 0)
                item.Hue = hue;

            return item;
        }

        public void StartExpiration()
        {
            m_Expiration = DateTime.UtcNow + ExpirationPeriod;
        }

        public void CheckAttach()
        {
            if (!HasExpired)
                Attach();
            else
                Detach();
        }

        public void Attach()
        {
            if (m_Item is IFactionItem)
                ((IFactionItem)m_Item).FactionItemState = this;

            if (m_Faction != null)
                m_Faction.State.FactionItems.Add(this);
        }

        public void Detach()
        {
            if (m_Item is IFactionItem)
                ((IFactionItem)m_Item).FactionItemState = null;

            if (m_Faction != null && m_Faction.State.FactionItems.Contains(this))
                m_Faction.State.FactionItems.Remove(this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)1);

            writer.Write(m_MinRank);

            writer.Write((Item)m_Item);
            writer.Write((DateTime)m_Expiration);
        }
    }
}