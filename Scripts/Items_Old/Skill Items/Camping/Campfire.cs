using System;
using System.Collections;
using Server.Mobiles;
using Server.Network;

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
            this.Movable = false;
            this.Light = LightType.Circle300;

            this.m_Entries = new ArrayList();

            this.m_Created = DateTime.UtcNow;
            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), new TimerCallback(OnTick));
        }

        public Campfire(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Created
        {
            get
            {
                return this.m_Created;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public CampfireStatus Status
        {
            get
            {
                switch ( this.ItemID )
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
                if (this.Status == value)
                    return;

                switch ( value )
                {
                    case CampfireStatus.Burning:
                        this.ItemID = 0xDE3;
                        this.Light = LightType.Circle300;
                        break;
                    case CampfireStatus.Extinguishing:
                        this.ItemID = 0xDE9;
                        this.Light = LightType.Circle150;
                        break;
                    default:
                        this.ItemID = 0xDEA;
                        this.Light = LightType.ArchedWindowEast;
                        this.ClearEntries();
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
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.ClearEntries();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.Delete();
        }

        private void OnTick()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan age = now - this.Created;

            if (age >= TimeSpan.FromSeconds(100.0))
                this.Delete();
            else if (age >= TimeSpan.FromSeconds(90.0))
                this.Status = CampfireStatus.Off;
            else if (age >= TimeSpan.FromSeconds(60.0))
                this.Status = CampfireStatus.Extinguishing;

            if (this.Status == CampfireStatus.Off || this.Deleted)
                return;

            foreach (CampfireEntry entry in new ArrayList(this.m_Entries))
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

            IPooledEnumerable eable = this.GetClientsInRange(SecureRange);

            foreach (NetState state in eable)
            {
                PlayerMobile pm = state.Mobile as PlayerMobile;

                if (pm != null && GetEntry(pm) == null)
                {
                    CampfireEntry entry = new CampfireEntry(pm, this);

                    m_Table[pm] = entry;
                    this.m_Entries.Add(entry);

                    pm.SendLocalizedMessage(500620); // You feel it would take a few moments to secure your camp.
                }
            }

            eable.Free();
        }

        private void ClearEntries()
        {
            if (this.m_Entries == null)
                return;

            foreach (CampfireEntry entry in new ArrayList(this.m_Entries))
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
            this.m_Player = player;
            this.m_Fire = fire;
            this.m_Start = DateTime.UtcNow;
            this.m_Safe = false;
        }

        public PlayerMobile Player
        {
            get
            {
                return this.m_Player;
            }
        }
        public Campfire Fire
        {
            get
            {
                return this.m_Fire;
            }
        }
        public DateTime Start
        {
            get
            {
                return this.m_Start;
            }
        }
        public bool Valid
        {
            get
            {
                return !this.Fire.Deleted && this.Fire.Status != CampfireStatus.Off && this.Player.Map == this.Fire.Map && this.Player.InRange(this.Fire, Campfire.SecureRange);
            }
        }
        public bool Safe
        {
            get
            {
                return this.Valid && this.m_Safe;
            }
            set
            {
                this.m_Safe = value;
            }
        }
    }
}