using Server.Network;
using Server.Spells.Third;
using System;

namespace Server.Items
{
    public class WrongBarredMetalDoor : BaseDoor, ILockpickable, IMageUnlockable
    {
        private int m_LockLevel, m_MaxLockLevel, m_RequiredSkill;
        private Mobile m_Picker;
        private Timer m_Timer;
        private bool m_MagicUnlocked;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Picker
        {
            get { return this.m_Picker; }
            set { this.m_Picker = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLockLevel
        {
            get { return this.m_MaxLockLevel; }
            set { this.m_MaxLockLevel = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LockLevel
        {
            get { return this.m_LockLevel; }
            set { this.m_LockLevel = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredSkill
        {
            get { return this.m_RequiredSkill; }
            set { this.m_RequiredSkill = value; }
        }

        [Constructable]
        public WrongBarredMetalDoor(DoorFacing facing)
            : base(0x685 + (2 * (int)facing), 0x686 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing))
        {
            this.Locked = true;
            this.m_LockLevel = 80;
            this.m_MaxLockLevel = 110;
            this.m_RequiredSkill = 100;
        }

        public WrongBarredMetalDoor(Serial serial) : base(serial)
        {
        }

        public virtual void LockPick(Mobile from)
        {
            this.Picker = from;
            this.Locked = false;
            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
        }

        public override void OnTelekinesis(Mobile from)
        {
            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();

            if (from.Skills.Magery.Value >= m_RequiredSkill)
            {
                m_MagicUnlocked = true;
            }

            Use(from);
        }

        public void OnMageUnlock(Mobile from)
        {
            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();

            if (from.Skills.Magery.Value >= m_RequiredSkill)
            {
                m_MagicUnlocked = true;
            }
        }

        public override void Use(Mobile from)
        {
            if (m_MagicUnlocked)
            {
                this.Locked = false;
                m_MagicUnlocked = false;
            }

            if (this.Locked && !this.Open)                
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501746); // It appears to be locked.
                return;
            }

            base.Use(from);
        }

        private class InternalTimer : Timer
        {
            private readonly BaseDoor m_Door;
            public InternalTimer(BaseDoor door)
                : base(TimeSpan.FromSeconds(10.0))
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_Door = door;
            }

            protected override void OnTick()
            {
                this.m_Door.Locked = true;
            }
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            
            writer.Write((int)m_RequiredSkill);
            writer.Write((int)m_MaxLockLevel);
            writer.Write((int)m_LockLevel);
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            
            m_RequiredSkill = reader.ReadInt();
            m_MaxLockLevel = reader.ReadInt();
            m_LockLevel = reader.ReadInt();

            this.Locked = true;
        }
    }
}
