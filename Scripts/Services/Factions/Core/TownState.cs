using System;

namespace Server.Factions
{
    public class TownState
    {
        private Town m_Town;
        private Faction m_Owner;
        private Mobile m_Sheriff;
        private Mobile m_Finance;
        private int m_Silver;
        private int m_Tax;
        private DateTime m_LastTaxChange;
        private DateTime m_LastIncome;
        public TownState(Town town)
        {
            this.m_Town = town;
        }

        public TownState(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 3:
                    {
                        this.m_LastIncome = reader.ReadDateTime();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Tax = reader.ReadEncodedInt();
                        this.m_LastTaxChange = reader.ReadDateTime();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Silver = reader.ReadEncodedInt();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Town = Town.ReadReference(reader);
                        this.m_Owner = Faction.ReadReference(reader);

                        this.m_Sheriff = reader.ReadMobile();
                        this.m_Finance = reader.ReadMobile();

                        this.m_Town.State = this;

                        break;
                    }
            }
        }

        public Town Town
        {
            get
            {
                return this.m_Town;
            }
            set
            {
                this.m_Town = value;
            }
        }
        public Faction Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        public Mobile Sheriff
        {
            get
            {
                return this.m_Sheriff;
            }
            set
            {
                if (this.m_Sheriff != null)
                {
                    PlayerState pl = PlayerState.Find(this.m_Sheriff);

                    if (pl != null)
                        pl.Sheriff = null;
                }

                this.m_Sheriff = value;

                if (this.m_Sheriff != null)
                {
                    PlayerState pl = PlayerState.Find(this.m_Sheriff);

                    if (pl != null)
                        pl.Sheriff = this.m_Town;
                }
            }
        }
        public Mobile Finance
        {
            get
            {
                return this.m_Finance;
            }
            set
            {
                if (this.m_Finance != null)
                {
                    PlayerState pl = PlayerState.Find(this.m_Finance);

                    if (pl != null)
                        pl.Finance = null;
                }

                this.m_Finance = value;

                if (this.m_Finance != null)
                {
                    PlayerState pl = PlayerState.Find(this.m_Finance);

                    if (pl != null)
                        pl.Finance = this.m_Town;
                }
            }
        }
        public int Silver
        {
            get
            {
                return this.m_Silver;
            }
            set
            {
                this.m_Silver = value;
            }
        }
        public int Tax
        {
            get
            {
                return this.m_Tax;
            }
            set
            {
                this.m_Tax = value;
            }
        }
        public DateTime LastTaxChange
        {
            get
            {
                return this.m_LastTaxChange;
            }
            set
            {
                this.m_LastTaxChange = value;
            }
        }
        public DateTime LastIncome
        {
            get
            {
                return this.m_LastIncome;
            }
            set
            {
                this.m_LastIncome = value;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)3); // version

            writer.Write((DateTime)this.m_LastIncome);

            writer.WriteEncodedInt((int)this.m_Tax);
            writer.Write((DateTime)this.m_LastTaxChange);

            writer.WriteEncodedInt((int)this.m_Silver);

            Town.WriteReference(writer, this.m_Town);
            Faction.WriteReference(writer, this.m_Owner);

            writer.Write((Mobile)this.m_Sheriff);
            writer.Write((Mobile)this.m_Finance);
        }
    }
}