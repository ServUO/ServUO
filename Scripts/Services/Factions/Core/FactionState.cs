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
            m_Faction = faction;
            m_Tithe = 50;
            m_Members = new List<PlayerState>();
            m_Election = new Election(faction);
            m_FactionItems = new List<FactionItem>();
            m_FactionTraps = new List<BaseFactionTrap>();
        }

        public FactionState(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 6:
                case 5:
                    {
                        m_LastAtrophy = reader.ReadDateTime();
                        goto case 4;
                    }
                case 4:
                    {
                        int count = reader.ReadEncodedInt();

                        for (int i = 0; i < count; ++i)
                        {
                            DateTime time = reader.ReadDateTime();

                            if (i < m_LastBroadcasts.Length)
                                m_LastBroadcasts[i] = time;
                        }

                        goto case 3;
                    }
                case 3:
                case 2:
                case 1:
                    {
                        Election ele = new Election(reader);

                        if (Settings.Enabled)
                            m_Election = ele;
                        else
                            m_Election = new Election(m_Faction);

                        goto case 0;
                    }
                case 0:
                    {
                        m_Faction = Faction.ReadReference(reader);

                        m_Commander = reader.ReadMobile();

                        if (version < 5)
                            m_LastAtrophy = DateTime.UtcNow;

                        if (version < 4)
                        {
                            DateTime time = reader.ReadDateTime();

                            if (m_LastBroadcasts.Length > 0)
                                m_LastBroadcasts[0] = time;
                        }

                        m_Tithe = reader.ReadEncodedInt();
                        m_Silver = reader.ReadEncodedInt();

                        int memberCount = reader.ReadEncodedInt();

                        m_Members = new List<PlayerState>();

                        for (int i = 0; i < memberCount; ++i)
                        {
                            PlayerState pl = new PlayerState(reader, m_Faction, m_Members);

                            if (pl.Mobile != null)
                            {
                                if (Settings.Enabled)
                                {
                                    m_Members.Add(pl);
                                }
                                else
                                {
                                    Settings.AddDisabledNotice(pl.Mobile);
                                }
                            }
                        }

                        m_Faction.State = this;
					
                        m_Faction.ZeroRankOffset = m_Members.Count;
                        m_Members.Sort();

                        for (int i = m_Members.Count - 1; i >= 0; i--)
                        {
                            PlayerState player = m_Members[i];

                            if (player.KillPoints <= 0)
                                m_Faction.ZeroRankOffset = i;
                            else
                                player.RankIndex = i;
                        }

                        m_FactionItems = new List<FactionItem>();

                        if (version >= 2)
                        {
                            int factionItemCount = reader.ReadEncodedInt();

                            for (int i = 0; i < factionItemCount; ++i)
                            {
                                FactionItem factionItem = new FactionItem(reader, m_Faction);

                                if(Settings.Enabled)
                                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(factionItem.CheckAttach)); // sandbox attachment
                            }
                        }

                        m_FactionTraps = new List<BaseFactionTrap>();

                        if (version >= 3)
                        {
                            int factionTrapCount = reader.ReadEncodedInt();

                            for (int i = 0; i < factionTrapCount; ++i)
                            {
                                BaseFactionTrap trap = reader.ReadItem() as BaseFactionTrap;

                                if (trap != null && !trap.CheckDecay())
                                {
                                    if (Settings.Enabled)
                                        m_FactionTraps.Add(trap);
                                    else
                                        trap.Delete();
                                }
                            }
                        }

                        break;
                    }
            }

            if (version < 6 && Settings.Enabled && Core.ML)
            {
                FactionCollectionBox box = new FactionCollectionBox(m_Faction);
                WeakEntityCollection.Add("factions", box);
                box.MoveToWorld(m_Faction.Definition.Stronghold.CollectionBox, Faction.Facet);
            }

            if (version < 1)
                m_Election = new Election(m_Faction);
        }

        public DateTime LastAtrophy
        {
            get
            {
                return m_LastAtrophy;
            }
            set
            {
                m_LastAtrophy = value;
            }
        }
        public bool FactionMessageReady
        {
            get
            {
                for (int i = 0; i < m_LastBroadcasts.Length; ++i)
                {
                    if (DateTime.UtcNow >= (m_LastBroadcasts[i] + BroadcastPeriod))
                        return true;
                }

                return false;
            }
        }
        public bool IsAtrophyReady
        {
            get
            {
                return DateTime.UtcNow >= (m_LastAtrophy + TimeSpan.FromHours(47.0));
            }
        }
        public List<FactionItem> FactionItems
        {
            get
            {
                return m_FactionItems;
            }
            set
            {
                m_FactionItems = value;
            }
        }
        public List<BaseFactionTrap> Traps
        {
            get
            {
                return m_FactionTraps;
            }
            set
            {
                m_FactionTraps = value;
            }
        }
        public Election Election
        {
            get
            {
                return m_Election;
            }
            set
            {
                m_Election = value;
            }
        }
        public Mobile Commander
        {
            get
            {
                return m_Commander;
            }
            set
            {
                if (m_Commander != null)
                    m_Commander.InvalidateProperties();

                m_Commander = value;

                if (m_Commander != null)
                {
                    m_Commander.SendLocalizedMessage(1042227); // You have been elected Commander of your faction

                    m_Commander.InvalidateProperties();

                    PlayerState pl = PlayerState.Find(m_Commander);

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
                return m_Tithe;
            }
            set
            {
                m_Tithe = value;
            }
        }
        public int Silver
        {
            get
            {
                return m_Silver;
            }
            set
            {
                m_Silver = value;
            }
        }
        public List<PlayerState> Members
        {
            get
            {
                return m_Members;
            }
            set
            {
                m_Members = value;
            }
        }
        public int CheckAtrophy()
        {
            if (DateTime.UtcNow < (m_LastAtrophy + TimeSpan.FromHours(47.0)))
                return 0;

            int distrib = 0;
            m_LastAtrophy = DateTime.UtcNow;

            List<PlayerState> members = new List<PlayerState>(m_Members);

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
            for (int i = 0; i < m_LastBroadcasts.Length; ++i)
            {
                if (DateTime.UtcNow >= (m_LastBroadcasts[i] + BroadcastPeriod))
                {
                    m_LastBroadcasts[i] = DateTime.UtcNow;
                    break;
                }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)6); // version

            writer.Write(m_LastAtrophy);

            writer.WriteEncodedInt((int)m_LastBroadcasts.Length);

            for (int i = 0; i < m_LastBroadcasts.Length; ++i)
                writer.Write((DateTime)m_LastBroadcasts[i]);

            m_Election.Serialize(writer);

            Faction.WriteReference(writer, m_Faction);

            writer.Write((Mobile)m_Commander);

            writer.WriteEncodedInt((int)m_Tithe);
            writer.WriteEncodedInt((int)m_Silver);

            writer.WriteEncodedInt((int)m_Members.Count);

            for (int i = 0; i < m_Members.Count; ++i)
            {
                PlayerState pl = (PlayerState)m_Members[i];

                pl.Serialize(writer);
            }

            writer.WriteEncodedInt((int)m_FactionItems.Count);

            for (int i = 0; i < m_FactionItems.Count; ++i)
                m_FactionItems[i].Serialize(writer);

            writer.WriteEncodedInt((int)m_FactionTraps.Count);

            for (int i = 0; i < m_FactionTraps.Count; ++i)
                writer.Write((Item)m_FactionTraps[i]);
        }
    }
}