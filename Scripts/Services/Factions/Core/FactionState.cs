using System;
using System.Collections.Generic;

namespace Server.Factions
{
    public class FactionState
    {
        private static readonly TimeSpan BroadcastPeriod = TimeSpan.FromHours(1.0);
        private const int BroadcastsPerPeriod = 2;
        private readonly Faction m_Faction;
        private readonly DateTime[] m_LastBroadcasts = new DateTime[BroadcastsPerPeriod];
        private Mobile m_Commander;
        private int m_Tithe;
        private int m_Silver;
        private List<PlayerState> m_Members;
        private Election m_Election;
        private List<FactionItem> m_FactionItems;
        private List<BaseFactionTrap> m_FactionTraps;
        private DateTime m_LastAtrophy;
        public FactionState(Faction faction)
        {
            this.m_Faction = faction;
            this.m_Tithe = 50;
            this.m_Members = new List<PlayerState>();
            this.m_Election = new Election(faction);
            this.m_FactionItems = new List<FactionItem>();
            this.m_FactionTraps = new List<BaseFactionTrap>();
        }

        public FactionState(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 5:
                    {
                        this.m_LastAtrophy = reader.ReadDateTime();
                        goto case 4;
                    }
                case 4:
                    {
                        int count = reader.ReadEncodedInt();

                        for (int i = 0; i < count; ++i)
                        {
                            DateTime time = reader.ReadDateTime();

                            if (i < this.m_LastBroadcasts.Length)
                                this.m_LastBroadcasts[i] = time;
                        }

                        goto case 3;
                    }
                case 3:
                case 2:
                case 1:
                    {
                        Election ele = new Election(reader);

                        if (Settings.Enabled)
                            this.m_Election = ele;
                        else
                            this.m_Election = new Election(m_Faction);

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Faction = Faction.ReadReference(reader);

                        this.m_Commander = reader.ReadMobile();

                        if (version < 5)
                            this.m_LastAtrophy = DateTime.UtcNow;

                        if (version < 4)
                        {
                            DateTime time = reader.ReadDateTime();

                            if (this.m_LastBroadcasts.Length > 0)
                                this.m_LastBroadcasts[0] = time;
                        }

                        this.m_Tithe = reader.ReadEncodedInt();
                        this.m_Silver = reader.ReadEncodedInt();

                        int memberCount = reader.ReadEncodedInt();

                        this.m_Members = new List<PlayerState>();

                        for (int i = 0; i < memberCount; ++i)
                        {
                            PlayerState pl = new PlayerState(reader, this.m_Faction, this.m_Members);

                            if (Settings.Enabled && pl.Mobile != null)
                                this.m_Members.Add(pl);
                        }

                        this.m_Faction.State = this;
					
                        this.m_Faction.ZeroRankOffset = this.m_Members.Count;
                        this.m_Members.Sort();

                        for (int i = this.m_Members.Count - 1; i >= 0; i--)
                        {
                            PlayerState player = this.m_Members[i];

                            if (player.KillPoints <= 0)
                                this.m_Faction.ZeroRankOffset = i;
                            else
                                player.RankIndex = i;
                        }

                        this.m_FactionItems = new List<FactionItem>();

                        if (version >= 2)
                        {
                            int factionItemCount = reader.ReadEncodedInt();

                            for (int i = 0; i < factionItemCount; ++i)
                            {
                                FactionItem factionItem = new FactionItem(reader, this.m_Faction);

                                if(Settings.Enabled)
                                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(factionItem.CheckAttach)); // sandbox attachment
                            }
                        }

                        this.m_FactionTraps = new List<BaseFactionTrap>();

                        if (version >= 3)
                        {
                            int factionTrapCount = reader.ReadEncodedInt();

                            for (int i = 0; i < factionTrapCount; ++i)
                            {
                                BaseFactionTrap trap = reader.ReadItem() as BaseFactionTrap;

                                if (trap != null && !trap.CheckDecay())
                                {
                                    if (Settings.Enabled)
                                        this.m_FactionTraps.Add(trap);
                                    else
                                        trap.Delete();
                                }
                            }
                        }

                        break;
                    }
            }

