using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;

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
            this.Title = title;
            this.Blessed = true;
            this.ControlSlots = 0;

            this.InitBody();
            this.InitOutfit();

            this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(2));
            this.m_Timer.Start();
        }

        public PersonalAttendant(Serial serial)
            : base(serial)
        {
        }

        public override bool ShowFameTitle
        {
            get
            {
                return true;
            }
        }
        public override bool Commandable
        {
            get
            {
                return false;
            }
        }
        public override bool NoHouseRestrictions
        {
            get
            {
                return true;
            }
        }
        public override bool CanOpenDoors
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BindedToPlayer
        {
            get
            {
                return this.m_BindedToPlayer;
            }
            set
            {
                this.m_BindedToPlayer = value;
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
            this.ControlOrder = OrderType.Follow;
            this.ControlTarget = by;

            if (this.m_Timer != null)
            {
                this.m_Timer.Interval = TimeSpan.FromSeconds(2);
                this.m_Timer.Priority = TimerPriority.OneSecond;
            }
        }

        public virtual void CommandStop(Mobile by)
        { 
            this.ControlOrder = OrderType.Stay;
            this.ControlTarget = null;

            if (this.m_Timer != null)
            {
                this.m_Timer.Interval = TimeSpan.FromSeconds(5);
                this.m_Timer.Priority = TimerPriority.FiveSeconds;
            }
        }

        public virtual void Dismiss(Mobile owner)
        {
            RemoveAttendant(owner);

            if (this.m_BindedToPlayer)
                owner.AddToBackpack(new PersonalAttendantDeed(owner));
            else
                owner.AddToBackpack(new PersonalAttendantDeed());
			
            this.Delete();
        }

        public virtual bool InGreetingMode(Mobile owner)
        {
            return false;
        }

        public virtual bool IsOwner(Mobile m)
        {
            return (this.ControlMaster == null || this.ControlMaster == m);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && this.IsOwner(from))
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

            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write(this.m_BindedToPlayer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    this.m_BindedToPlayer = reader.ReadBool();
                    break;
            }

            TimeSpan delay = TimeSpan.FromSeconds(2);

            if (this.ControlOrder == OrderType.Stay)
                delay = TimeSpan.FromSeconds(5);

            this.m_Timer = new InternalTimer(this, delay);
            this.m_Timer.Start();

            AddAttendant(this.ControlMaster, this);
        }

        private class InternalTimer : Timer
        {
            private readonly PersonalAttendant m_Attendant;
            public InternalTimer(PersonalAttendant attendant, TimeSpan delay)
                : base(delay, delay)
            {
                this.m_Attendant = attendant;

                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (this.m_Attendant != null && !this.m_Attendant.Deleted)
                {
                    Mobile m = this.m_Attendant.ControlMaster;

                    if (m != null)
                    {
                        if ((m.NetState == null || !m.Alive) && !this.m_Attendant.InGreetingMode(m)) 
                            this.m_Attendant.Dismiss(m);
                        else if (this.m_Attendant.ControlOrder == OrderType.Follow && !m.InRange(this.m_Attendant.Location, 12))
                            Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(CatchUp), m.Location);
                    }
                }
            }

            private void CatchUp(object obj)
            {
                if (this.m_Attendant != null && !this.m_Attendant.Deleted)
                {
                    this.m_Attendant.ControlOrder = OrderType.Follow;
                    this.m_Attendant.ControlTarget = this.m_Attendant.ControlMaster;

                    if (obj is Point3D && this.m_Attendant.ControlMaster != null)
                        this.m_Attendant.MoveToWorld((Point3D)obj, this.m_Attendant.ControlMaster.Map);
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
            this.m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (this.m_Attendant == null || this.m_Attendant.Deleted)
                return;

            this.m_Attendant.CommandFollow(this.Owner.From);
        }
    }

    public class AttendantStopEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantStopEntry(PersonalAttendant attendant)
            : base(6112)
        {
            this.m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (this.m_Attendant == null || this.m_Attendant.Deleted)
                return;

            this.m_Attendant.CommandStop(this.Owner.From);
        }
    }

    public class AttendantDismissEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantDismissEntry(PersonalAttendant attendant)
            : base(6228)
        {
            this.m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (this.m_Attendant == null || this.m_Attendant.Deleted)
                return;

            this.m_Attendant.Dismiss(this.Owner.From);
        }
    }

    public class AttendantUseEntry : ContextMenuEntry
    {
        private readonly PersonalAttendant m_Attendant;
        public AttendantUseEntry(PersonalAttendant attendant, int title)
            : base(title)
        {
            this.m_Attendant = attendant;
        }

        public override void OnClick()
        {
            if (this.m_Attendant == null || this.m_Attendant.Deleted)
                return;

            this.m_Attendant.OnDoubleClick(this.Owner.From);
        }
    }
}