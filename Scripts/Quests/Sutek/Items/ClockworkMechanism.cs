using System;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
    public class CompletedClockworkAssembly : Item
    {
        public CompletedClockworkAssembly()
            : base(0x1EA8)
        {
            this.Weight = 1.0;
        }

        public CompletedClockworkAssembly(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112879;
            }
        }// completed clockwork assembly
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1072351); // Quest Item
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
        }
    }

    public class ClockworkMechanism : Item
    {
        private static readonly int[] labeltypes = { 1112859, 1112861, 1029734, 1075914, 1075915, 1075917, 1029746 };
        //readonly int m_Remaining;
        //readonly SutekResourceType m_Resource;
        private int m_Type;
        private int m_Lifespan;
        private Timer m_Timer;
        [Constructable]
        public ClockworkMechanism()
            : base(0x1EA8)
        {
            this.Weight = 1.0;
            this.m_Lifespan = 3600;
            this.Movable = true;

            this.m_Type = labeltypes[Utility.Random(labeltypes.Length)];
            
            this.StartTimer();
        }

        public ClockworkMechanism(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item

            if (this.m_Lifespan > 0)
                list.Add(1072517, this.m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1112858, String.Concat("#", this.m_Type.ToString())); // ~1_TYPE~ clockwork mechanism
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (null == from || 0 == this.m_Lifespan)
                return;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                if (from.HasGump(typeof(ClockworkStartGump)))
                    from.CloseGump(typeof(ClockworkStartGump));

                from.SendGump(new ClockworkStartGump(this));
            }
        }

        public void BeginMechanismAssembly(Mobile from)
        {
            from.Target = new InternalTarget(from, this);
        }

        public override void OnDelete()
        {
            if (this.Deleted)
                return;

            if (null != this.m_Timer && this.m_Timer.Running)
            {
                this.StopTimer();
                this.m_Timer = null;
            }

            base.OnDelete();
        }

        public void StartTimer()
        {
            if (null == this.m_Timer)
                this.m_Timer = new InternalTimer(this);

            this.m_Timer.Start();
            this.Movable = false;
        }

        public void StopTimer()
        {
            this.m_Timer.Stop();
            this.m_Lifespan = 0;
        }

        public void OnTick()
        {
            this.m_Lifespan -= 10;
            this.InvalidateProperties();
            if (this.m_Lifespan <= 0)
            {
                this.StopTimer();
                if (this.RootParentEntity is Mobile)
                    ((Mobile)this.RootParentEntity).SendLocalizedMessage(1112822); // You fail to find the next ingredient in time. Your clockwork assembly crumbles.
                this.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_Type);
            writer.Write(this.m_Lifespan);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    this.m_Type = reader.ReadInt();
                    this.m_Lifespan = reader.ReadInt();
                    break;
            }

            this.StartTimer();
        }

        private class InternalTarget : Target
        {
            private readonly ClockworkMechanism m_Item;
            //private readonly bool m_Finished;
            private int m_Remaining;
            private SutekResourceType m_Type;
            private DateTime m_EndTime;
            private double m_Delay;
            public InternalTarget(Mobile from, ClockworkMechanism item)
                : this(from, item, SutekQuestResource.GetRandomResource(), Utility.RandomMinMax(10, 20), 10.0, DateTime.UtcNow + TimeSpan.FromSeconds(10.0))
            {
            }

            public InternalTarget(Mobile from, ClockworkMechanism item, SutekResourceType type, int remaining, double delay, DateTime endtime)
                : base(2, true, TargetFlags.None)
            {
                this.m_Item = item;
                this.m_Type = type;
                this.m_Remaining = remaining;
                this.m_EndTime = endtime;
                this.m_Delay = delay;

                from.SendLocalizedMessage(1112821, String.Concat("#", SutekQuestResource.GetLabelId(this.m_Type).ToString())); // I need to add some ~1_INGREDIENT~.

                this.BeginTimeout(from, this.m_EndTime - DateTime.UtcNow);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (this.m_Remaining > 0)
                    from.Target = new InternalTarget(from, this.m_Item, this.m_Type, this.m_Remaining, this.m_Delay, this.m_EndTime);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (TargetCancelType.Timeout == cancelType)
                {
                    this.CancelTimeout();
                    this.m_Remaining = 0;
                    from.SendLocalizedMessage(1112822); // You fail to find the next ingredient in time. Your clockwork assembly crumbles.
                    this.m_Item.Delete();
                }
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item == null || this.m_Item.Deleted)
                    return;

                if (targeted is SutekQuestResource && ((SutekQuestResource)targeted).ActualType == this.m_Type)
                {
                    this.CancelTimeout();

                    if (--this.m_Remaining > 0)    
                    {
                        from.SendLocalizedMessage(1112819); // You've successfully added this ingredient.
                        this.m_Type = SutekQuestResource.GetRandomResource();
                        this.m_Delay += 0.1 + (Utility.RandomDouble() / 4.0);
                        this.m_EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(this.m_Delay);
                    }
                    else
                    {
                        this.m_Item.StopTimer();
                        Item item = new CompletedClockworkAssembly();
                        if (null == this.m_Item.Parent && this.m_Item.Parent is Container)
                        {
                            item.Location = new Point3D(this.m_Item.Location);
                            ((Container)this.m_Item.Parent).AddItem(item);
                        }
                        else
                            item.MoveToWorld(this.m_Item.GetWorldLocation(), this.m_Item.Map);

                        this.m_Item.Delete();

                        from.SendLocalizedMessage(1112987); // The training clockwork fails and the creature vanishes.
                        from.SendLocalizedMessage(1112872); // Your assembly is completed. Return it to Sutek for your reward!
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1112820); // That is not the right ingredient.
                    this.OnTargetCancel(from, TargetCancelType.Timeout);
                }
            }
        }

        private class InternalTimer : Timer
        {
            private readonly ClockworkMechanism i_item;
            public InternalTimer(ClockworkMechanism item)
                : base(TimeSpan.FromSeconds(10.0), TimeSpan.FromSeconds(10.0))
            {
                this.Priority = TimerPriority.OneSecond;
                this.i_item = item;
            }

            protected override void OnTick()
            {
                if (null == this.i_item || this.i_item.Deleted)
                {
                    this.Stop();
                    return;
                }

                this.i_item.OnTick();
            }
        }
    }
}