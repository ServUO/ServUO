using System;
using System.Collections;
using Server.Mobiles;

/*
** TimedLever, TimedSwitch, and XmlLatch class and TimedSwitchableItem
** Version 1.01
** updated 5/06/04
** ArteGordon
*/
namespace Server.Items
{
    public class XmlLatch : Item
    {
        private TimeSpan m_MinDelay;
        private TimeSpan m_MaxDelay;
        private DateTime m_End;
        private InternalTimer m_Timer;
        private int m_State = 0;
        private int m_ResetState = 0;
        [Constructable]
        public XmlLatch()
            : base(0x1BBF)
        {
            this.Movable = false;
        }

        public XmlLatch(int itemID)
            : base(itemID)
        {
        }

        public XmlLatch(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan MinDelay
        {
            get
            {
                return this.m_MinDelay;
            }
            set
            {
                this.m_MinDelay = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan MaxDelay
        {
            get
            {
                return this.m_MaxDelay;
            }
            set
            {
                this.m_MaxDelay = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan TimeUntilReset
        {
            get
            {
                if (this.m_Timer != null && this.m_Timer.Running)
                    return this.m_End - DateTime.UtcNow;
                else
                    return TimeSpan.FromSeconds(0);
            }
            set
            {
                this.DoTimer(value);
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public virtual int ResetState
        {
            get
            {
                return this.m_ResetState;
            }
            set
            {
                this.m_ResetState = value;
                if (this.m_Timer != null && this.m_Timer.Running)
                    this.m_Timer.Stop();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public virtual int State
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;
                this.StartTimer();
                this.InvalidateProperties();
            }
        }
        public void StartTimer()
        {
            if (this.m_State != this.m_ResetState && (this.m_MinDelay > TimeSpan.Zero || this.m_MaxDelay > TimeSpan.Zero))
                this.DoTimer();
            else if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
        }

        public virtual void OnReset()
        {
            this.State = this.ResetState;
        }

        public void DoTimer()
        {
            int minSeconds = (int)this.m_MinDelay.TotalSeconds;
            int maxSeconds = (int)this.m_MaxDelay.TotalSeconds;

            TimeSpan delay = TimeSpan.FromSeconds(Utility.RandomMinMax(minSeconds, maxSeconds));
            this.DoTimer(delay);
        }

        public void DoTimer(TimeSpan delay)
        {
            this.m_End = DateTime.UtcNow + delay;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new InternalTimer(this, delay);
            this.m_Timer.Start();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            // version 0
            writer.Write(this.m_State);
            writer.Write(this.m_ResetState);
            writer.Write(this.m_MinDelay);
            writer.Write(this.m_MaxDelay);
            bool running = (this.m_Timer != null && this.m_Timer.Running);
            writer.Write(running);
            if (this.m_Timer != null && this.m_Timer.Running)
                writer.Write(this.m_End - DateTime.UtcNow);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        // note this is redundant with the base class serialization, but it is there for older (pre 1.02) version compatibility
                        // not needed
                        this.m_State = reader.ReadInt();
                        this.m_ResetState = reader.ReadInt();
                        this.m_MinDelay = reader.ReadTimeSpan();
                        this.m_MaxDelay = reader.ReadTimeSpan();
                        bool running = reader.ReadBool();
                        if (running)
                        {
                            TimeSpan delay = reader.ReadTimeSpan();
                            this.DoTimer(delay);
                        }
                    }
                    break;
            }
        }

        private class InternalTimer : Timer
        {
            private readonly XmlLatch m_latch;
            public InternalTimer(XmlLatch xmllatch, TimeSpan delay)
                : base(delay)
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_latch = xmllatch;
            }

            protected override void OnTick()
            {
                if (this.m_latch != null && !this.m_latch.Deleted)
                {
                    this.Stop();
                    this.m_latch.OnReset();
                }
            }
        }
    }

    public class TimedLever : XmlLatch, ILinkable
    {
        private leverType m_LeverType = leverType.Two_State;
        private int m_LeverSound = 936;
        private Item m_TargetItem0 = null;
        private string m_TargetProperty0 = null;
        private Item m_TargetItem1 = null;
        private string m_TargetProperty1 = null;
        private Item m_TargetItem2 = null;
        private string m_TargetProperty2 = null;
        private Item m_LinkedItem = null;
        private bool already_being_activated = false;
        private bool m_Disabled = false;
        [Constructable]
        public TimedLever()
            : base(0x108C)
        {
            this.Name = "A lever";
            this.Movable = false;
        }

        public TimedLever(Serial serial)
            : base(serial)
        {
        }

        public enum leverType
        {
            Two_State,
            Three_State
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool Disabled
        {
            set
            {
                this.m_Disabled = value;
            }
            get
            {
                return this.m_Disabled;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Link
        {
            set
            {
                this.m_LinkedItem = value;
            }
            get
            {
                return this.m_LinkedItem;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public override int State
        {
            get
            {
                return base.State;
            }
            set
            {
                // prevent infinite recursion 
                if (!this.already_being_activated)
                {
                    this.already_being_activated = true;
                    this.Activate(null, value, null);
                    this.already_being_activated = false;
                }

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int LeverSound
        {
            get
            {
                return this.m_LeverSound;
            }
            set
            {
                this.m_LeverSound = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public leverType LeverType
        {
            get
            {
                return this.m_LeverType;
            }
            set
            {
                this.m_LeverType = value;
                this.State = 0;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        new public virtual Direction Direction
        {
            get
            {
                return base.Direction;
            }
            set
            {
                base.Direction = value;
                this.SetLeverStatic();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target0Item
        {
            get
            {
                return this.m_TargetItem0;
            }
            set
            {
                this.m_TargetItem0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0Property
        {
            get
            {
                return this.m_TargetProperty0;
            }
            set
            {
                this.m_TargetProperty0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0ItemName
        {
            get
            {
                if (this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted)
                    return this.m_TargetItem0.Name;
                else
                    return null;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target1Item
        {
            get
            {
                return this.m_TargetItem1;
            }
            set
            {
                this.m_TargetItem1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1Property
        {
            get
            {
                return this.m_TargetProperty1;
            }
            set
            {
                this.m_TargetProperty1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1ItemName
        {
            get
            {
                if (this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted)
                    return this.m_TargetItem1.Name;
                else
                    return null;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target2Item
        {
            get
            {
                return this.m_TargetItem2;
            }
            set
            {
                this.m_TargetItem2 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target2Property
        {
            get
            {
                return this.m_TargetProperty2;
            }
            set
            {
                this.m_TargetProperty2 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target2ItemName
        {
            get
            {
                if (this.m_TargetItem2 != null && !this.m_TargetItem2.Deleted)
                    return this.m_TargetItem2.Name;
                else
                    return null;
            }
        }
        public void Activate(Mobile from, int state, ArrayList links)
        {
            if (this.Disabled)
                return;

            string status_str = null;

            if (state < 0)
                state = 0;
            if (state > 1 && this.m_LeverType == leverType.Two_State)
                state = 1;
            if (state > 2)
                state = 2;

            // assign the latch state and start the timer
            base.State = state;

            // update the graphic
            this.SetLeverStatic();

            // play the switching sound
            //if (from != null)
            //{
            //	from.PlaySound(m_LeverSound);
            //}
            try
            {
                Effects.PlaySound(this.Location, this.Map, this.m_LeverSound);
            }
            catch
            {
            }

            // if a target object has been specified then apply the property modification
            if (state == 0 && this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted && this.m_TargetProperty0 != null && this.m_TargetProperty0.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty0, this.m_TargetItem0, null, this, out status_str);
            }
            if (state == 1 && this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted && this.m_TargetProperty1 != null && this.m_TargetProperty1.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty1, this.m_TargetItem1, null, this, out status_str);
            }
            if (state == 2 && this.m_TargetItem2 != null && !this.m_TargetItem2.Deleted && this.m_TargetProperty2 != null && this.m_TargetProperty2.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty2, this.m_TargetItem2, null, this, out status_str);
            }

            // if the switch is linked, then activate the link as well
            if (this.Link != null && this.Link is ILinkable)
            {
                if (links == null)
                {
                    links = new ArrayList();
                }
                // activate other linked objects if they have not already been activated
                if (!links.Contains(this))
                {
                    links.Add(this);

                    ((ILinkable)this.Link).Activate(from, state, links);
                }
            }

            // report any problems to staff
            if (status_str != null && from != null && from.IsStaff())
            {
                from.SendMessage("{0}", status_str);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            // version 2
            writer.Write(this.m_Disabled);
            // version 1
            writer.Write(this.m_LinkedItem);
            // version 0
            writer.Write(this.m_LeverSound);
            writer.Write((int)this.m_LeverType);
            writer.Write(this.m_TargetItem0);
            writer.Write(this.m_TargetProperty0);
            writer.Write(this.m_TargetItem1);
            writer.Write(this.m_TargetProperty1);
            writer.Write(this.m_TargetItem2);
            writer.Write(this.m_TargetProperty2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    {
                        this.m_Disabled = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_LinkedItem = reader.ReadItem();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_LeverSound = reader.ReadInt();
                        int ltype = reader.ReadInt();
                        switch (ltype)
                        {
                            case (int)leverType.Two_State:
                                this.m_LeverType = leverType.Two_State;
                                break;
                            case (int)leverType.Three_State:
                                this.m_LeverType = leverType.Three_State;
                                break;
                        }
                        this.m_TargetItem0 = reader.ReadItem();
                        this.m_TargetProperty0 = reader.ReadString();
                        this.m_TargetItem1 = reader.ReadItem();
                        this.m_TargetProperty1 = reader.ReadString();
                        this.m_TargetItem2 = reader.ReadItem();
                        this.m_TargetProperty2 = reader.ReadString();
                    }
                    break;
            }
            // refresh the lever static to reflect the state
            this.SetLeverStatic();
        }

        public void SetLeverStatic()
        {
            switch (this.Direction)
            {
                case Direction.North:
                case Direction.South:
                case Direction.Right:
                case Direction.Up:
                    if (this.m_LeverType == leverType.Two_State)
                        this.ItemID = 0x108c + this.State * 2;
                    else
                        this.ItemID = 0x108c + this.State;
                    break;
                case Direction.East:
                case Direction.West:
                case Direction.Left:
                case Direction.Down:
                    if (this.m_LeverType == leverType.Two_State)
                        this.ItemID = 0x1093 + this.State * 2;
                    else
                        this.ItemID = 0x1093 + this.State;
                    break;
                default:
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || this.Disabled)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }
            // animate and change state
            int newstate = this.State + 1;
            if (newstate > 1 && this.m_LeverType == leverType.Two_State)
                newstate = 0;
            if (newstate > 2)
                newstate = 0;

            // carry out the switch actions
            this.Activate(from, newstate, null);
        }
    }

    public class TimedSwitch : XmlLatch, ILinkable
    {
        private int m_SwitchSound = 939;
        private Item m_TargetItem0 = null;
        private string m_TargetProperty0 = null;
        private Item m_TargetItem1 = null;
        private string m_TargetProperty1 = null;
        private Item m_LinkedItem = null;
        private bool already_being_activated = false;
        private bool m_Disabled = false;
        [Constructable]
        public TimedSwitch()
            : base(0x108F)
        {
            this.Name = "A switch";
            this.Movable = false;
        }

        public TimedSwitch(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public bool Disabled
        {
            set
            {
                this.m_Disabled = value;
            }
            get
            {
                return this.m_Disabled;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Link
        {
            set
            {
                this.m_LinkedItem = value;
            }
            get
            {
                return this.m_LinkedItem;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public override int State
        {
            get
            {
                return base.State;
            }
            set
            {
                // prevent infinite recursion 
                if (!this.already_being_activated)
                {
                    this.already_being_activated = true;
                    this.Activate(null, value, null);
                    this.already_being_activated = false;
                }

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int SwitchSound
        {
            get
            {
                return this.m_SwitchSound;
            }
            set
            {
                this.m_SwitchSound = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        new public virtual Direction Direction
        {
            get
            {
                return base.Direction;
            }
            set
            {
                base.Direction = value;
                this.SetSwitchStatic();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target0Item
        {
            get
            {
                return this.m_TargetItem0;
            }
            set
            {
                this.m_TargetItem0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0Property
        {
            get
            {
                return this.m_TargetProperty0;
            }
            set
            {
                this.m_TargetProperty0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0ItemName
        {
            get
            {
                if (this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted)
                    return this.m_TargetItem0.Name;
                else
                    return null;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target1Item
        {
            get
            {
                return this.m_TargetItem1;
            }
            set
            {
                this.m_TargetItem1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1Property
        {
            get
            {
                return this.m_TargetProperty1;
            }
            set
            {
                this.m_TargetProperty1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1ItemName
        {
            get
            {
                if (this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted)
                    return this.m_TargetItem1.Name;
                else
                    return null;
            }
        }
        public void Activate(Mobile from, int state, ArrayList links)
        {
            if (this.Disabled)
                return;

            string status_str = null;

            if (state < 0)
                state = 0;
            if (state > 1)
                state = 1;

            // assign the latch state and start the timer
            base.State = state;

            // update the graphic
            this.SetSwitchStatic();

            // play the switching sound
            //if (from != null)
            //{
            //	from.PlaySound(m_SwitchSound);
            //}
            try
            {
                Effects.PlaySound(this.Location, this.Map, this.m_SwitchSound);
            }
            catch
            {
            }

            // if a target object has been specified then apply the property modification
            if (state == 0 && this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted && this.m_TargetProperty0 != null && this.m_TargetProperty0.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty0, this.m_TargetItem0, null, this, out status_str);
            }
            if (state == 1 && this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted && this.m_TargetProperty1 != null && this.m_TargetProperty1.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty1, this.m_TargetItem1, null, this, out status_str);
            }

            // if the switch is linked, then activate the link as well
            if (this.Link != null && this.Link is ILinkable)
            {
                if (links == null)
                {
                    links = new ArrayList();
                }
                // activate other linked objects if they have not already been activated
                if (!links.Contains(this))
                {
                    links.Add(this);

                    ((ILinkable)this.Link).Activate(from, state, links);
                }
            }

            // report any problems to staff
            if (status_str != null && from != null && from.IsStaff())
            {
                from.SendMessage("{0}", status_str);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            // version 2
            writer.Write(this.m_Disabled);
            // version 1
            writer.Write(this.m_LinkedItem);
            // version 0
            writer.Write(this.m_SwitchSound);
            writer.Write(this.m_TargetItem0);
            writer.Write(this.m_TargetProperty0);
            writer.Write(this.m_TargetItem1);
            writer.Write(this.m_TargetProperty1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    {
                        this.m_Disabled = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_LinkedItem = reader.ReadItem();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_SwitchSound = reader.ReadInt();
                        this.m_TargetItem0 = reader.ReadItem();
                        this.m_TargetProperty0 = reader.ReadString();
                        this.m_TargetItem1 = reader.ReadItem();
                        this.m_TargetProperty1 = reader.ReadString();
                    }
                    break;
            }
            // refresh the lever static to reflect the state
            this.SetSwitchStatic();
        }

        public void SetSwitchStatic()
        {
            switch (this.Direction)
            {
                case Direction.North:
                case Direction.South:
                case Direction.Right:
                case Direction.Up:
                    this.ItemID = 0x108f + this.State;
                    break;
                case Direction.East:
                case Direction.West:
                case Direction.Left:
                case Direction.Down:
                    this.ItemID = 0x1091 + this.State;
                    break;
                default:
                    this.ItemID = 0x108f + this.State;
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || this.Disabled)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }
            // animate and change state
            int newstate = this.State + 1;
            if (newstate > 1)
                newstate = 0;

            // carry out the switch actions
            this.Activate(from, newstate, null);
        }
    }

    public class TimedSwitchableItem : XmlLatch, ILinkable
    {
        private int m_SwitchSound = 939;
        private Item m_TargetItem0 = null;
        private string m_TargetProperty0 = null;
        private Item m_TargetItem1 = null;
        private string m_TargetProperty1 = null;
        private int m_ItemID0 = 0x108F;
        private int m_ItemID1 = 0x1090;
        private Item m_LinkedItem = null;
        private bool already_being_activated = false;
        private Point3D m_Offset = Point3D.Zero;
        private bool m_Disabled = false;
        private bool m_NoDoubleClick = false;
        [Constructable]
        public TimedSwitchableItem()
            : base(0x108F)
        {
            this.Name = "A switchable item";
            this.Movable = false;
        }

        public TimedSwitchableItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Spawner)]
        public bool Disabled
        {
            set
            {
                this.m_Disabled = value;
            }
            get
            {
                return this.m_Disabled;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool NoDoubleClick
        {
            set
            {
                this.m_NoDoubleClick = value;
            }
            get
            {
                return this.m_NoDoubleClick;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Point3D Offset
        {
            set
            {
                this.m_Offset = value;
            }
            get
            {
                return this.m_Offset;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Link
        {
            set
            {
                this.m_LinkedItem = value;
            }
            get
            {
                return this.m_LinkedItem;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public override int State
        {
            get
            {
                return base.State;
            }
            set
            {
                // prevent infinite recursion 
                if (!this.already_being_activated)
                {
                    this.already_being_activated = true;
                    this.Activate(null, value, null);
                    this.already_being_activated = false;
                }

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int ItemID0
        {
            get
            {
                return this.m_ItemID0;
            }
            set
            {
                this.m_ItemID0 = value;
                // refresh the lever static to reflect the state
                this.SetSwitchStatic();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int ItemID1
        {
            get
            {
                return this.m_ItemID1;
            }
            set
            {
                this.m_ItemID1 = value;
                // refresh the lever static to reflect the state
                this.SetSwitchStatic();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int SwitchSound
        {
            get
            {
                return this.m_SwitchSound;
            }
            set
            {
                this.m_SwitchSound = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target0Item
        {
            get
            {
                return this.m_TargetItem0;
            }
            set
            {
                this.m_TargetItem0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0Property
        {
            get
            {
                return this.m_TargetProperty0;
            }
            set
            {
                this.m_TargetProperty0 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target0ItemName
        {
            get
            {
                if (this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted)
                    return this.m_TargetItem0.Name;
                else
                    return null;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public Item Target1Item
        {
            get
            {
                return this.m_TargetItem1;
            }
            set
            {
                this.m_TargetItem1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1Property
        {
            get
            {
                return this.m_TargetProperty1;
            }
            set
            {
                this.m_TargetProperty1 = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public string Target1ItemName
        {
            get
            {
                if (this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted)
                    return this.m_TargetItem1.Name;
                else
                    return null;
            }
        }
        public void Activate(Mobile from, int state, ArrayList links)
        {
            if (this.Disabled)
                return;

            string status_str = null;

            if (state < 0)
                state = 0;
            if (state > 1)
                state = 1;

            if (base.State != state)
            {
                // apply the offset
                this.SetSwitchOffset();
            }
            // assign the latch state and start the timer
            base.State = state;

            // update the graphic
            this.SetSwitchStatic();

            // play the switching sound
            //if (from != null)
            //{
            //	from.PlaySound(m_SwitchSound);
            //}
            try
            {
                Effects.PlaySound(this.Location, this.Map, this.m_SwitchSound);
            }
            catch
            {
            }

            // if a target object has been specified then apply the property modification
            if (state == 0 && this.m_TargetItem0 != null && !this.m_TargetItem0.Deleted && this.m_TargetProperty0 != null && this.m_TargetProperty0.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty0, this.m_TargetItem0, null, this, out status_str);
            }
            if (state == 1 && this.m_TargetItem1 != null && !this.m_TargetItem1.Deleted && this.m_TargetProperty1 != null && this.m_TargetProperty1.Length > 0)
            {
                BaseXmlSpawner.ApplyObjectStringProperties(null, this.m_TargetProperty1, this.m_TargetItem1, null, this, out status_str);
            }

            // if the switch is linked, then activate the link as well
            if (this.Link != null && this.Link is ILinkable)
            {
                if (links == null)
                {
                    links = new ArrayList();
                }
                // activate other linked objects if they have not already been activated
                if (!links.Contains(this))
                {
                    links.Add(this);

                    ((ILinkable)this.Link).Activate(from, state, links);
                }
            }

            // report any problems to staff
            if (status_str != null && from != null && from.IsStaff())
            {
                from.SendMessage("{0}", status_str);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4); // version
            // version 4
            writer.Write(this.m_NoDoubleClick);
            // version 3
            writer.Write(this.m_Disabled);
            writer.Write(this.m_Offset);
            // version 2
            writer.Write(this.m_LinkedItem);
            // version 1
            writer.Write(this.m_ItemID0);
            writer.Write(this.m_ItemID1);
            // version 0
            writer.Write(this.m_SwitchSound);
            writer.Write(this.m_TargetItem0);
            writer.Write(this.m_TargetProperty0);
            writer.Write(this.m_TargetItem1);
            writer.Write(this.m_TargetProperty1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 4:
                    {
                        this.m_NoDoubleClick = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        this.m_Disabled = reader.ReadBool();
                        this.m_Offset = reader.ReadPoint3D();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_LinkedItem = reader.ReadItem();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_ItemID0 = reader.ReadInt();
                        this.m_ItemID1 = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_SwitchSound = reader.ReadInt();
                        this.m_TargetItem0 = reader.ReadItem();
                        this.m_TargetProperty0 = reader.ReadString();
                        this.m_TargetItem1 = reader.ReadItem();
                        this.m_TargetProperty1 = reader.ReadString();
                    }
                    break;
            }
            // refresh the lever static to reflect the state
            this.SetSwitchStatic();
        }

        public void SetSwitchStatic()
        {
            switch (this.State)
            {
                case 0:
                    this.ItemID = this.ItemID0;
                    break;
                case 1:
                    this.ItemID = this.ItemID1;
                    break;
            }
        }

        public void SetSwitchOffset()
        {
            switch (this.State)
            {
                case 0:
                    this.Location = new Point3D(this.X - this.m_Offset.X, this.Y - this.m_Offset.Y, this.Z - this.m_Offset.Z);
                    break;
                case 1:
                    this.Location = new Point3D(this.X + this.m_Offset.X, this.Y + this.m_Offset.Y, this.Z + this.m_Offset.Z);
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || this.Disabled || this.NoDoubleClick)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }
            // animate and change state
            int newstate = this.State + 1;
            if (newstate > 1)
                newstate = 0;

            // carry out the switch actions
            this.Activate(from, newstate, null);
        }
    }
}