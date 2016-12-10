using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Factions
{
    public class PlayerState : IComparable
    {
        private readonly Mobile m_Mobile;
        private readonly Faction m_Faction;
        private readonly List<PlayerState> m_Owner;
        private int m_KillPoints;
        private DateTime m_Leaving;
        private MerchantTitle m_MerchantTitle;
        private RankDefinition m_Rank;
        private List<SilverGivenEntry> m_SilverGiven;
        private bool m_IsActive;
        private Town m_Sheriff;
        private Town m_Finance;
        private DateTime m_LastHonorTime;
        private bool m_InvalidateRank = true;
        private int m_RankIndex = -1;
        public PlayerState(Mobile mob, Faction faction, List<PlayerState> owner)
        {
            this.m_Mobile = mob;
            this.m_Faction = faction;
            this.m_Owner = owner;

            this.Attach();
            this.Invalidate();
        }

        public PlayerState(GenericReader reader, Faction faction, List<PlayerState> owner)
        {
            this.m_Faction = faction;
            this.m_Owner = owner;

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_IsActive = reader.ReadBool();
                        this.m_LastHonorTime = reader.ReadDateTime();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Mobile = reader.ReadMobile();

                        this.m_KillPoints = reader.ReadEncodedInt();
                        this.m_MerchantTitle = (MerchantTitle)reader.ReadEncodedInt();

                        this.m_Leaving = reader.ReadDateTime();

                        break;
                    }
            }

            this.Attach();
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
        }
        public List<PlayerState> Owner
        {
            get
            {
                return this.m_Owner;
            }
        }
        public MerchantTitle MerchantTitle
        {
            get
            {
                return this.m_MerchantTitle;
            }
            set
            {
                this.m_MerchantTitle = value;
                this.Invalidate();
            }
        }
        public Town Sheriff
        {
            get
            {
                return this.m_Sheriff;
            }
            set
            {
                this.m_Sheriff = value;
                this.Invalidate();
            }
        }
        public Town Finance
        {
            get
            {
                return this.m_Finance;
            }
            set
            {
                this.m_Finance = value;
                this.Invalidate();
            }
        }
        public List<SilverGivenEntry> SilverGiven
        {
            get
            {
                return this.m_SilverGiven;
            }
        }
        public int KillPoints
        { 
            get
            {
                return this.m_KillPoints;
            }
            set
            { 
                if (this.m_KillPoints != value)
                {
                    if (value > this.m_KillPoints)
                    {
                        if (this.m_KillPoints <= 0)
                        {
                            if (value <= 0)
                            {
                                this.m_KillPoints = value;
                                this.Invalidate();
                                return;
                            }
							
                            this.m_Owner.Remove(this);
                            this.m_Owner.Insert(this.m_Faction.ZeroRankOffset, this);

                            this.m_RankIndex = this.m_Faction.ZeroRankOffset;
                            this.m_Faction.ZeroRankOffset++;
                        }
                        while ((this.m_RankIndex - 1) >= 0)
                        {
                            PlayerState p = this.m_Owner[this.m_RankIndex - 1] as PlayerState;
                            if (value > p.KillPoints)
                            {
                                this.m_Owner[this.m_RankIndex] = p;
                                this.m_Owner[this.m_RankIndex - 1] = this;
                                this.RankIndex--;
                                p.RankIndex++;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        if (value <= 0)
                        {
                            if (this.m_KillPoints <= 0)
                            {
                                this.m_KillPoints = value;
                                this.Invalidate();
                                return;
                            }

                            while ((this.m_RankIndex + 1) < this.m_Faction.ZeroRankOffset)
                            {
                                PlayerState p = this.m_Owner[this.m_RankIndex + 1] as PlayerState;
                                this.m_Owner[this.m_RankIndex + 1] = this;
                                this.m_Owner[this.m_RankIndex] = p;
                                this.RankIndex++;
                                p.RankIndex--;
                            }

                            this.m_RankIndex = -1;
                            this.m_Faction.ZeroRankOffset--;
                        }
                        else
                        {
                            while ((this.m_RankIndex + 1) < this.m_Faction.ZeroRankOffset)
                            {
                                PlayerState p = this.m_Owner[this.m_RankIndex + 1] as PlayerState;
                                if (value < p.KillPoints)
                                {
                                    this.m_Owner[this.m_RankIndex + 1] = this;
                                    this.m_Owner[this.m_RankIndex] = p;
                                    this.RankIndex++;
                                    p.RankIndex--;
                                }
                                else
                                    break;
                            }
                        }
                    }

                    this.m_KillPoints = value;
                    this.Invalidate();
                }
            }
        }
        public int RankIndex
        {
            get
            {
                return this.m_RankIndex;
            }
            set
            {
                if (this.m_RankIndex != value)
                {
                    this.m_RankIndex = value;
                    this.m_InvalidateRank = true;
                }
            }
        }
        public RankDefinition Rank
        { 
            get
            { 
                if (this.m_InvalidateRank)
                {
                    RankDefinition[] ranks = this.m_Faction.Definition.Ranks;
                    int percent;

                    if (this.m_Owner.Count == 1)
                        percent = 1000;
                    else if (this.m_RankIndex == -1)
                        percent = 0;
                    else
                        percent = ((this.m_Faction.ZeroRankOffset - this.m_RankIndex) * 1000) / this.m_Faction.ZeroRankOffset;

                    for (int i = 0; i < ranks.Length; i++)
                    {
                        RankDefinition check = ranks[i];

                        if (percent >= check.Required)
                        {
                            this.m_Rank = check;
                            this.m_InvalidateRank = false;
                            break;
                        }
                    }

                    this.Invalidate();
                }

                return this.m_Rank;
            }
        }
        public DateTime LastHonorTime
        {
            get
            {
                return this.m_LastHonorTime;
            }
            set
            {
                this.m_LastHonorTime = value;
            }
        }
        public DateTime Leaving
        {
            get
            {
                return this.m_Leaving;
            }
            set
            {
                this.m_Leaving = value;
            }
        }
        public bool IsLeaving
        {
            get
            {
                return (this.m_Leaving > DateTime.MinValue);
            }
        }
        public bool IsActive
        {
            get
            {
                return this.m_IsActive;
            }
            set
            {
                this.m_IsActive = value;
            }
        }
        public static PlayerState Find(Mobile mob)
        {
            if (mob is PlayerMobile)
                return ((PlayerMobile)mob).FactionPlayerState;

            return null;
        }

        public bool CanGiveSilverTo(Mobile mob)
        {
            if (this.m_SilverGiven == null)
                return true;

            for (int i = 0; i < this.m_SilverGiven.Count; ++i)
            {
                SilverGivenEntry sge = this.m_SilverGiven[i];

                if (sge.IsExpired)
                    this.m_SilverGiven.RemoveAt(i--);
                else if (sge.GivenTo == mob)
                    return false;
            }

            return true;
        }

        public void OnGivenSilverTo(Mobile mob)
        {
            if (this.m_SilverGiven == null)
                this.m_SilverGiven = new List<SilverGivenEntry>();

            this.m_SilverGiven.Add(new SilverGivenEntry(mob));
        }

        public void Invalidate()
        {
            if (this.m_Mobile is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)this.m_Mobile;
                pm.InvalidateProperties();
                pm.InvalidateMyRunUO();
            }
        }

        public void Attach()
        {
            if (Settings.Enabled && this.m_Mobile is PlayerMobile)
                ((PlayerMobile)this.m_Mobile).FactionPlayerState = this;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)1); // version

            writer.Write(this.m_IsActive);
            writer.Write(this.m_LastHonorTime);

            writer.Write((Mobile)this.m_Mobile);

            writer.WriteEncodedInt((int)this.m_KillPoints);
            writer.WriteEncodedInt((int)this.m_MerchantTitle);

            writer.Write((DateTime)this.m_Leaving);
        }

        public int CompareTo(object obj)
        {
            return ((PlayerState)obj).m_KillPoints - this.m_KillPoints;
        }
    }
}