            if (version < 1)
                this.m_Election = new Election(this.m_Faction);
        }

        public DateTime LastAtrophy
        {
            get
            {
                return this.m_LastAtrophy;
            }
            set
            {
                this.m_LastAtrophy = value;
            }
        }
        public bool FactionMessageReady
        {
            get
            {
                for (int i = 0; i < this.m_LastBroadcasts.Length; ++i)
                {
                    if (DateTime.UtcNow >= (this.m_LastBroadcasts[i] + BroadcastPeriod))
                        return true;
                }

                return false;
            }
        }
        public bool IsAtrophyReady
        {
            get
            {
                return DateTime.UtcNow >= (this.m_LastAtrophy + TimeSpan.FromHours(47.0));
            }
        }
        public List<FactionItem> FactionItems
        {
            get
            {
                return this.m_FactionItems;
            }
            set
            {
                this.m_FactionItems = value;
            }
        }
        public List<BaseFactionTrap> Traps
        {
            get
            {
                return this.m_FactionTraps;
            }
            set
            {
                this.m_FactionTraps = value;
            }
        }
        public Election Election
        {
            get
            {
                return this.m_Election;
            }
            set
            {
                this.m_Election = value;
            }
        }
        public Mobile Commander
        {
            get
            {
                return this.m_Commander;
            }
            set
            {
                if (this.m_Commander != null)
                    this.m_Commander.InvalidateProperties();

                this.m_Commander = value;

                if (this.m_Commander != null)
                {
                    this.m_Commander.SendLocalizedMessage(1042227); // You have been elected Commander of your faction

                    this.m_Commander.InvalidateProperties();

                    PlayerState pl = PlayerState.Find(this.m_Commander);

                    if (pl != null && pl.Finance != null)
                        pl.Finance.Finance = null;

                    if (pl != null && pl.Sheriff != null)
                        pl.Sheriff.Sheriff = null;
                }
            }
        }
        public int Tithe
        {
            get
            {
                return this.m_Tithe;
            }
            set
            {
                this.m_Tithe = value;
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
        public List<PlayerState> Members
        {
            get
            {
                return this.m_Members;
            }
            set
            {
                this.m_Members = value;
            }
        }
        public int CheckAtrophy()
        {
            if (DateTime.UtcNow < (this.m_LastAtrophy + TimeSpan.FromHours(47.0)))
                return 0;

            int distrib = 0;
            this.m_LastAtrophy = DateTime.UtcNow;

            List<PlayerState> members = new List<PlayerState>(this.m_Members);

            for (int i = 0; i < members.Count; ++i)
            {
                PlayerState ps = members[i];
					
                if (ps.IsActive)
                {
                    ps.IsActive = false;
                    continue;
                }
                else if (ps.KillPoints > 0)
                {
                    int atrophy = (ps.KillPoints + 9) / 10;
                    ps.KillPoints -= atrophy;
                    distrib += atrophy;
                }
            }

            return distrib;
        }

        public void RegisterBroadcast()
        {
            for (int i = 0; i < this.m_LastBroadcasts.Length; ++i)
            {
                if (DateTime.UtcNow >= (this.m_LastBroadcasts[i] + BroadcastPeriod))
                {
                    this.m_LastBroadcasts[i] = DateTime.UtcNow;
                    break;
                }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)5); // version

            writer.Write(this.m_LastAtrophy);

            writer.WriteEncodedInt((int)this.m_LastBroadcasts.Length);

            for (int i = 0; i < this.m_LastBroadcasts.Length; ++i)
                writer.Write((DateTime)this.m_LastBroadcasts[i]);

            this.m_Election.Serialize(writer);

            Faction.WriteReference(writer, this.m_Faction);

            writer.Write((Mobile)this.m_Commander);

            writer.WriteEncodedInt((int)this.m_Tithe);
            writer.WriteEncodedInt((int)this.m_Silver);

            writer.WriteEncodedInt((int)this.m_Members.Count);

            for (int i = 0; i < this.m_Members.Count; ++i)
            {
                PlayerState pl = (PlayerState)this.m_Members[i];

                pl.Serialize(writer);
            }

            writer.WriteEncodedInt((int)this.m_FactionItems.Count);

            for (int i = 0; i < this.m_FactionItems.Count; ++i)
                this.m_FactionItems[i].Serialize(writer);

            writer.WriteEncodedInt((int)this.m_FactionTraps.Count);

            for (int i = 0; i < this.m_FactionTraps.Count; ++i)
                writer.Write((Item)this.m_FactionTraps[i]);
        }
    }
}