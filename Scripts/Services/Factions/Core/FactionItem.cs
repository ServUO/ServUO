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
        public FactionItem(Item item, Faction faction)
        {
            this.m_Item = item;
            this.m_Faction = faction;
        }

        public FactionItem(GenericReader reader, Faction faction)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Item = reader.ReadItem();
                        this.m_Expiration = reader.ReadDateTime();
                        break;
                    }
            }

            this.m_Faction = faction;
        }

        public Item Item
        {
            get
            {
                return this.m_Item;
            }
        }
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
        }
        public DateTime Expiration
        {
            get
            {
                return this.m_Expiration;
            }
        }
        public bool HasExpired
        {
            get
            {
                if (this.m_Item == null || this.m_Item.Deleted)
                    return true;

                return (this.m_Expiration != DateTime.MinValue && DateTime.UtcNow >= this.m_Expiration);
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

        public static Item Imbue(Item item, Faction faction, bool expire, int hue)
        {
            if (!(item is IFactionItem))
                return item;

            FactionItem state = Find(item);

            if (state == null)
            {
                state = new FactionItem(item, faction);
                state.Attach();
            }

            if (expire)
                state.StartExpiration();

            item.Hue = hue;
            return item;
        }

        public void StartExpiration()
        {
            this.m_Expiration = DateTime.UtcNow + ExpirationPeriod;
        }

        public void CheckAttach()
        {
            if (!this.HasExpired)
                this.Attach();
            else
                this.Detach();
        }

        public void Attach()
        {
            if (this.m_Item is IFactionItem)
                ((IFactionItem)this.m_Item).FactionItemState = this;

            if (this.m_Faction != null)
                this.m_Faction.State.FactionItems.Add(this);
        }

        public void Detach()
        {
            if (this.m_Item is IFactionItem)
                ((IFactionItem)this.m_Item).FactionItemState = null;

            if (this.m_Faction != null && this.m_Faction.State.FactionItems.Contains(this))
                this.m_Faction.State.FactionItems.Remove(this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0);

            writer.Write((Item)this.m_Item);
            writer.Write((DateTime)this.m_Expiration);
        }
    }
}