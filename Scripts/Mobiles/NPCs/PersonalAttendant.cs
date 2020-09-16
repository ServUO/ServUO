using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class PersonalAttendant : BaseCreature
    {
        private static readonly Dictionary<Mobile, Mobile> m_Table = new Dictionary<Mobile, Mobile>();
        private bool m_BindedToPlayer;
        private InternalTimer m_Timer;
        public PersonalAttendant(string title)
            : base(AIType.AI_Vendor, FightMode.None, 22, 1, 0.15, 0.2)
        {
            Title = title;
            Blessed = true;
            ControlSlots = 0;

            InitBody();
            InitOutfit();

            m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(2));
            m_Timer.Start();
        }

        public PersonalAttendant(Serial serial)
            : base(serial)
        {
        }

        public override bool ShowFameTitle => true;
        public override bool Commandable => false;
        public override bool NoHouseRestrictions => true;
        public override bool CanOpenDoors => true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BindedToPlayer
        {
            get
            {
                return m_BindedToPlayer;
            }
            set
            {
                m_BindedToPlayer = value;
            }
        }
        public static bool CheckAttendant(Mobile owner)
        {
            if (owner != null)
                return m_Table.ContainsKey(owner);

            return false;
        }

        public static void AddAttendant(Mobile owner, PersonalAttendant attendant)
        {
            if (owner != null)
                m_Table[owner] = attendant;
        }

        public static void RemoveAttendant(Mobile owner)
        {
            if (owner != null)
                m_Table.Remove(owner);
        }

        public virtual void InitBody()
        {
        }

        public virtual void InitOutfit()
        {
        }

        public virtual void CommandFollow(Mobile by)
        {
            ControlOrder = OrderType.Follow;
            ControlTarget = by;

            if (m_Timer != null)
            {
                m_Timer.Interval = TimeSpan.FromSeconds(2);
                m_Timer.Priority = TimerPriority.OneSecond;
            }
        }

        public virtual void CommandStop(Mobile by)
        {
            ControlOrder = OrderType.Stay;
            ControlTarget = null;

            if (m_Timer != null)
            {
                m_Timer.Interval = TimeSpan.FromSeconds(5);
                m_Timer.Priority = TimerPriority.FiveSeconds;
            }
        }

        public virtual void Dismiss(Mobile owner)
        {
            RemoveAttendant(owner);

            if (m_BindedToPlayer)
                owner.AddToBackpack(new PersonalAttendantDeed(owner));
            else
                owner.AddToBackpack(new PersonalAttendantDeed());

            Delete();
        }

        public virtual bool InGreetingMode(Mobile owner)
        {
            return false;
        }

        public virtual bool IsOwner(Mobile m)
        {
            return (ControlMaster == null || ControlMaster == m);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && IsOwner(from))
            {
                list.Add(new AttendantFollowEntry(this));
                list.Add(new AttendantStopEntry(this));
                list.Add(new AttendantDismissEntry(this));
            }
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return (int)from.AccessLevel >= (int)AccessLevel.GameMaster;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write(m_BindedToPlayer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    m_BindedToPlayer = reader.ReadBool();
                    break;
            }

            TimeSpan delay = TimeSpan.FromSeconds(2);

            if (ControlOrder == OrderType.Stay)
                delay = TimeSpan.FromSeconds(5);

            m_Timer = new InternalTimer(this, delay);
            m_Timer.Start();

            AddAttendant(ControlMaster, this);
        }

        private class InternalTimer : Timer
        {
            private readonly PersonalAttendant m_Attendant;
            public InternalTimer(PersonalAttendant attendant, TimeSpan delay)
                : base(delay, delay)
            {
                m_Attendant = attendant;

                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Attendant != null && !m_Attendant.Deleted)
                {
                    Mobile m = m_Attendant.ControlMaster;

                    if (m != null)
                    {
                        if ((m.NetState == null || !m.Alive) && !m_Attendant.InGreetingMode(m))
                            m_Attendant.Dismiss(m);
                        else if (m_Attendant.ControlOrder == OrderType.Follow && !m.InRange(m_Attendant.Location, 12))
                            DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(CatchUp), m.Location);
                    }
                }
            }

            private void CatchUp(object obj)
            {
                if (m_Attendant != null && !m_Attendant.Deleted)
                {
                    m_Attendant.ControlOrder = OrderType.Follow;
                    m_Attendant.ControlTarget = m_Attendant.ControlMaster;

                    if (obj is Point3D && m_Attendant.ControlMaster != null)
                        m_Attendant.MoveToWorld((Point3D)obj, m_Attendant.ControlMaster.Map);
                }
            }
        }
    }
}

namespace Server.ContextMenus
{
    public class AttendantFollowEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantFollowEntry(PersonalAttendant attendant)
            : base(6108)
        {
            m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (m_Attendant == null || m_Attendant.Deleted)
                return;

            m_Attendant.CommandFollow(Owner.From);
        }
    }

    public class AttendantStopEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantStopEntry(PersonalAttendant attendant)
            : base(6112)
        {
            m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (m_Attendant == null || m_Attendant.Deleted)
                return;

            m_Attendant.CommandStop(Owner.From);
        }
    }

    public class AttendantDismissEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantDismissEntry(PersonalAttendant attendant)
            : base(6228)
        {
            m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (m_Attendant == null || m_Attendant.Deleted)
                return;

            m_Attendant.Dismiss(Owner.From);
        }
    }

    public class AttendantUseEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantUseEntry(PersonalAttendant attendant, int title)
            : base(title)
        {
            m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (m_Attendant == null || m_Attendant.Deleted)
                return;

            m_Attendant.OnDoubleClick(Owner.From);
        }
    }
}