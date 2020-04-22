using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;

namespace Server.Items
{
    public enum CampfireStatus
    {
        Burning,
        Extinguishing,
        Off
    }

    public class Campfire : Item
    {
        public static readonly int SecureRange = 7;
        private static readonly Hashtable m_Table = new Hashtable();
        private readonly Timer m_Timer;
        private readonly DateTime m_Created;
        private readonly ArrayList m_Entries;
        public Campfire()
            : base(0xDE3)
        {
            Movable = false;
            Light = LightType.Circle300;

            m_Entries = new ArrayList();

            m_Created = DateTime.UtcNow;
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), OnTick);
        }

        public Campfire(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Created => m_Created;
        [CommandProperty(AccessLevel.GameMaster)]
        public CampfireStatus Status
        {
            get
            {
                switch (ItemID)
                {
                    case 0xDE3:
                        return CampfireStatus.Burning;

                    case 0xDE9:
                        return CampfireStatus.Extinguishing;

                    default:
                        return CampfireStatus.Off;
                }
            }
            set
            {
                if (Status == value)
                    return;

                switch (value)
                {
                    case CampfireStatus.Burning:
                        ItemID = 0xDE3;
                        Light = LightType.Circle300;
                        break;
                    case CampfireStatus.Extinguishing:
                        ItemID = 0xDE9;
                        Light = LightType.Circle150;
                        break;
                    default:
                        ItemID = 0xDEA;
                        Light = LightType.ArchedWindowEast;
                        ClearEntries();
                        break;
                }
            }
        }
        public static CampfireEntry GetEntry(Mobile player)
        {
            return (CampfireEntry)m_Table[player];
        }

        public static void RemoveEntry(CampfireEntry entry)
        {
            m_Table.Remove(entry.Player);
            entry.Fire.m_Entries.Remove(entry);
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            ClearEntries();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Delete();
        }

        private void OnTick()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan age = now - Created;

            if (age >= TimeSpan.FromSeconds(100.0))
                Delete();
            else if (age >= TimeSpan.FromSeconds(90.0))
                Status = CampfireStatus.Off;
            else if (age >= TimeSpan.FromSeconds(60.0))
                Status = CampfireStatus.Extinguishing;

            if (Status == CampfireStatus.Off || Deleted)
                return;

            foreach (CampfireEntry entry in new ArrayList(m_Entries))
            {
                if (!entry.Valid || entry.Player.NetState == null)
                {
                    RemoveEntry(entry);
                }
                else if (!entry.Safe && now - entry.Start >= TimeSpan.FromSeconds(30.0))
                {
                    entry.Safe = true;
                    entry.Player.SendLocalizedMessage(500621); // The camp is now secure.
                }
            }

            IPooledEnumerable eable = GetClientsInRange(SecureRange);

            foreach (NetState state in eable)
            {
                PlayerMobile pm = state.Mobile as PlayerMobile;

                if (pm != null && GetEntry(pm) == null)
                {
                    CampfireEntry entry = new CampfireEntry(pm, this);

                    m_Table[pm] = entry;
                    m_Entries.Add(entry);

                    pm.SendLocalizedMessage(500620); // You feel it would take a few moments to secure your camp.
                }
            }

            eable.Free();
        }

        private void ClearEntries()
        {
            if (m_Entries == null)
                return;

            foreach (CampfireEntry entry in new ArrayList(m_Entries))
            {
                RemoveEntry(entry);
            }
        }
    }

    public class CampfireEntry
    {
        private readonly PlayerMobile m_Player;
        private readonly Campfire m_Fire;
        private readonly DateTime m_Start;
        private bool m_Safe;
        public CampfireEntry(PlayerMobile player, Campfire fire)
        {
            m_Player = player;
            m_Fire = fire;
            m_Start = DateTime.UtcNow;
            m_Safe = false;
        }

        public PlayerMobile Player => m_Player;
        public Campfire Fire => m_Fire;
        public DateTime Start => m_Start;
        public bool Valid => !Fire.Deleted && Fire.Status != CampfireStatus.Off && Player.Map == Fire.Map && Player.InRange(Fire, Campfire.SecureRange);
        public bool Safe
        {
            get
            {
                return Valid && m_Safe;
            }
            set
            {
                m_Safe = value;
            }
        }
    }
}
